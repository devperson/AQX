using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class DeviceViewModel
	{
		public Device Device { get; private set;}
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string FriendlyTypeName { get; private set; }
		public string Description { get; private set; }
		public string Type { get; private set; }
		public bool IsGroup
		{
			//HARDCODED
			get { return this.Type == "RGROUP";}
		}
		public bool IsUpdatingStatus { get; private set; }
		public SeverityLevel DisplaySeverityLevel { get; private set; }
		public IEnumerable<DeviceBadgeViewModel> Badges { get; private set; }
		public IEnumerable<Device> DevicesInGroup { get; private set;}

		public DeviceViewModel()
		{
		}

		public DeviceViewModel(Device device)
		{
			this.Device = device;

			this.Device.ProcessRemoveLists(); 

			//device.MergeFromApplicationMetadata(DataCache.ApplicationMetadata, device.Type);

			this.Id = device.Id;
			this.Name = device.Name;
			this.Type = device.Type;
			this.FriendlyTypeName = DataCache.ApplicationMetadata?.GetFriendlyNameForDeviceType(this.Type);
			this.IsUpdatingStatus = device.IsUpdatingStatus.GetValueOrDefault();
                        
			if (String.IsNullOrEmpty(this.Name))
				this.Name = this.Id;

			if (this.IsGroup)
			{
				this.Description = device.Badges != null && device.Badges.Items != null && device.Badges.Items.Any() ? device.Badges?.Items?.First().Value?.Texts?.FirstOrDefault() : String.Empty;
			}
			else
				this.Description = String.Format("{0} {1}", this.FriendlyTypeName, device.Number);

			string distance;
			if (this.GetDistanceString(out distance))
				this.Description += ", " + distance;

			this.Badges = new List<DeviceBadgeViewModel>();
			var badges = this.Badges as List<DeviceBadgeViewModel>; 
			if (device.Badges != null)
			{
				foreach (var value in device.Badges)
				{
					var badge = new DeviceBadgeViewModel()
					{
						Type = value.Value.Type, 
						Text = value.Value.Texts?.LastOrDefault()
					};
					SeverityLevel severity;
					if (Enum.TryParse<SeverityLevel>(value.Value.Severity, out severity))
						badge.SeverityLevel = severity;

					badges.Add(badge);
				}
			}

			// calculate severity level from data 
			this.DisplaySeverityLevel = SeverityLevel.Missing;
  			if (device.Status != null)
			{
				int severity;
				if (Int32.TryParse(device.Status.Severity, out severity))
				{
					var severityLevel = DataCache.ApplicationMetadata?.GetSeverityLevel(severity);
					if (severityLevel != null)
						this.DisplaySeverityLevel = severityLevel.Value;
				}
			}
		}

		private bool GetDistanceString(out string distanceString)
		{
			bool output = false;
			distanceString = String.Empty;

			try
			{
				if (this.Device != null)
				{
					if (this.Device.Location != null)
					{
						if (this.Device.Location.Latitude.GetValueOrDefault() != 0 && 
						    this.Device.Location.Longitude.GetValueOrDefault() != 0)
						{
							output = true;
							double meters = LocationUtility.DistanceInMetersFrom(this.Device.Location.Latitude.Value, this.Device.Location.Longitude.Value);
							if (meters < 0)
							{
								distanceString = String.Empty;
								output = false;
							}
							else {
								if (meters > 1000)
								{
									double km = (meters / 1000);
									distanceString = km.ToString("#,###.#") + " km away";
								}
								else {
									distanceString = Convert.ToInt32(meters).ToString() + " meters away";
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return output; 
		}
	}

	public enum DeviceStatus
	{
		Online,
		Offline,
		Yellow,
		Changing
  	}
}
