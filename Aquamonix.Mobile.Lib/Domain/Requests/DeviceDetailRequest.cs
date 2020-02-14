using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{

    [DataContract]
    public class DeviceDetailRequest : ApiRequestForDetail<DeviceDetailsRequestBody>
    {
        public const string RequestKey = "Req.Devices";
        public DeviceDetailRequest(string username, string password, string appVersion = "1", string timestamp = null, string sessionId = null) : base(RequestKey, sessionId)
        {
            this.Body = new DeviceDetailsRequestBody()
            {
                //UserName = username,
                //Password = password,
                //AppVersion = appVersion
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
    public class DeviceDetailsRequestBody
    {

        [DataMember(Name = PropertyNames.MetaData)]
        public RequestMetadata MetaData { get; set; }

        public DeviceDetailsRequestBody()
        {
        }
    }
}
