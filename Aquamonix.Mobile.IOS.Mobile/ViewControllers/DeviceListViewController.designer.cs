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

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("DeviceListViewController")]
	partial class DeviceListViewController
	{
		private readonly static FontWithColor DeviceNameFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize12, Colors.StandardTextColor); 
		private readonly static FontWithColor DeviceDescriptionFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize7, Colors.StandardTextColor); 
		private readonly static FontWithColor NavHeaderFirstLineFont = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize6, Colors.StandardTextColor);
		private readonly static FontWithColor NavHeaderSecondLineFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize2, Colors.StandardTextColor);
		private readonly static FontWithColor BadgeFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize3, Colors.StandardTextColor);

		private readonly NavBarView _navBarView = new NavBarView();
		private readonly SearchBox _searchBoxView = new SearchBox();
		private readonly DeviceListTableViewController _tableViewController = new DeviceListTableViewController();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this.ShowTabBar = true;
            this.ShowConnectionBar = false;
			this.PrimeView.AddSubviews(_searchBoxView, _tableViewController.TableView);
		}
	}
}
