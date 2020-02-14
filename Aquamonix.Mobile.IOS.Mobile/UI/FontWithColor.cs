using System;

using UIKit;

namespace Aquamonix.Mobile.IOS.UI
{
    /// <summary>
    /// UI Utility to pair fonts with font colors
    /// </summary>
	public class FontWithColor
	{
		public UIFont Font { get; private set;}

		public UIColor Color { get; private set;}

		public FontWithColor(UIFont font, UIColor color)
		{
			this.Font = font;
			this.Color = color;
		}

		public FontWithColor(UIFont font, int hexValue)
		{
			this.Font = font;
			this.Color = Colors.FromHex(hexValue);
		}

		public FontWithColor(string fontName, int fontSize, UIColor color)
		{
			this.Font = UIFont.FromName(fontName, fontSize);
			this.Color = color;
		}

		public FontWithColor(string fontName, int fontSize, int hexValue)
		{
			this.Font = UIFont.FromName(fontName, fontSize);
			this.Color = Colors.FromHex(hexValue);
		}
	}
}
