using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;
using Foundation;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Device alerts screen.
    /// </summary>
    public partial class AlertListViewController : ListViewControllerBase
    { 
        private static AlertListViewController _instance;
        public static string _deviceId;
        List<AlertViewModel> searchResults = new List<AlertViewModel>();
        private static List<AlertViewModel> _alerts = new List<AlertViewModel>();
        protected static string _result = null;

        public List<AlertViewModel> Alerts { get { return _alerts; } }

        protected bool DismissEnabled
        {
            get { return (DataCache.ServerVersion >= Mobile.Lib.Environment.AppSettings.AlertsDismissMinVersion); }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get {
                var output = (_searchBoxView.IsDisposed) ? 0 : _searchBoxView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            }
        }

        private AlertListViewController(string deviceId) : base(deviceId)
        {
        }

        public static AlertListViewController CreateInstance(string deviceId)
        {
            _deviceId = deviceId;
            ExceptionUtility.Try(() =>
            {
                if (_instance != null && _instance.DeviceId != deviceId)
                {
                    _instance.Dispose();
                    _instance = null;

                }
            });
     if (_instance == null)
                _instance = new AlertListViewController(deviceId);
            return _instance;
        }


        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                this.ShowHideDismissAllButton();
               
                if (this._searchBoxView.Text.Length > 0)
                    this.DoSearch(this._searchBoxView.Text);
                else
                {
                    _tableViewController.LoadData(this.Device.Alerts.OrderBy((a) => a.Date).Reverse());
                    this.ShowHideDismissAllButton();
                    this.HandleViewDidLayoutSubviews();
                }

                this._navBarView.SetTitleAndIcon(this.Device);
                this.SetDismissAllButton();
            });
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();

            this._searchBoxView.SetFrameHeight(SearchBox.Height);
        }

        protected override void WillNavigateAway(UIKit.UIViewController destination)
        {
            this._searchBoxView.DismissKeyboard();
        }

        protected override void InitializeViewController()
        {
            ExceptionUtility.Try(() =>
            {
                base.InitializeViewController();

                this.NavigationBarView = this._navBarView;
                this._searchBoxView.OnTextChanged = DoSearch;
                this.SetCustomBackButton();

                // TODO need to only set this if user has AlertsAccessLevel > 1 for this device
                

                this.HeaderView = this._searchBoxView;
                this.TableViewController = this._tableViewController;
            });
        }

        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = StringLiterals.AlertsEmptyMessage;
            bool isEmpty = true;
            if (this.Device != null)
            {
                if (this.Device.Alerts != null)
                    isEmpty = !this.Device.Alerts.Any();
            }

            return isEmpty;
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


        private bool HasActiveAlerts()
        {
            return ExceptionUtility.Try<bool>(() => {
                bool output = false;
                if (this.Alerts != null)
                {
                    foreach(var alert in this.Alerts)
                    {
                        if (alert.Active)
                        {
                            output = true;
                            break;
                        }
                    }
                }
                return output;
            });
        }

        private int GetActiveAlertsCount()
        {
            return ExceptionUtility.Try<int>(() => {
                int output = 0;
                if (this.Alerts != null)
                {
                    foreach (var alert in this.Alerts)
                    {
                        if (alert.Active)
                        {
                            output++;
                        }
                    }
                }
                return output;
            });
        }

        private IEnumerable<string> GetActiveAlertIds()
        {
            return ExceptionUtility.Try<List<string>>(() => {
                List<string> output = new List<string>();
                if (this.Alerts != null)
                {
                    foreach (var alert in this.Alerts)
                    {
                        if (alert.Active)
                        {
                            output.Add(alert.Id);
                        }
                        //if (output.Count > 1)
                        //    break;
                    }
                }
                return output;
            });
        }

        private IEnumerable<string> GetActiveAlertIdsFromSearchList()
        {
            return ExceptionUtility.Try<List<string>>(() => {
                List<string> output = new List<string>();
                if (this.searchResults != null)
                {
                    foreach (var alert in this.searchResults)
                    {
                        if (alert.Active)
                        {
                            output.Add(alert.Id);
                        }
                        //if (output.Count > 1)
                        //    break;
                    }
                }
                return output;
            });
        }

        private void SetDismissAllButton()
        {
            ExceptionUtility.Try(() =>
            {
                if (HasAccessLevelToDelete())
                {
                    if (this.NavigationItem != null)
                    {
                        this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(StringLiterals.DismissAll, UIBarButtonItemStyle.Bordered, (o, e) =>
                        {
                            if (this.NavigationController != null)
                            {
                            //  this.NavigationController.PopViewController(false);

                            ExceptionUtility.Try(() =>
                                {
                                    string title = StringLiterals.Alert;
                                    AlertUtility.ShowConfirmationAlert(title, StringLiterals.DismissAllAlertsConfirmationMessage(), (b) =>
                                    {
                                        if (b)
                                        {
                                            DismissAlertsConfirmed();
                                        }
                                    }, StringLiterals.YesButtonText);
                                });
                            }
                        });
                    }
                }
            });
        }

        private void ShowHideDismissAllButton()
        {
            ExceptionUtility.Try(() => 
            {
                if (this.DismissEnabled)
                {
                    if (this.NavigationItem?.RightBarButtonItem != null)
                    {
                        this.NavigationItem.RightBarButtonItem.Enabled = this.HasActiveAlerts();
                    }
                }
            });
        }

        //added
        private bool HasAccessLevelToDelete()
        {
            return ExceptionUtility.Try<bool>(() => {
                bool output = false;
                foreach (var devices in User.Current.DevicesAccess.Items.ToList())
                {
                    var keys = devices.Key;
                    var AccessLevel = devices.Value.AccessLevel;
                    var AlertsAccessLevel = devices.Value.AlertsAccessLevel;
                    if (_deviceId == devices.Key && Convert.ToInt64(AlertsAccessLevel) > 1)
                    {
                        output = true;
                        break;
                    }
                }
                return output;
            });
        }

        private void DismissAlertsConfirmed()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.HasAccessLevelToDelete())
                {
                    if (String.IsNullOrEmpty(_searchBoxView.Text))
                    {
                        var activeAlertIds = this.GetActiveAlertIds();

                        if (activeAlertIds.Any())
                        {
                            ServiceContainer.AlertService.DismissDeviceAlerts(this.DeviceId, activeAlertIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
                             {
                                 MainThreadUtility.InvokeOnMain(() =>
                                 {
                                     ProgressUtility.Dismiss();
                                 });
                             });
                        } 
                    }
                    else
                    {
                        var activeAlertIds = this.GetActiveAlertIdsFromSearchList();

                        if (activeAlertIds.Any())
                        {
                            ServiceContainer.AlertService.DismissDeviceAlerts(this.DeviceId, activeAlertIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
                            {
                                MainThreadUtility.InvokeOnMain(() =>
                                {
                                    ProgressUtility.Dismiss();
                                });
                            });
                        }
                    }
                }
            });
        }

        private void DoSearch(string searchString)
        {
            ExceptionUtility.Try(() =>
            {
                searchString = searchString.Trim().ToLower();
                searchResults = new List<AlertViewModel>();

                if (!String.IsNullOrEmpty(searchString))
                {
                    //starts with
                    var startsWith = this.Device.Alerts.Where((d) =>
                                                        (d.DisplayText != null && d.DisplayText.ToLower().StartsWith(searchString)) ||
                                                        (d.DeviceName != null && d.DeviceName.ToLower().StartsWith(searchString))
                                                       ).ToList();

                    //contains 
                    var contains = this.Device.Alerts.Where((d) =>
                                                      (d.DisplayText != null && d.DisplayText.ToLower().Contains(searchString)) ||
                                                      (d.DeviceName != null && d.DeviceName.ToLower().Contains(searchString))
                                                     ).ToList();

                    searchResults.AddRange(startsWith);

                    foreach (var alert in contains)
                    {
                        if (!searchResults.Where((r) => r.Id == alert.Id).Any())
                            searchResults.Add(alert);
                    }
                }
                else
                    searchResults = this.Device.Alerts.ToList();

                _tableViewController.LoadData(searchResults.OrderBy((a) => a.Date).Reverse());
            });
        }

        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImageGray = UIImage.FromFile("Images/alarms1.png");
            private static UIImage _iconImageRed = UIImage.FromFile("Images/alarms1_active.png");

            public NavBarView() : base() { }

            public void SetTitleAndIcon(DeviceDetailViewModel device)
            {
                UIImage icon = _iconImageGray;
                foreach (var alert in device.Alerts)
                {
					if (alert.ShowRed)
                    {
                        icon = _iconImageRed;
                        break;
                    }
                }

                this.SetTitleAndImage(device.Name, icon);
            }
        }

        private class AlertListTableViewController : TopLevelTableViewControllerBase<AlertViewModel, AlertListTableViewSource>
        {
            public AlertListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(AlertListTableViewCell), AlertListTableViewCell.TableCellKey);
            }

            protected override AlertListTableViewSource CreateTableSource(IList<AlertViewModel> values)
            {
                return new AlertListTableViewSource(values);
            }
        }

        private class AlertListTableViewSource : TableViewSourceBase<AlertViewModel>
        {
            public AlertListTableViewSource(IList<AlertViewModel> alerts) : base(alerts) { }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                AlertListTableViewCell cell = (AlertListTableViewCell)tableView.DequeueReusableCell(AlertListTableViewCell.TableCellKey, indexPath);
                AlertViewModel alert = null;
                _alerts = Values.ToList();
                if (indexPath.Row < Values.Count)
                    alert = Values[indexPath.Row];
                

                //create cell style
                if (alert != null)
                {
                    _result = alert.Id;
                    cell.LoadCellValues(alert);
                }

                return cell;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 72;
            }
            //added
            #region Swipe Delete
            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                try
                {
                    bool output = false;
                    var item = _alerts[indexPath.Row];
                    var devicename = item.DeviceName;
                    if (item.Active)
                    {
                        foreach (var devices in User.Current.DevicesAccess.Items.ToList())
                        {
                            var keys = devices.Key;
                            var AccessLevel = devices.Value.AccessLevel;
                            var AlertsAccessLevel = devices.Value.AlertsAccessLevel;
                            if (devicename == devices.Key && Convert.ToInt64(AlertsAccessLevel) > 1)
                            {
                                output = true;
                                break;
                            }
                        }
                    }
                    return output;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
            {

                UITableViewRowAction moreButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Dismiss", delegate {
                    var alertItem = _alerts.ElementAt(indexPath.Row);
                    DismissSingleAlert(alertItem);
                });
                moreButton.BackgroundColor = UIColor.FromRGB(17, 125, 184);
                return new UITableViewRowAction[] { moreButton };
            }
            #endregion
            //added
            private void DismissSingleAlert(AlertViewModel alertItem)
            {
                var alerts = new List<string>();
                alerts.Add(alertItem.Id);
                try
                {
                    ServiceContainer.AlertService.DismissDeviceAlerts(alertItem.DeviceId, alerts, onReconnect: () => { DismissSingleAlert(alertItem);}).ContinueWith((r) =>
                    {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            ProgressUtility.Dismiss();
                        });
                    });
                }
                catch (Exception ex)
                {

                }
            }
            

        }

        private class AlertListTableViewCell : TableViewCellBase
        {
            public const string TableCellKey = "AlertListTableViewCell";

            private const int LeftMargin = 8;
            private const int RightMargin = 12;
            private const int LeftTextMargin = 28;
            private const int TopMargin = 15;
            private const int BlueCircleSize = 12;

            private readonly AquamonixLabel _dateTimeLabel = new AquamonixLabel();
            private readonly AquamonixLabel _descriptionLabel = new AquamonixLabel();
            private readonly UIImageView _blueCircleImageView = new UIImageView();

            private static readonly UIImage _blueCircleImage = GraphicsUtility.CreateColoredRect(
                Colors.BlueButtonText,
                new CoreGraphics.CGSize()
                {
                    Height = BlueCircleSize,
                    Width = BlueCircleSize
                });

            public AlertListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    //label 
                    _dateTimeLabel.SetFontAndColor(DateTimeFont);

                    //blue circle
                    _blueCircleImageView.Image = _blueCircleImage;
                    _blueCircleImageView.SetFrameSize(BlueCircleSize, BlueCircleSize);
                    _blueCircleImageView.SetFrameLocation(LeftMargin, TopMargin + 1);
                    _blueCircleImageView.MakeRoundedCorners(UIRectCorner.AllCorners, BlueCircleSize / 2);

                    this.ContentView.AddSubviews(_dateTimeLabel, _descriptionLabel, _blueCircleImageView);
                });
            }

            public void LoadCellValues(AlertViewModel alert)
            {
                ExceptionUtility.Try(() =>
                {
                    _dateTimeLabel.Text = alert.DisplayDate;
                    _dateTimeLabel.SizeToFit();

                    if (alert.Active)
                        _descriptionLabel.SetFontAndColor(AlertTextBoldFont);
                    else
                        _descriptionLabel.SetFontAndColor(AlertTextNormalFont);

                    if (alert.ShowRed)
                        _descriptionLabel.TextColor = Colors.ErrorTextColor;
                    else
                        _descriptionLabel.TextColor = Colors.StandardTextColor;

                    _descriptionLabel.Text = alert.DisplayText;
                    _descriptionLabel.SizeToFit();

                    _blueCircleImageView.Hidden = (!alert.Active);
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                this._dateTimeLabel.SetFrameHeight(14);
                this._dateTimeLabel.SetFrameLocation(LeftTextMargin, TopMargin);
                this._dateTimeLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);

                this._descriptionLabel.SetFrameLocation(LeftTextMargin, _dateTimeLabel.Frame.Bottom + 5);
                this._descriptionLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
            }
        }
    }
}
