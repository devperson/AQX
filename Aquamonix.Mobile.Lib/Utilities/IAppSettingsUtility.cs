using System;
using System.Collections.Generic;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for reading application settings from wherever they are stored.
    /// </summary>
	public interface IAppSettingsUtility
	{
		TValueType GetAppSettingValue<TValueType>(string appSettingsName, TValueType defaultValue);
	}
}
