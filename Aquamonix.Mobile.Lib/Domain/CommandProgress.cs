using System;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Domain.Responses; 

namespace Aquamonix.Mobile.Lib.Domain
{
	public class CommandProgress
	{
		public CommandProgress() { }

		public CommandProgress(CommandType commandType, ProgressResponseBody progressResponse, string[] subItemIds = null) 
		{
			this.CommandType = commandType;

			if (progressResponse != null)
			{
				this.ProgressResponse = progressResponse;

				this.CommandId = progressResponse.CommandId;
				this.Progress = progressResponse.Progress;
				this.ProgressSpecific = progressResponse.ProgressSpecific;
				this.ProgressDescription = progressResponse.ProgressDescription;
				this.DeviceId = progressResponse.DeviceId;
				this.SubItemIds = subItemIds;
			}
		}

		[IgnoreDataMember]
		public ProgressResponseBody ProgressResponse { get; set; }

		[DataMember]
		public string CommandId { get; set; }

		[DataMember(Name = PropertyNames.DeviceId)]
		public string DeviceId { get; set; }

		[DataMember]
		public string[] SubItemIds { get; set;}

		[DataMember(Name = PropertyNames.Progress)]
		public string Progress { get; set; }

		[DataMember(Name = PropertyNames.ProgressDescription)]
		public string ProgressDescription { get; set; }

		[DataMember(Name = PropertyNames.ProgressSpecific)]
		public int? ProgressSpecific { get; set; }

		[DataMember]
		public CommandType CommandType { get; set;}

		[IgnoreDataMember]
		public ProgressResponseStatus Status
		{
			get { return new ProgressResponseStatus(this.Progress); }
		}
	}

	public enum CommandType
	{
		DeviceNext,
		DevicePrev,
		DeviceStop,
		StartPrograms,
		StartStations, 
		TestStations,
		StartCircuits,
		StopCircuits,
		DeviceFeatureSetting,
		DisablePrograms,
		SetProgramsPercent,
		UpdateSensors
	}
}
