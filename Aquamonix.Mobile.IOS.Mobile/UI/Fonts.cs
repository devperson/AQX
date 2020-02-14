using System;

using UIKit;

namespace Aquamonix.Mobile.IOS.UI
{
    /// <summary>
    /// UI Utility for standard app fonts.
    /// </summary>
	public static class Fonts
	{
		public const string RegularFontName = "SFUIText-Regular";
		public const string BoldFontName = "SFUIText-Bold"; 
		public const string SemiboldFontName = "SFUIText-Semibold"; 

		public static UIFont NavHeaderFont = UIFont.FromName(SemiboldFontName, Sizes.FontSize8);
		public static UIFont SummaryHeaderFont = UIFont.FromName(BoldFontName, Sizes.FontSize5);
	}
}
 
