using System;
using System.Threading;

using BigTed;

using UIKit;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Utilities
{
    /// <summary>
    /// IOS-specific implementation of IProgressUtility that uses BTProgressHud.
    /// </summary>
	public class BTProgressUtility: IProgressUtility
	{
		private volatile static bool _isProgressVisible;
		private static object progressLocker = new object();

		public void Show(string progressMessage, bool blockUI = true)
		{
			LogUtility.LogMessage("Progress Message: " + progressMessage);

			if (!_isProgressVisible)
			{
				lock (progressLocker)
				{
					if (!_isProgressVisible)
					{
						_isProgressVisible = true;

						MainThreadUtility.Instance.BeginInvokeOnMainThread(() =>
						{
							ShowInternal(progressMessage, blockUI);
						});
					}
				}
			}
		}

		public void SafeShow(string progressMessage, Action activity, bool blockUI = true)
		{
			try
			{
				this.Show(progressMessage, blockUI);

				if (activity != null)
					activity();
			}
			catch (Exception e)
			{
				ProgressUtility.Dismiss();
				LogUtility.LogException(e);
			}
		}

		public T SafeShow<T>(string progressMessage, Func<T> activity, bool blockUI = true)
		{
			try
			{
				this.Show(progressMessage, blockUI);

				if (activity != null)
					return activity(); 
			}
			catch (Exception e)
			{
				ProgressUtility.Dismiss();
				LogUtility.LogException(e);
			}
        	return default(T); 
		}

		public void Dismiss()
		{
			if (_isProgressVisible)
			{
				lock (progressLocker)
				{
					if (_isProgressVisible)
					{
						_isProgressVisible = false;
						MainThreadUtility.Instance.BeginInvokeOnMainThread(BTProgressHUD.Dismiss);
					}
				}
			}
		}

		private void ShowInternal(string progressMessage, bool blockUI)
		{
#if !DEBUG
			progressMessage = null;
#endif

			var maskType = (blockUI ? ProgressHUD.MaskType.Black : ProgressHUD.MaskType.None);
			BTProgressHUD.Show(progressMessage, -1, maskType);
		}
	}
}
