using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS
{
    /// <summary>
    /// Extension methods for UILabel
    /// </summary>
	public static class UILabelExtensions
	{
		public static void SetFontAndColor(this UILabel label, FontWithColor fontWithColor)
		{
			label.Font = fontWithColor.Font;
			label.TextColor = fontWithColor.Color;
		}
	}
}
