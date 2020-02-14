using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS
{
    /// <summary>
    /// Extension methods for UIButton.
    /// </summary>
	public static class UIButtonExtensions
	{
		public static void SetFontAndColor(this UIButton button, FontWithColor fontWithColor)
		{
			button.Font = fontWithColor.Font;
			button.SetTitleColor(fontWithColor.Color, UIControlState.Normal);
			button.SetTitleColor(fontWithColor.Color, UIControlState.Disabled);
		}
	}
}
