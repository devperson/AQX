using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ServiceStack.Text;
using Newtonsoft.Json;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class RequestResponseHeader
	{
		[DataMember(Name=PropertyNames.Type)]
		public string Type { get; private set; }

		[DataMember(Name = PropertyNames.Channel)]
		public string Channel { get; set; }

		[DataMember(Name = PropertyNames.SessionId)]
		public string SessionId { get; set; }

        [DataMember(Name = PropertyNames.EndPointVersion)]
        public string ServerVersion { get; set; }

        public RequestResponseHeader(string type, string channel, string sessionId)
		{
			if (String.IsNullOrEmpty(channel))
				channel = String.Empty;
			
			this.Type = type;
			this.Channel = channel;
			this.SessionId = sessionId;
		}

		public RequestResponseHeader(string type, long channel, string sessionId) : this(type, channel.ToString(), sessionId) { }
	}
}
