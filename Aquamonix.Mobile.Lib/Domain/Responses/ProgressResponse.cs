using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class ProgressResponse : ApiResponse<ProgressResponseBody>
	{
		public override bool IsFinal
		{
			get
			{
				bool output = base.IsFinal;

				if (output)
				{
					if (this.Body != null && !this.Body.Status.IsFinal)
						output = false;

					if (this.HasError)
						output = true;
				}

				return output; 
			}
		}

		public override bool IsSuccessful
		{
			get
			{
				var output = base.IsSuccessful;
				if (output)
				{
					if (this.Body != null && this.Body.Progress != null && this.Body.Progress == "StoppedFailed")
						output = false;
				}

				return output; 
			}
		}

		public string CommandId
		{
			get { return this?.Body?.CommandId; }
		}

		public ProgressResponse() : base()
		{
		}
	}

	[DataContract]
	public class ProgressResponseBody : ResponseBodyBase
	{
		[DataMember(Name=PropertyNames.Progress)]
		public string Progress { get; set; }

		[DataMember(Name=PropertyNames.ProgressDescription)]
		public string ProgressDescription { get; set; }

		[DataMember(Name = PropertyNames.ProgressSpecific)]
		public int? ProgressSpecific { get; set; }

		//[DataMember(Name = "CommandId")]
		//public string CommandId { get; set; }

		[DataMember(Name = PropertyNames.DeviceId)]
		public string DeviceId { get; set;}

		[DataMember(Name = PropertyNames.Devices)]
		public ItemsDictionary<Device> Devices { get; set; }

		[IgnoreDataMember]
		public ProgressResponseStatus Status
		{
			get { return new ProgressResponseStatus(this.Progress);}
		}

		public override void ReadyResponse()
		{
			base.ReadyResponse();

			this.Devices?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Devices);
		}
	}
}
