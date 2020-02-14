using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.IO;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Environment; 

namespace Aquamonix.Mobile.Lib.Database
{
    /// <summary>
    /// Controls caching of the application's most important data; all devices & metadata. 
    /// Controls also to some extent, merging of device updates and notification to the rest of the application 
    /// when device updates are ready to consume.
    /// </summary>
	public static class DataCache
	{
		private static readonly bool PreserveDeviceOrder = true;

        public static int CurrentProgramId { get; set; }

        //where devices are stored in memory
		private static ConcurrentDictionary<string, DeviceCacheItem> _cachedDevices = new ConcurrentDictionary<string, DeviceCacheItem>();
        
        //where progress updates for commands are stored in memory
		private static ConcurrentDictionary<string, CommandProgress> _commandProgresses = new ConcurrentDictionary<string, CommandProgress>();
		
        //list of registered listeners for global device updates
        private static List<Action<IEnumerable<Device>>> _globalListeners = new List<Action<IEnumerable<Device>>>();

        //keeps the correct device order 
		private static List<string> _deviceOrderIds = new List<string>(); 

        //location to storage file
		private static string _filePath;

        //current number of active alerts 
		private static int _activeAlertsCount = 0;

        //cached server version 
        private static ServerVersion? _serverVersion = null;

        //used for thread-locking 
		private static readonly object _syncLock = new Object(); 


        /// <summary>
        /// Gets/sets the current base of app metadata.
        /// </summary>
		public static ApplicationMetadata ApplicationMetadata { get; set;}

        /// <summary>
        /// Gets the cached or current server version 
        /// </summary>
        /// <value>The server version.</value>
        public static ServerVersion ServerVersion
        {
            get
            {
                if (!_serverVersion.HasValue || _serverVersion.Value.IsNull || !_serverVersion.Value.IsValid)
                {
                    _serverVersion = ServiceContainer.ServerVersion;
                }
                return _serverVersion.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not there are currently devices available in memory.
        /// </summary>
		public static bool HasDevices
		{
            get { return _cachedDevices.Count > 0; } 
		}

        /// <summary>
        /// Gets the current number of active alerts.
        /// </summary>
		public static int ActiveAlertsCount
		{
			get { return _activeAlertsCount;}
		}

        /// <summary>
        /// Attempts to retrieve a device from cache.
        /// </summary>
        /// <param name="id">Id of the device to retrieve</param>
        /// <returns>The device from cache, or null</returns>
		public static Device GetDeviceFromCache(string id)
		{
			return ExceptionUtility.Try<Device>(() =>
			{
				var value = GetFromCacheInternal(id);
				if (value != null)
				{
					LogUtility.LogMessage("Device " + id + " retrieved from cache.");
					return value.Device;
				}

				return null;
			});
   		}

        /// <summary>
        /// Returns all devices currently in cache.
        /// </summary>
        /// <returns>List of Device objects</returns>
		public static IEnumerable<Device> GetAllDevicesFromCache()
		{
			return ExceptionUtility.Try<IEnumerable<Device>>(() =>
			{
				var list = _cachedDevices.Values.Select((r) => r.Device).ToList();

				//order the list 
				if (PreserveDeviceOrder)
					list = OrderDevices(list);

				return list;
			});
   		}

        /// <summary>
        /// If the given device (same id) already exists in cache, updates/merges the device objects into one. 
        /// Otherwise, adds the given device object to cache.
        /// </summary>
        /// <param name="device">Device to add or update</param>
		public static void AddOrUpdate(Device device)
		{
			AddOrUpdateInternal(device);
			if (device != null)
			{
				CallGlobalCallbacks();
				Save();
			}
		}

        /// <summary>
        /// Given a standard DevicesResponse, reads all devices from it and caches them (or updates their values in cache)
        /// </summary>
        /// <param name="response">A DevicesResponse from server</param>
		public static void CacheDevicesResponse(DevicesResponse response)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage(String.Format("Caching response {0} in app DB", response?.Header?.Type)); 
				if (response != null && response.IsSuccessful && response?.Body?.Devices != null)
				{
					CacheDevicesResponse(response.Body.Devices?.Items);
				}
			});
		}

