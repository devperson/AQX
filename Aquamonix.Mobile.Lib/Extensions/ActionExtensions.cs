using System;
using System.Timers;

namespace Aquamonix.Mobile.Lib.Extensions
{
    /// <summary>
    /// Extension methods for System.Action. 
    /// </summary>
	public static class ActionExtensions
	{
		public static void RunAfter(this Action action, double span)
		{
			var dispatcherTimer = new Timer { Interval = span };
			dispatcherTimer.Elapsed += (sender, args) =>
			{
				var timer = sender as Timer;
				if (timer != null)
				{
					timer.Stop();
					timer.Dispose(); 
				}

				action();
			};
			dispatcherTimer.Start();
		}
	}
}
