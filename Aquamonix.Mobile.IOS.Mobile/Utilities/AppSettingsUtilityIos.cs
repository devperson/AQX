using System;
using System.IO;
using System.Collections.Generic;

using Foundation;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Utilities
{
	public class AppSettingsUtilityIos : Aquamonix.Mobile.Lib.Utilities.IAppSettingsUtility
	{
		private static readonly IDictionary<string, string> AppSettingsDictionary = new Dictionary<string, string>();
		private static readonly object LoadLock = new object();
		private static bool IsLoaded { get; set; }

		public TValueType GetAppSettingValue<TValueType>(string appSettingsName, TValueType defaultValue)
		{
			EnsureLoaded();
			string stringAppSettingsValue;
			var found = AppSettingsDictionary.TryGetValue(appSettingsName, out stringAppSettingsValue);
			if (!found)
			{
				return defaultValue;
			}
			var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TValueType));
			var appSettingsValue = (TValueType)typeConverter.ConvertFrom(stringAppSettingsValue);
			return appSettingsValue;
		}

		private static void EnsureLoaded()
		{
			if (!IsLoaded)
			{
				try
				{
					lock (LoadLock)
					{
						if (!IsLoaded)
						{
							Load();
						}
					}
				}
				finally
				{
					IsLoaded = true;
				}
			}
		}

		private static void Load()
		{
			ExceptionUtility.Try(() =>
			{
				//	MonoTouch.ObjCRuntime.Class.ThrowOnInitFailure = false;
				if (!File.Exists("info.plist"))
					return;

				var appSettingsNsDictionary = new NSDictionary("info.plist");
				var keys = appSettingsNsDictionary.Keys;
				AppSettingsDictionary.Clear();
				foreach (var keyNsObject in keys)
				{
					var key = keyNsObject.ToString();
					NSObject valueNsObject;
					var found = appSettingsNsDictionary.TryGetValue(keyNsObject, out valueNsObject);
					if (!found)
					{
						continue;
					}
					var stringValue = valueNsObject.ToString();
					AppSettingsDictionary[key] = stringValue;
				}
			});
		}
	}
}
