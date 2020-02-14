using System;

using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Static helper class for displaying UI progress spinners to the user.
    /// </summary>
	public static class ProgressUtility
	{
		public static void Show(string progressMessage, bool blockUI = true)
		{
			Providers.ProgressUtility.Show(progressMessage, blockUI);
		}

		public static void SafeShow(string progressMessage, Action activity, bool blockUI = true)
		{
			Providers.ProgressUtility.SafeShow(progressMessage, activity, blockUI);
		}

		public static T SafeShow<T>(string progressMessage, Func<T> activity, bool blockUI = true)
		{
			return Providers.ProgressUtility.SafeShow(progressMessage, activity, blockUI); 
		}

		public static void Dismiss()
		{
			Providers.ProgressUtility.Dismiss();
		}
    }
}
