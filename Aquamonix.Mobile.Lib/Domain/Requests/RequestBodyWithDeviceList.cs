using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public abstract class RequestBodyWithDeviceList
	{
		[DataMember(Name = PropertyNames.Devices)]
		public ItemsDictionary<DeviceRequest> Devices { get; set; }

		public RequestBodyWithDeviceList(IEnumerable<DeviceRequest> devices)
		{
			this.Devices = new ItemsDictionary<DeviceRequest>();
			foreach (var d in devices)
			{
				this.Devices.Add(d.Id, d);
			}
		}

		public RequestBodyWithDeviceList(IEnumerable<string> deviceIds)
		{
			this.Devices = new ItemsDictionary<DeviceRequest>();
			foreach (var id in deviceIds)
			{
				if (id != null)
					this.Devices.Add(id, new DeviceRequest());
			}
   		}
	}

	[DataContract]
	public class DeviceRequest
	{
		[IgnoreDataMember]
		public string Id { get; set; }

		public bool PassEmptyCircuitsList
		{
			set
			{
				if (value)
					this.Circuits = new ItemsDictionary<EmptyRequestObject>(); 
			}
		}

		[DataMember(Name = PropertyNames.Pumps)]
		public ItemsDictionary<EmptyRequestObject> Pumps { get; private set; }

		[DataMember(Name = PropertyNames.Stations)]
		public ItemsDictionary<EmptyRequestObject> Stations { get; private set; }

		[DataMember(Name = PropertyNames.Programs)]
		public ItemsDictionary<EmptyRequestObject> Programs { get; private set; }
        
        /*Edited for Pivot*/
        [DataMember(Name = PropertyNames.Programs)]
        public ItemsDictionary<EmptyRequestObject> PivotPrograms { get; private set; }

        [DataMember(Name = PropertyNames.Circuits)]
		public ItemsDictionary<EmptyRequestObject> Circuits { get; private set; }

		[DataMember(Name = PropertyNames.StatusGroups)]
		public ItemsDictionary<StatusGroup> StatusGroups { get; private set; }

		[DataMember(Name = PropertyNames.Settings)]
		public ItemsDictionary<DeviceSetting> Settings { get; private set; }

		[DataMember(Name = PropertyNames.MetaData)]
		public DeviceMetaDataRequest MetaData { get; set; }

		public DeviceRequest()
		{
			this.MetaData = new DeviceMetaDataRequest() { };
		}

		public DeviceRequest(string deviceId) : this()
		{
			this.Id = deviceId;
		}

		public DeviceRequest(Device d) : this()
		{
			if (d != null)
			{
				this.Id = d.Id;

				//if (d.StatusGroups != null)
				//	this.StatusGroups = d.StatusGroups;

				//if (d.Settings != null)
				//	this.Settings = d.Settings;

				if (Environment.AppSettings.UseTimestamps)
				{
					if (d.MetaData?.TimeStamp != null)
						this.MetaData = new DeviceMetaDataRequest() { TimeStamp = d.MetaData.TimeStamp };
				}
			}
		}

		public void AddProgram(string programId)
		{
			if (this.Programs == null)
				this.Programs = new ItemsDictionary<EmptyRequestObject>();

			this.Programs.Add(programId, new EmptyRequestObject()); 
		}

		public void AddPrograms(IEnumerable<string> programIds)
		{
			foreach (string programId in programIds)
				this.AddProgram(programId);
		}
        /*Edited for Pivot*/
        public void AddPivotPrograms(IEnumerable<string> programIds)
        {
            foreach (string programId in programIds)
                this.AddProgram(programId);
        }

        public void AddStation(string stationId)
		{
			if (this.Stations == null)
				this.Stations = new ItemsDictionary<EmptyRequestObject>();

			this.Stations.Add(stationId, new EmptyRequestObject());
		}

		public void AddStations(IEnumerable<string> stationIds)
		{
			foreach (string stationId in stationIds)
				this.AddStation(stationId);
		}

		public void AddPump(string pumpId)
		{
			if (this.Pumps == null)
				this.Pumps = new ItemsDictionary<EmptyRequestObject>();

			this.Pumps.Add(pumpId, new EmptyRequestObject());
		}

		public void AddPumps(IEnumerable<string> pumpIds)
		{
			foreach (string pumpId in pumpIds)
				this.AddPump(pumpId);
		}

		public void AddCircuit(string circuitId)
		{
			if (this.Circuits == null)
				this.Circuits = new ItemsDictionary<EmptyRequestObject>();

			this.Circuits.Add(circuitId, new EmptyRequestObject());
		}

		public void AddCircuits(IEnumerable<string> circuitIds)
		{
			foreach (string circuitId in circuitIds)
				this.AddCircuit(circuitId);
		}

		public void AddSetting(DeviceSetting setting)
		{
			if (this.Settings == null)
				this.Settings = new ItemsDictionary<DeviceSetting>();

			if (setting != null && setting.Id != null)
				this.Settings.Add(setting.Id, setting);
		}

		public void AddStatusGroup(StatusGroup statusGroup)
		{
			if (this.StatusGroups == null)
				this.StatusGroups = new ItemsDictionary<StatusGroup>();

			if (statusGroup != null && statusGroup.Id != null)
				this.StatusGroups.Add(statusGroup.Id, statusGroup);
		}
	}

	[DataContract]
	public class DeviceMetaDataRequest
	{
		[DataMember(Name = PropertyNames.MetaDataTimeStamp)]
		public int? TimeStamp { get; set; }
    }
}
