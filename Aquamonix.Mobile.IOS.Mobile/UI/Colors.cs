using System;

using UIKit;

namespace Aquamonix.Mobile.IOS.UI
{
    /// <summary>
    /// UI utility related to standard app colors.
    /// </summary>
	public class Colors
	{
		public static readonly UIColor GhostGrayBackground = Colors.FromHex(0xf1f3f5);
		//TODO: this color is not quite right 
		public static readonly UIColor LightGrayBackground = Colors.FromHex(0xd3d8dc);
		public static readonly UIColor DividerLineColor = Colors.FromHex(0xd3d8dc);
		public static readonly UIColor AquamonixGreen = Colors.FromHex(0x5da328); // UIColor.FromRGBA(119, 188, 67, 255);
		public static readonly UIColor AquamonixBrown = Colors.FromHex(0x847132); 
		public static readonly UIColor StandardTextColor = Colors.FromHex(0x58595d);
		public static readonly UIColor GreenTextColor = Colors.FromHex(0x539924);
		public static readonly UIColor OrangeTextColor = Colors.FromHex(0xf49c20);
		public static readonly UIColor LightGrayTextColor = Colors.FromHex(0x9b9b9b);
		public static readonly UIColor MediumGrayTextColor = Colors.FromHex(0x8396ad);
		public static readonly UIColor ErrorTextColor = Colors.FromHex(0xff0700);
		public static readonly UIColor BlueButtonText = Colors.FromHex(0x007cba);
        public static readonly UIColor DismissBGColor = Colors.FromHex(0x117DB8);

        public static UIColor FromHex(int hexValue)
		{
			return UIColor.FromRGBA(
				(nfloat)(((hexValue & 0xFF0000) >> 16) / 255.0),
				(nfloat)(((hexValue & 0x00FF00) >> 8) / 255.0),
				(nfloat)(((hexValue & 0x0000FF) >> 0) / 255.0),
				(nfloat)1
			); 
		}
	}
}
