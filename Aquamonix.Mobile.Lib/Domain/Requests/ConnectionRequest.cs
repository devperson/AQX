using System;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class ConnectionRequest : ApiRequest<ConnectionRequestBody>
	{
		public const string RequestKey = "System.RequestConnection"; 

		public ConnectionRequest(string username, string password, string appVersion = "1", string timestamp=null, string sessionId = null) : base(RequestKey, sessionId)
		{
			this.Body = new ConnectionRequestBody()
			{
				UserName = username,
				Password = password,
				AppVersion = appVersion
			};

			this.Header.SessionId = sessionId;

			if (Environment.AppSettings.UseTimestamps)
			{
				if (timestamp != null)
					this.Body.MetaData = new RequestMetadata() { TimeStamp = timestamp };
			}
		}
	}

	[DataContract]
	public class ConnectionRequestBody
	{
		[DataMember(Name=PropertyNames.UserName)]
		public string UserName { get; set; }

		[DataMember(Name = PropertyNames.Password)]
		public string Password { get; set; }

		[DataMember(Name = PropertyNames.AppVersion)]
		public string AppVersion { get; set; }

		[DataMember(Name = PropertyNames.MetaData)]
		public RequestMetadata MetaData { get; set; }

		public ConnectionRequestBody()
		{
		}
	}

	[DataContract]
	public class RequestMetadata
	{
		[DataMember(Name = PropertyNames.MetaDataTimeStamp)]
		public string TimeStamp { get; set;}
	}
}
