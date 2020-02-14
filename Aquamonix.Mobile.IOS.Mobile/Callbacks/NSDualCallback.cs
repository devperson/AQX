using System;
using Foundation;

namespace Aquamonix.Mobile.IOS.Callbacks
{
	public class NSDualCallback : NSObject
	{
		public NSCallback OnSuccess { get; set; }

		public NSCallback OnFailure { get; set; }
	}
}

