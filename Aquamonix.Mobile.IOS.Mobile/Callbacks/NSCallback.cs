using System;
using Foundation;

namespace Aquamonix.Mobile.IOS.Callbacks
{
	public class NSCallback : NSObject
	{
		public Action Callback { get; set; }

		public NSCallback(Action callback)
		{
			Callback = callback;
		}
	}
}

