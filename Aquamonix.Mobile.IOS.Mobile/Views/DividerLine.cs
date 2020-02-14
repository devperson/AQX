using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Horizontal divider line.
    /// </summary>
	public class DividerLine : AquamonixView
    {
		public DividerLine() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = Colors.DividerLineColor;
			});
		}

		public void SetSize(int height = 1)
		{
			ExceptionUtility.Try(() =>
			{
                if (this.Superview != null)
                {
                    this.SetFrameSize(this.Superview.Frame.Width, height);
                }
			});
		}
	}
}
