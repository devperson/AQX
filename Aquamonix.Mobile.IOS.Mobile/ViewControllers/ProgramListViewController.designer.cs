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
	[Register("ProgramListViewController")]
	partial class ProgramListViewController
	{
		private static readonly FontWithColor CellLine1StandardFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor CellLine1GreenFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize8, Colors.AquamonixGreen);
		private static readonly UIFont StartStopButtonFont = UIFont.FromName(Fonts.BoldFontName, Sizes.FontSize3); 
		                                                                     
		private readonly NavBarView _navBarView = new NavBarView();
		private readonly ProgramListTableViewController _tableViewController = new ProgramListTableViewController();
		private readonly PrevStopNextFooter _navFooterView = new PrevStopNextFooter();
		private readonly SummaryHeader _summaryView = new SummaryHeader();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();
             
			this.PrimeView.AddSubviews(
				_tableViewController.TableView, 
				_navFooterView, 
				_summaryView
			);
		}
	}
}
