using System;
using System.IO;

using ServiceStack.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for serializing/deserializing JSON.
    /// </summary>
	public interface IJsonUtility
	{
        /// <summary>
        /// Serialize object to JSON string.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON string</returns>
		string Serialize(object obj);

        /// <summary>
        /// Deserialize JSON string to object.
        /// </summary>
        /// <param name="json">String to deserialize</param>
        /// <returns>An object of type T</returns>
		T Deserialize<T>(string json);

        /// <summary>
        /// Deserialize JSON string to an object of the given type.
        /// </summary>
        /// <param name="type">Type of object to deserialize</param>
        /// <param name="json">String to deserialize</param>
        /// <returns>An object of the given type</returns>
		object Deserialize(Type type, string json);
	}

    /// <summary>
    /// Static helper class for serializing/deserializing JSON.
    /// </summary>
	public static class JsonUtility
	{
		public static string Serialize(object obj)
		{
			if (Providers.JsonUtility != null)
				return Providers.JsonUtility.Serialize(obj);

			return null; 
		}


		public static T Deserialize<T>(string json)
		{
			if (Providers.JsonUtility != null)
				return Providers.JsonUtility.Deserialize<T>(json);

			return default(T);
		}

		public static object Deserialize(Type type, string json)
		{
			if (Providers.JsonUtility != null)
				return Providers.JsonUtility.Deserialize(type, json);

			return null;
		}

		public static T OpenFromFile<T>(string fileName) where T : class
		{
			var fPath = Path.Combine(FileUtility.GetCachesDirectory(), fileName);

			if (!FileUtility.FileExists(fPath))
				return null;

			var json = FileUtility.ReadAllText(fPath);

			return Deserialize<T>(json);
		}

		public static void SaveToFile(object t, string fileName)
		{
			var fPath = Path.Combine(FileUtility.GetCachesDirectory(), fileName);
			FileUtility.WriteAllText(fPath, Serialize(t));
		}
	}
}

