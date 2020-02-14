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
	[Register("ScheduleListViewController")]
	partial class ScheduleListViewController
	{
		private static readonly FontWithColor ScheduleNameFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor ScheduleDescriptionFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor CircuitsFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize4, Colors.StandardTextColor);
		private static readonly FontWithColor OffFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.LightGrayTextColor);
		//private static readonly FontWithColor OnFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);

		private readonly NavBarView _navBarView = new NavBarView();
		private readonly ScheduleListTableViewController _tableViewController = new ScheduleListTableViewController();
		private readonly SummaryHeader _summaryView = new SummaryHeader();
		private readonly PrevStopNextFooter _navFooterView = new PrevStopNextFooter(PrevStopNextFooter.StopNextPrevType.Circuits);

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this.PrimeView.AddSubviews(
				_tableViewController.TableView,
				_summaryView,
				_navFooterView
			); 
		}
	}
}