        /// <summary>
        /// Given a standard AlertsResponse, loops through all alerts in it and merges them  into the appropriate 
        /// cached devices.
        /// </summary>
        /// <param name="response">Standard AlertsResponse from server.</param>
		public static void HandleAlertsResponse(AlertsResponse response)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("Caching alerts response in app DB");
				if (response != null && response.IsSuccessful && response.Body.Alerts != null)
				{
                    DataCache.CacheAlerts(response.Body.Alerts.Values);
				}
			});
		}

        public static void CacheAlerts(IEnumerable<Alert> alerts, bool deleteNonexisting = false)
        {
            ExceptionUtility.Try(() => 
            { 
                if (alerts != null)
                {
                    //merge alerts into appropriate devices 
                    List<string> deviceIds = new List<string>();
                    foreach (var alert in alerts)
                    {
                        if (!String.IsNullOrEmpty(alert.DeviceId))
                        {
                            var cacheItem = GetFromCacheInternal(alert.DeviceId);
                            if (cacheItem != null && cacheItem.Device != null)
                            {
                                var device = cacheItem.Device;
                                device.MergeAlert(alert);

                                if (!deviceIds.Contains(device.Id))
                                    deviceIds.Add(device.Id);
                            }
                        }
                    }

                    //delete alerts from devices, if not in list 
                    if (deleteNonexisting)
                    {
                        var devices = GetAllDevicesFromCache();

                        foreach(var device in devices)
                        {
                            var toRemove = new List<string>();

                            if (device.Alerts != null)
                            {
                                foreach(var alert in device.Alerts)
                                {
                                    if (alerts.FirstOrDefault((a) => (a.Id == alert.Value.Id)) == null)
                                    {
                                        toRemove.Add(alert.Key);
                                    }
                                }
                                device.AlertsCount = device.Alerts.Count;
                            }

                            if (toRemove.Count > 0)
                            {
                                foreach(string id in toRemove)
                                {
                                    device.Alerts.Remove(id); 
                                }
                            }
                        }
                    }

                    //broadcast updates for all devices
                    foreach (var id in deviceIds)
                        TriggerDeviceUpdate(id);

                    //limit number of alerts 
                    LimitAlerts();
                }
            }); 
        }

        public static void CacheAlertsForDevice(string deviceId, IEnumerable<Alert> alerts, bool deleteNonexisting = false)
        {
            ExceptionUtility.Try(() =>
            {
                if (alerts != null)
                {
                    var device = GetDeviceFromCache(deviceId);

                    foreach (var alert in alerts)
                    {
                        if (!String.IsNullOrEmpty(alert.DeviceId) && alert.DeviceId == deviceId)
                        {
                            device.MergeAlert(alert);
                        }
                    }

                    //delete alerts from devices, if not in list 
                    if (deleteNonexisting)
                    {
                        if (device != null)
                        {
                            var toRemove = new List<string>();

                            if (device.Alerts != null)
                            {
                                foreach (var alert in device.Alerts)
                                {
                                    if (alerts.FirstOrDefault((a) => (a.Id == alert.Value.Id)) == null)
                                    {
                                        toRemove.Add(alert.Key);
                                    }
                                }
                                device.AlertsCount = device.Alerts.Count;
                            }

                            if (toRemove.Count > 0)
                            {
                                foreach (string id in toRemove)
                                {
                                    device.Alerts.Remove(id);
                                }
                            }
                        }
                    }

                    //broadcast updates for device
                    TriggerDeviceUpdate(deviceId);

                    //limit number of alerts 
                    LimitAlerts();
                }
            });
        }

        public static IEnumerable<AlertViewModel> GetGlobalAlerts()
        {
            var devices = DataCache.GetAllDevicesFromCache();
            var alerts = new List<AlertViewModel>();

            foreach (var device in devices)
            {
                if (device.Alerts != null)
                {
                    foreach (var alert in device.Alerts)
                    {
                        var alertViewModel = new AlertViewModel(alert.Value, device);
                        alerts.Add(alertViewModel);
                    }
                }
            }

            return alerts.OrderBy((a) => a.Date).Reverse().ToList();
        }

        /// <summary>
        /// Given a list of devices, reads all devices from it and caches them (or updates their values in cache)
        /// </summary>
        /// <param name="devices">List of devices.</param>
		public static void CacheDevicesResponse(IDictionary<string, Device> devices)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage(String.Format("Caching response in app DB"));
				if (devices != null && devices.Count() > 0)
				{
					foreach (var d in devices.Values)
					{
						AddOrUpdateInternal(d);
					}

					CallGlobalCallbacks();
					Save();
				}
			});
		}

        /// <summary>
        /// Registers a listener for global device updates (for any device)
        /// </summary>
        /// <param name="callback">The callback to register</param>
		public static void SubscribeToAllDeviceUpdates(Action<IEnumerable<Device>> callback)
		{
			ExceptionUtility.Try(() =>
			{
				_globalListeners.Add(callback);
			});
		}

        /// <summary>
        /// Unregisters a listener from global device updates (for any device)
        /// </summary>
        /// <param name="callback">The callback to unregister</param>
		public static void UnsubscribeFromAllDeviceUpdates(Action<IEnumerable<Device>> callback)
		{
			ExceptionUtility.Try(() =>
			{
				_globalListeners.Remove(callback);
			});
		}

        /// <summary>
        /// Registers a listener for updates to a specific device.
        /// </summary>
        /// <param name="id">Id of the device for which to receive updates</param>
        /// <param name="callback">The callback listener</param>
		public static void SubscribeToDeviceUpdates(string id, Action<Device> callback)
		{
			ExceptionUtility.Try(() =>
			{
				DeviceCacheItem value = null;

				if (_cachedDevices.TryGetValue(id, out value))
				{
					value.AddUpdateCallback(callback);
				}

				LogUtility.LogMessage(id + " subscribing to device updates.");
			});
		}

        /// <summary>
        /// Unregisters a listener from updates to a specific device.
        /// </summary>
        /// <param name="id">Id of the device for which to no longer receive updates</param>
        /// <param name="callback">The callback listener</param>
		public static void UnsubscribeFromDeviceUpdates(string id, Action<Device> callback)
		{
			ExceptionUtility.Try(() =>
			{
				DeviceCacheItem value = null;

				if (_cachedDevices.TryGetValue(id, out value))
				{
					value.RemoveUpdateCallback(callback);
				}

				LogUtility.LogMessage(id + " unsubscribed from device updates.");
			});
		}

        /// <summary>
        /// Call on app startup.
        /// </summary>
		public static void Initialize()
		{
			_filePath = Path.Combine(FileUtility.GetCachesDirectory(), "Database.db");

			ReadFromCache();
		}

        /// <summary>
        /// Read in any values previously saved to permanent storage.
        /// </summary>
		public static void ReadFromCache()
		{
			ExceptionUtility.Try(() =>
			{
				if (FileUtility.FileExists(_filePath))
				{
					LogUtility.LogMessage("Cache: restoring from file.");

					string json = FileUtility.ReadAllText(_filePath);
					var cachedData = JsonUtility.Deserialize<JsonCacheObject>(json);

					_cachedDevices.Clear();

					if (cachedData != null)
					{
						ApplicationMetadata = cachedData.Metadata;
                        if (cachedData.ServerVersion != null)
                            _serverVersion = ServerVersion.Parse(cachedData.ServerVersion);

						if (cachedData.Devices != null)
						{
							foreach (var device in cachedData.Devices)
							{
								AddOrUpdateInternal(device);
							}
						}

						//get the command progresses
						if (cachedData.Commands != null)
						{
							foreach (var command in cachedData.Commands)
							{
								_commandProgresses.AddOrUpdate(command.CommandId, (arg) => command, (arg1, arg2) => command);
							}
						}
					}
				}
				else {
					LogUtility.LogMessage("Cache: file not found.");
				}
			});
		}

        /// <summary>
        /// Save current state to permanent storage.
        /// </summary>
		public static void Save()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("Saving cache.");

                var cacheObj = new JsonCacheObject()
                {
                    Metadata = ApplicationMetadata,
                    Devices = _cachedDevices.Select((r) => r.Value.Device).ToList(),
                    Commands = _commandProgresses.Values.ToList(),
                    ServerVersion = ServiceContainer.ServerVersion.ToString()
				};

				lock(_syncLock) { 
					FileUtility.WriteAllText(_filePath, JsonUtility.Serialize(cacheObj));
				}
			});
		}

        /// <summary>
        /// Clear all data from temporary & permanent storage (from memory and disk)
        /// </summary>
		public static void Clear()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("Clearing cache.");

				ApplicationMetadata = null;
				_cachedDevices.Clear();
				_globalListeners.Clear();
				_activeAlertsCount = 0;
				_deviceOrderIds.Clear();
				_commandProgresses.Clear(); 

				Save(); 
			});
		}

        /// <summary>
        /// Enforce correct ordering of devices.
        /// </summary>
        /// <param name="deviceIds">List of device ids to order.</param>
		public static void SetDeviceOrder(List<string> deviceIds)
		{
			if (PreserveDeviceOrder && deviceIds != null)
			{
				ExceptionUtility.Try(() =>
				{
					List<string> orderIds = new List<string>();

					foreach (var id in deviceIds)
						orderIds.Add(id);

					_deviceOrderIds = orderIds;
				}); 
			}
		}

        /// <summary>
        /// Triggers an update event to be broadcast for the given device.
        /// </summary>
        /// <param name="deviceId">The device for which to trigger the event</param>
		public static void TriggerDeviceUpdate(string deviceId)
		{
			ExceptionUtility.Try(() =>
			{
				var value = GetFromCacheInternal(deviceId);
				if (value != null)
				{
					foreach (var callback in value.UpdateCallbacks)
					{
						LogUtility.LogMessage("Calling callback event for device " + deviceId);
						callback(value.Device);
					}

					CallGlobalCallbacks();
				}
			});
		}

        /// <summary>
        /// Sets the active alert count.
        /// </summary>
        /// <param name="count">The value to set</param>
		public static void SetActiveAlertsCount(int? count)
		{
			if (count.HasValue)
			{
				if (_activeAlertsCount != count.Value)
				{
					_activeAlertsCount = count.Value;
					NotificationUtility.PostNotification(NotificationType.AlertsCountChanged, null);
				}
			}
		}

        /// <summary>
        /// Records a command progress update received. 
        /// </summary>
        /// <param name="item">The CommandProgress item</param>
        /// <param name="save">If true, caches the item in permanent storage</param>
		public static void AddCommandProgress(CommandProgress item, bool save = true)
		{
			ExceptionUtility.Try(() =>
			{
				if (item != null && item.ProgressResponse != null)
				{
					if (item.CommandId != null && item.CommandId != "-100")
					{
                        //if it's the final one, we can clear the history 
						if (item.Status.IsFinal)
						{
							ClearCommandProgress(item.CommandId, save);
						}
						else {
                            //add or update existing 
							if (!_commandProgresses.ContainsKey(item.CommandId))
							{
								_commandProgresses.AddOrUpdate(item.CommandId, (arg) => item, (arg1, arg2) => item);

                                //save 
								if (save)
									DataCache.Save();
							}
						}
					}
				}
			}); 
		}

        /// <summary>
        /// Clears the list of recorded updates for the specified command.
        /// </summary>
        /// <param name="commandId">Id of the command in question</param>
        /// <param name="save">If true, saves to permanent storage</param>
		public static void ClearCommandProgress(string commandId = null, bool save = true)
		{
			ExceptionUtility.Try(() =>
			{
				if (commandId != null)
				{
					CommandProgress value;
					if (_commandProgresses.ContainsKey(commandId))
					{
						_commandProgresses.TryRemove(commandId, out value);

						if (save)
							DataCache.Save();
					}
				}
				else {
					_commandProgresses.Clear();

					if (save)
						DataCache.Save();
				}
			});
		}

        /// <summary>
        /// Syncs the list of command progresses in memory, with those received from the server.
        /// </summary>
        /// <param name="items">List of command progresses received from server.</param>
		public static void SyncCommandProgresses(IEnumerable<CommandProgress> items)
		{
			ExceptionUtility.Try(() =>
			{
				if (items == null)
					items = new List<CommandProgress>();

                //add any missing ones 
				foreach (var item in items)
				{
					var cachedItem = _commandProgresses[item.CommandId];
					if (cachedItem != null)
					{
						AddCommandProgress(item, save: false);
					}
				}

                //remove any that shouldn't be there
				var commandIds = items.Select((i) => i.CommandId);
				List<string> commandIdsToRemove = new List<string>();
				foreach (var item in _commandProgresses.Values)
				{
					if (!commandIds.Contains(item.CommandId))
						commandIdsToRemove.Add(item.CommandId);
				}

				foreach (var id in commandIdsToRemove)
				{
					CommandProgress value;
					_commandProgresses.TryRemove(id, out value);
				}
			});
		}

        /// <summary>
        /// Gets the list of commands pending for a particular device.
        /// </summary>
        /// <param name="type">Type of command we want to see</param>
        /// <param name="deviceId">Id of the device for which we want pending commands</param>
        /// <returns>List of CommandProgress items</returns>
		public static IEnumerable<CommandProgress> GetCommandProgresses(CommandType type, string deviceId)
		{
			return ExceptionUtility.Try<IEnumerable<CommandProgress>>(() =>
			{
				return _commandProgresses.Values.Where((arg1, arg2) => arg1.DeviceId == deviceId && arg1.CommandType == type); 
			});
		}

        /// <summary>
        /// Gets any and all sub items for pending commands for the given device & type.
        /// </summary>
        /// <param name="type">Type of command we want to see</param>
        /// <param name="deviceId">Id of the device for which we want pending commands</param>
        /// <returns>List of unique sub ids for all found commands</returns>
		public static IEnumerable<string> GetCommandProgressesSubIds(CommandType type, string deviceId)
		{
			return ExceptionUtility.Try<IEnumerable<string>>(() =>
			{
				var pending = _commandProgresses.Values.Where((arg1, arg2) => arg1.DeviceId == deviceId && arg1.CommandType == type);
				List<string> mergedOutput = new List<string>(); 
				if (pending != null && pending.Any())
				{
					foreach (var command in pending)
					{
						if (command.SubItemIds != null)
						{
							foreach (string itemId in command.SubItemIds)
							{
								if (!mergedOutput.Contains(itemId))
									mergedOutput.Add(itemId); 
							}
						}
					}
				}

				return mergedOutput;
			});
		}

        /// <summary>
        /// Returns a value indicating whether or not the given device has any pending commands.
        /// </summary>
        /// <param name="type">The type of command we're interested in.</param>
        /// <param name="deviceId">The id of the device that we care about</param>
        /// <returns>Boolean value; true if pending commands found for given device</returns>
		public static bool HasPendingCommands(CommandType type, string deviceId)
		{
			return ExceptionUtility.Try<bool>(() => { 
				var pending = GetCommandProgresses(type, deviceId);
					if (pending != null)
					{
						return pending.Any();
					}
					return false;
			});
		}


		private static void AddOrUpdateInternal(Device device)
		{
			ExceptionUtility.Try(() =>
			{
				if (device != null)
				{
					if (device != null && !String.IsNullOrEmpty(device.Id))
					{
						if (device.Type == null)
							device.Type = TryGetDeviceType(device.Id);
						
						device.MergeFromMetadata();
						var newItem = new DeviceCacheItem(device);
						_cachedDevices.AddOrUpdate(device.Id, (k) =>
						{
							LogUtility.LogMessage("Device " + device.Id + " added to cache.");
							return newItem;
						},
						(k, v) =>
						{
							if (v.Device != null)
							{
								//then merge new data into old data 
								v.Device.MergeFromParent(newItem.Device, false, false);
								LogUtility.LogMessage("Device " + device.Id + " updated in cache.");
							}
							else
							{
								v.Device = newItem.Device;
								LogUtility.LogMessage("Device " + device.Id + " added to cache.");
							}

							foreach (var callback in v.UpdateCallbacks)
							{
								LogUtility.LogMessage("Calling callback event for device " + v.Device.Id);
								callback(v.Device);
							}
							return v;
						});

                        LimitAlerts();
					}
				}
			});
		}

		private static string TryGetDeviceType(string id)
		{
			string output = String.Empty;
			if (_cachedDevices.ContainsKey(id))
				output = _cachedDevices[id].Device?.Type;

			return output; 
		}

		private static void CallGlobalCallbacks()
		{
			foreach (var callback in _globalListeners.ToList())
				callback(GetAllDevicesFromCache());
		}

		private static DeviceCacheItem GetFromCacheInternal(string id)
		{
			return ExceptionUtility.Try<DeviceCacheItem>(() =>
			{
				DeviceCacheItem output = null;

				if (id != null)
				{
					_cachedDevices.TryGetValue(id, out output);
				}

				return output;
			});
		}

		private static List<Device> OrderDevices(List<Device> devices)
		{
			return ExceptionUtility.Try<List<Device>>(() =>
			{
				List<Device> orderedOutput = new List<Device>(); 

				for (int n = 0; n < _deviceOrderIds.Count; n++)
				{
					string id = _deviceOrderIds[n];
					var device = devices.Where((d) => d.Id == id).FirstOrDefault();
					if (device != null)
					{
						devices.Remove(device);
						orderedOutput.Add(device); 
					}
				}

				while (devices.Count > 0)
				{
					orderedOutput.Add(devices[0]);
					devices.RemoveAt(0); 
				}

				return orderedOutput;
			});
		}

        private static void LimitAlerts()
        {
            ExceptionUtility.Try(() =>
            {
                var alerts = GetGlobalAlerts(); 

                //trim down the number of alerts
                if (alerts.Count() > AppSettings.AlertsMaxCount)
                {
                    var alertsList = alerts.ToList(); 
                    while(alertsList.Count > AppSettings.AlertsMaxCount)
                    {
                        int index = alertsList.Count - 1; 
                        var alert = alertsList[index];
                        var device = GetDeviceFromCache(alert.DeviceId); 

                        if (device != null)
                        {
                            device.Alerts.Remove(alert.Id);
                            device.UpdateAlertCounts();
                        }

                        alertsList.RemoveAt(index);
                    }

                    //count the number of active alerts 
                    UpdateGlobalActiveAlertsCount();
                }
            });
        }

        private static void UpdateGlobalActiveAlertsCount()
        {
            var alerts = GetGlobalAlerts();
            int alertCount = 0;
            foreach (var alert in alerts)
            {
                if (alert.Active)
                {
                    alertCount++;
                }
            }

            SetActiveAlertsCount(alertCount);
        }


		private class DeviceCacheItem
		{
			public Device Device { get; set;}
			public string Id { get { return this.Device.Id; } }
			public DateTime? CacheTimestamp { get; set;}
			public IList<Action<Device>> UpdateCallbacks { get; private set;}

			public DeviceCacheItem(Device device)
			{
				this.UpdateCallbacks = new List<Action<Device>>();
				this.Device = device;
				this.CacheTimestamp = DateTime.Now;
			}

			public void AddUpdateCallback(Action<Device> callback)
			{
				if (!this.UpdateCallbacks.Contains(callback))
					this.UpdateCallbacks.Add(callback);
			}

			public void RemoveUpdateCallback(Action<Device> callback)
			{
				this.UpdateCallbacks.Remove(callback);
			}
		}

		[DataContract]
		private class JsonCacheObject
		{
			[DataMember(Name = PropertyNames.Devices)]
			public List<Device> Devices { get; set;}

			[DataMember(Name = PropertyNames.MetaData)]
			public ApplicationMetadata Metadata{ get; set; }

			[DataMember(Name = PropertyNames.Commands)]
			public List<CommandProgress> Commands { get; set; }

            [DataMember(Name = PropertyNames.ServerVersion)]
            public string ServerVersion { get; set; }
		}
	}
}
