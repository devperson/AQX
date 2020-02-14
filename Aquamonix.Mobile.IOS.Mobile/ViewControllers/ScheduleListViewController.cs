using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Schedules screen.
    /// </summary>
	public partial class ScheduleListViewController : ListViewControllerBase
    {
        private static ScheduleListViewController _instance;

        private bool ShowNavFooter
        {
            get 
            {
                return (Device.SupportsCircuitsStop || Device.SupportsCircuitsPrev || Device.SupportsCircuitsNext);
            }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get
            {
                //return _tableViewController.TableView.Frame.Top; 
                var output = (_summaryView.IsDisposed) ? 0 : _summaryView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            }
        }

        private ScheduleListViewController(string deviceId) : base(deviceId)
        {
        }

        public static ScheduleListViewController CreateInstance(string deviceId)
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }
            });

            if (_instance == null)
                _instance = new ScheduleListViewController(deviceId);

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();

                this._tableViewController.LoadData(this.Device.Schedules);
                this.HandleViewDidLayoutSubviews();
                this._navBarView.SetTitle(this.Device.Name);
                this._summaryView.SetMessages(this.Device?.SchedulesFeature?.HeaderSummaryTexts);
                this._navFooterView.Device = this.Device;

                this.HandleViewDidLayoutSubviews();
            });
        }

        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = StringLiterals.SchedulesEmptyMessage;
            bool isEmpty = true;
            if (this.Device != null)
            {
                if (this.Device.Schedules != null)
                    isEmpty = !this.Device.Schedules.Any();
            }

            return isEmpty;
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();

            _summaryView.SizeToFit();
        }

        protected override void InitializeViewController()
        {
            ExceptionUtility.Try(() =>
            {
                base.InitializeViewController();

                this._navFooterView.Hidden = (!this.ShowNavFooter);

                this.NavigationBarView = this._navBarView;
                this.TableViewController = this._tableViewController;
                this.HeaderView = this._summaryView;
                this.SetCustomBackButton();

                if (this.ShowNavFooter)
                    this.FooterView = this._navFooterView;
            });
        }

        protected override void AfterShowReconBar()
        {
            base.AfterShowReconBar();
            this.AdjustTableForReconBar(_tableViewController, true);
        }

        protected override void AfterHideReconBar()
        {
            base.AfterHideReconBar();
            this.AdjustTableForReconBar(_tableViewController, false);
        }


        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImage = UIImage.FromFile("Images/schedule.png");

            public NavBarView() : base() { }

            public void SetTitle(string title)
            {
                this.SetTitleAndImage(title, _iconImage);
            }
        }

        private class ScheduleListTableViewController : TopLevelTableViewControllerBase<ScheduleViewModel, ScheduleListTableViewSource>
        {
            public ScheduleListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(ScheduleListTableViewCell), ScheduleListTableViewCell.TableCellKey);
            }

            protected override ScheduleListTableViewSource CreateTableSource(IList<ScheduleViewModel> values)
            {
                return new ScheduleListTableViewSource(values);
            }
        }

        private class ScheduleListTableViewSource : TableViewSourceBase<ScheduleViewModel>
        {
            public ScheduleListTableViewSource(IList<ScheduleViewModel> schedules) : base(schedules) { }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                ScheduleListTableViewCell cell = (ScheduleListTableViewCell)tableView.DequeueReusableCell(ScheduleListTableViewCell.TableCellKey, indexPath);

                ScheduleViewModel schedule = null;
                if (indexPath.Row < Values.Count)
                    schedule = Values[indexPath.Row];

                //create cell style
                if (schedule != null)
                {
                    cell.LoadCellValues(schedule);
                }

                return cell;
            }
        }

        private class ScheduleListTableViewCell : TableViewCellBase
        {
            public const string TableCellKey = "ScheduleListTableViewCell";

            private const int LeftMargin = 17;
            private const int RightMargin = 17;

            private readonly AquamonixLabel _scheduleNameLabel = new AquamonixLabel();
            private readonly AquamonixLabel _scheduleDescriptionLabel = new AquamonixLabel();
            private readonly AquamonixLabel _circuitsLabel = new AquamonixLabel();
            private readonly AquamonixLabel _offLabel = new AquamonixLabel();

            public ScheduleListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    _scheduleNameLabel.SetFontAndColor(ScheduleNameFont);

                    _scheduleDescriptionLabel.SetFontAndColor(ScheduleDescriptionFont);

                    _circuitsLabel.SetFontAndColor(CircuitsFont);

                    _offLabel.SetFontAndColor(OffFont);
                    _offLabel.Text = StringLiterals.Off;
                    _offLabel.SizeToFit();
                    _offLabel.Hidden = true;

                    this.ContentView.AddSubviews(_scheduleNameLabel, _scheduleDescriptionLabel, _circuitsLabel, _offLabel);
                });
            }

            public void LoadCellValues(ScheduleViewModel schedule)
            {
                ExceptionUtility.Try(() =>
                {
                    _scheduleNameLabel.Text = schedule.Name;
                    _scheduleNameLabel.SizeToFit();

                    _scheduleDescriptionLabel.Text = schedule.Description;
                    _scheduleDescriptionLabel.SizeToFit();

                    _circuitsLabel.Text = schedule.Circuits;
                    _circuitsLabel.SizeToFit();

                    _scheduleDescriptionLabel.Hidden = !schedule.On;
                    _circuitsLabel.Hidden = !schedule.On;
                    _offLabel.Hidden = schedule.On;
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                this._scheduleNameLabel.SetFrameHeight(20);
                this._scheduleNameLabel.SetFrameLocation(LeftMargin, this.ContentView.Frame.Height / 2 - _scheduleNameLabel.Frame.Height / 2);
                this._scheduleNameLabel.EnforceMaxXCoordinate(this.ContentView.Frame.Width / 2);

                this._scheduleDescriptionLabel.SetFrameHeight(20);
                this._scheduleDescriptionLabel.EnforceMaxWidth(this.Frame.Width - _scheduleNameLabel.Frame.Right - RightMargin);
                this._scheduleDescriptionLabel.SetFrameLocation(this.ContentView.Frame.Width - _scheduleDescriptionLabel.Frame.Width - RightMargin, 12);

                this._circuitsLabel.SetFrameHeight(19);
                this._circuitsLabel.EnforceMaxWidth(this.Frame.Width - _scheduleNameLabel.Frame.Right - RightMargin);
                this._circuitsLabel.SetFrameLocation(this.ContentView.Frame.Width - _circuitsLabel.Frame.Width - RightMargin, 32);

                this._offLabel.SetFrameHeight(20);
                this._offLabel.EnforceMaxWidth(this.Frame.Width - _scheduleNameLabel.Frame.Right - RightMargin);
                this._offLabel.SetFrameLocation(
                    this.ContentView.Frame.Width - this._offLabel.Frame.Width - RightMargin,
                    this._scheduleNameLabel.Frame.Y
                );
            }
        }
    }
}

