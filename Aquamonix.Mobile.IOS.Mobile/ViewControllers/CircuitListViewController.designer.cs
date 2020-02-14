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

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("CircuitsListViewController")]
	partial class CircuitListViewController
	{
		private static readonly FontWithColor BoldCellTextFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize8, Colors.AquamonixBrown);
		private static readonly FontWithColor NormalCellTextFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor NavHeaderFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, UIColor.White);
		private static readonly FontWithColor NavHeaderBoldFont = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, UIColor.White);
		private static readonly FontWithColor StatusLabelFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize3, Colors.StandardTextColor);

		private readonly NavBarView _navBarView = new NavBarView();
		private readonly CircuitListTableViewController _tableViewController = new CircuitListTableViewController();
		private readonly PrevStopNextFooter _navFooterView = new PrevStopNextFooter(PrevStopNextFooter.StopNextPrevType.Circuits);
		private readonly SummaryHeader _summaryView = new SummaryHeader();
		private readonly SelectionHeader _selectionHeaderView = new SelectionHeader();
		private readonly SelectionButtonFooter _buttonFooterView = new SelectionButtonFooter();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this._selectionHeaderView.Hidden = true;
			this._buttonFooterView.Hidden = true;

			this.PrimeView.AddSubviews(
				_tableViewController.TableView,
				_navFooterView,
				_summaryView,
				_selectionHeaderView,
				_buttonFooterView
			);
		}
	}
}
