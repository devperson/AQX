// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using CoreGraphics;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("GlobalAlertListViewController")]
	partial class GlobalAlertListViewController
	{
		private static readonly FontWithColor AlertTextNormalLine1Font = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize9, Colors.StandardTextColor);
		private static readonly FontWithColor AlertTextBoldLine1Font = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize9, Colors.StandardTextColor);

		private static readonly FontWithColor AlertTextNormalLine2Font = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize7, Colors.StandardTextColor);
		private static readonly FontWithColor AlertTextBoldLine2Font = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize7, Colors.StandardTextColor);

		private static readonly FontWithColor DateTimeFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize3, Colors.MediumGrayTextColor);

		private readonly NavBarView _navBarView = new NavBarView();
		private readonly SearchBox _searchBoxView = new SearchBox();
		private readonly GlobalAlertListTableViewController _tableViewController = new GlobalAlertListTableViewController();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this.PrimeView.AddSubviews(_tableViewController.TableView, _searchBoxView);
		}
	}
}
