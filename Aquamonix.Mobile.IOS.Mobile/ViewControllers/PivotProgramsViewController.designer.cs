// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    [Register("PivotProgramsViewController")]
    partial class PivotProgramsViewController
    {
        private static readonly FontWithColor CellLine1StandardFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
        private static readonly FontWithColor CellLine1GreenFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize8, Colors.AquamonixGreen);
        private static readonly UIFont StartStopButtonFont = UIFont.FromName(Fonts.BoldFontName, Sizes.FontSize3);

       private readonly NavBarView _navBarView = new NavBarView();
       // private readonly UILabel _navBarView = new UILabel();
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
                //_intervalPickerView,
                _summaryView
            );
        }
    }
}