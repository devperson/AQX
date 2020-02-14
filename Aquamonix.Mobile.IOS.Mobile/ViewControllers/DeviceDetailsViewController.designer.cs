// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("DeviceDetailsViewController")]
	partial class DeviceDetailsViewController
	{
		private static readonly FontWithColor MainTextNormal = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor MainTextBoldRed = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize8, Colors.ErrorTextColor);
		private static readonly FontWithColor ByLineText = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize5, Colors.StandardTextColor);
		private static readonly FontWithColor NavHeaderFontBig = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize6, Colors.StandardTextColor);
		private static readonly FontWithColor NavHeaderFontSmall = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize2, Colors.StandardTextColor);

		private readonly NavBarView _navBarView = new NavBarView();
		private readonly DeviceDetailsTableViewController _tableViewController = new DeviceDetailsTableViewController();
		private readonly PrevStopNextFooter _navFooterView = new PrevStopNextFooter();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();
          
            this.PrimeView.AddSubviews(_tableViewController.TableView, _navFooterView);
   		}
	}
}
