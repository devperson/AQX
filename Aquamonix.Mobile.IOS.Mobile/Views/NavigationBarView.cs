using System;

using UIKit;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Gray navigation bar at very top of each screen.
    /// </summary>
	public class NavigationBarView : AquamonixView
    {
		public const int Height = 45;

		public NavigationBarView(bool fullWidth = false) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.SetFrameHeight(Height);

				if (fullWidth)
				{
					this.SetFrameWidth(UIScreen.MainScreen.Bounds.Width);
					this.SetFrameLocation(0, 0);
				}
				else
				{
					this.SetFrameWidth(UIScreen.MainScreen.Bounds.Width - 120);
					this.SetFrameLocation(60, 0);
				}
			});
		}
	}
}
