using System;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	//HARDCODED
	public class ResponseType
	{
		public const string Connection = "System.ResponseConnection";
		public const string Devices = "Resp.Devices";
		public const string Device = "Resp.Device";
		public const string Alerts = "Resp.Alerts";
		public const string Progress = "Resp.Progress";
		public const string SystemError = "System.Error";
		public const string TimeoutError = "System.TimeoutError";
       
        public bool IsDevicesResponse(string responseType)
		{
			return (responseType == Devices || responseType == Device);
		}
	}
}
