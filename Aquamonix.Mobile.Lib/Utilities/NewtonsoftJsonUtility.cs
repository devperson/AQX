using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Implementation of IJsonUtility that uses Newtonsoft to serialize/deserialize JSON.
    /// </summary>
	public class NewtonsoftJsonUtility : IJsonUtility
	{
		public NewtonsoftJsonUtility()
		{
			Newtonsoft.Json.JsonConvert.DefaultSettings = new System.Func<Newtonsoft.Json.JsonSerializerSettings>(() =>
				new Newtonsoft.Json.JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = NullValueHandling.Ignore
				}
			);
		}

		public string Serialize(object obj)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}


		public T Deserialize<T>(string json)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
		}

		public object Deserialize(Type type, string json)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
		}
	}
}

