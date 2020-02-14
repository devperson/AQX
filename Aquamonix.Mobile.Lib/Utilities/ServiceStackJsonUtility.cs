using System;
using System.IO;

using ServiceStack.Text;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Implementation of IJsonUtility that uses ServiceStack to serialize/deserialize JSON (faster than Newtonsoft)
    /// </summary>
	public class ServiceStackJsonUtility : IJsonUtility
	{
		public ServiceStackJsonUtility()
		{
			//ServiceStack.Licensing.RegisterLicense(@"2439-e1JlZjoyNDM5LE5hbWU6RE9PUjMsVHlwZTpCdXNpbmVzcyxIYXNoOkh4aXNtTkNvakhWTUlKRzVZVHVqNUNkdS9rb3RRbEpVbUFCdEJBbU5DMzhqMU15R0RxUTVtMzlQcFRua09pM0k2Zk9ocWZ3TTNwM1AwcUx1a0lHM1ZoVHdxeXdlYzB2ejdGMFE4N1dCVWJ4MSszZURWSEFMRVUrZkdDU21ZR3cyalRMY0prUXFMRkk1bjMzRkw3c2N5Wmx0N0UrRGVobDhlR1k0Zm1ybGpxVT0sRXhwaXJ5OjIwMTYtMDMtMjV9");

			JsConfig.IncludePublicFields = true;
			JsConfig.EmitCamelCaseNames = true;
		}

		public string Serialize(object obj)
		{
            try
            {
                return JsonSerializer.SerializeToString(obj);
            }
            catch(Exception)
            {
                return null;
            }
		}


		public T Deserialize<T>(string json)
		{
			return JsonSerializer.DeserializeFromString<T>(json);
		}

		public object Deserialize(Type type, string json)
		{
			return JsonSerializer.DeserializeFromString(json, type);
		}
	}
}
