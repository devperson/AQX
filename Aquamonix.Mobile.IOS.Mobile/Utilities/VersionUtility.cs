using System;

using Foundation;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Utilities 
{
    /// <summary>
    /// Utility for retreiving this application's version & build numbers.
    /// </summary>
	public static class VersionUtility
	{
		public static string Version
		{
			get
			{
				return ExceptionUtility.Try<string>(() =>
				{
					return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")].ToString();
				});
			}
		}

		public static string BuildNumber
		{
			get
			{
				return ExceptionUtility.Try<string>(() =>
				{
					return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();
				});
			}
		}

		public static string GetVersionString()
		{
#if RELEASE_BUILD
			return String.Format("Aquamonix v {0}", VersionUtility.Version); 
#else
			return String.Format("Aquamonix v {0}.{1}", VersionUtility.Version, VersionUtility.BuildNumber);
#endif
		}
	}
}
