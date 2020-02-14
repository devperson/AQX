using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Global alerts screen.
    /// </summary>
	public partial class GlobalAlertListViewController : ListViewControllerBase
    {
        private static GlobalAlertListViewController _instance;
        List<AlertViewModel> searchResults = new List<AlertViewModel>();
        private List<AlertViewModel> _alerts = null;

        public List<AlertViewModel> Alerts { get { return _alerts; } }

        protected bool DismissEnabled
        {
            get { return (DataCache.ServerVersion >= Mobile.Lib.Environment.AppSettings.AlertsDismissMinVersion); }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get
            {
                var output = (_searchBoxView.IsDisposed) ? 0 : _searchBoxView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6 ;
                return output;
            }
        }

        private GlobalAlertListViewController() : base()
        {
            _alerts = DataCache.GetGlobalAlerts().ToList();

            this.Initialize();

            DataCache.SubscribeToAllDeviceUpdates(this.OnDeviceUpdate); 
        }

        public static GlobalAlertListViewController CreateInstance()
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }

                _instance = new GlobalAlertListViewController();
            });

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                this.SetDismissAllButton();
                this.ShowHideDismissAllButton();

                if (this._searchBoxView.Text.Length > 0)
                    this.DoSearch(this._searchBoxView.Text);
                else
                {
                    _tableViewController.LoadData(_alerts);
                    this.ShowHideDismissAllButton();
                    this.HandleViewDidLayoutSubviews();
                }

                this._navBarView.SetTitleAndIcon(_alerts);
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
                this.ShowTabBar = true;
                this.ShowConnectionBar = false;
                this.TabBar.SelectTab(0);

                // TODO need to only set this if user has AlertsAccessLevel > 1 for at least one device
                //this.SetDismissAllButton();

                this.HeaderView = this._searchBoxView;
                this.TableViewController = this._tableViewController;
               
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


        private void RefreshData()
        {
            ExceptionUtility.Try(() => {
                _alerts = DataCache.GetGlobalAlerts().ToList();
                this.DoLoadData();
            });
        }

        private bool HasActiveAlerts()
        {
            return ExceptionUtility.Try<bool>(() => {
                bool output = false;
                if (this.Alerts != null)
                {
                    foreach (var alert in this.Alerts)
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
        //added
        private bool HasAccessLevelToDelete()
        {
            return ExceptionUtility.Try<bool>(() => {
                bool output = false;
                if (_alerts.Count > 0)
                {
                    foreach (var item in User.Current.DevicesAccess.Items.ToList())
                    {
                        var keys = item.Key;
                        var AccessLevel = item.Value.AccessLevel;
                        var AlertsAccessLevel = item.Value.AlertsAccessLevel;
                        if (Convert.ToInt64(AlertsAccessLevel) > 1)
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

        private IEnumerable<AlertViewModel> GetActiveAlert()
        {
            return ExceptionUtility.Try<List<AlertViewModel>>(() => {
                List<AlertViewModel> output = new List<AlertViewModel>();
                if (this.Alerts != null)
                {
                    foreach (var alert in this.Alerts)
                    {
                        if (alert.Active)
                        {
                            output.Add(alert);
                        }
                        //if (output.Count > 1)
                        //    break;
                    }
                }
                return output;
            });
        }

        //added
        private IEnumerable<AlertViewModel> GetActiveAlertsFromSearchList()
        {
            return ExceptionUtility.Try<List<AlertViewModel>>(() => {
                List<AlertViewModel> output = new List<AlertViewModel>();
                if (this.searchResults != null)
                {
                    foreach (var alert in this.searchResults)
                    {
                        if (alert.Active)
                        {
                            output.Add(alert);
                        }
                        //if (output.Count > 1)
                        //    break;
                    }
                }
                return output;
            });
        }



        //edited
        private void SetDismissAllButton()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.HasAccessLevelToDelete())
                {
                    if (this.NavigationItem != null)
                    {
                        this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(StringLiterals.DismissAll, UIBarButtonItemStyle.Plain, (o, e) =>
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

        //edited//
        private void ShowHideDismissAllButton()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.DismissEnabled)
                {
                    if (this.NavigationItem?.RightBarButtonItem != null)
                    {
                        //this.NavigationItem.RightBarButtonItem.Enabled = this.HasActiveAlerts();
                        this.NavigationItem.RightBarButtonItem.Enabled = this.HasAccessLevelToDelete();
                    }
                }
            });
        }
        //edited 
        private void DismissAlertsConfirmed()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.HasAccessLevelToDelete())
                {
                    if (String.IsNullOrEmpty(_searchBoxView.Text))
                    {
                        //var activeAlertIds = this.GetActiveAlertIds();
                        var activeAlerts = GetActiveAlert();
                        //check all the active alers have correct access with device
                        var deletingAlertsIds = CheckAlertDeletePermission(activeAlerts);

                        if (deletingAlertsIds.Count < activeAlerts.Count())
                        {
                            AlertUtility.ShowConfirmationAlert(StringLiterals.Alert, StringLiterals.ActiveAlertsNotAccessForDissmiss(), (b) =>
                            {
                                if (b)
                                {
                                    ServiceContainer.AlertService.DismissGlobalAlerts(deletingAlertsIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
                                    {
                                        MainThreadUtility.InvokeOnMain(() =>
                                        {
                                            ProgressUtility.Dismiss();
                                        });
                                    });
                                }
                            }, StringLiterals.Dismiss);
                        }
                        else
                        {
                            ServiceContainer.AlertService.DismissGlobalAlerts(deletingAlertsIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
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
                        //var activeAlertIds = this.GetActiveAlertIds();
                        var activeAlerts = GetActiveAlertsFromSearchList();
                        //check all the active alers have correct access with device
                        var deletingAlertsIds = CheckAlertDeletePermission(activeAlerts);

                        if (deletingAlertsIds.Count < activeAlerts.Count())
                        {
                            AlertUtility.ShowConfirmationAlert(StringLiterals.Alert, StringLiterals.ActiveAlertsNotAccessForDissmiss(), (b) =>
                            {
                                if (b)
                                {
                                    ServiceContainer.AlertService.DismissGlobalAlerts(deletingAlertsIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
                                    {
                                        MainThreadUtility.InvokeOnMain(() =>
                                        {
                                            ProgressUtility.Dismiss();
                                        });
                                    });
                                }
                            }, StringLiterals.Dismiss);
                        }
                        else
                        {
                            ServiceContainer.AlertService.DismissGlobalAlerts(deletingAlertsIds, onReconnect: () => { DismissAlertsConfirmed(); }).ContinueWith((r) =>
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
        //added
        private List<string> CheckAlertDeletePermission(IEnumerable<AlertViewModel> activeAlerts)
        {
            return ExceptionUtility.Try<List<string>>(() => {
                List<string> output = new List<string>();
                foreach(var item in activeAlerts)
                {
                    foreach(var userdevice in User.Current.DevicesAccess.Items.ToList())
                    {
                        var keys = userdevice.Key;
                        var AccessLevel = userdevice.Value.AccessLevel;
                        var AlertsAccessLevel = userdevice.Value.AlertsAccessLevel;
                        if (item.DeviceName == keys && Convert.ToInt64(AlertsAccessLevel) > 1)
                        {
                            output.Add(item.Id);
                        }
                    }
                }
                return output;
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
                    var startsWith = this._alerts.Where((d) =>
                                                        (d.DisplayText != null && d.DisplayText.ToLower().StartsWith(searchString)) ||
                                                        (d.DeviceName != null && d.DeviceName.ToLower().StartsWith(searchString))
                                                       ).ToList();

                    //contains 
                    var contains = this._alerts.Where((d) =>
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
                    searchResults = this._alerts;

                _tableViewController.LoadData(searchResults.OrderBy((a) => a.Date).Reverse());
            });
        }

        private void OnDeviceUpdate(IEnumerable<Device> devices)
        {
            this.RefreshData(); 
        }


         
        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImageGray = UIImage.FromFile("Images/alarms1.png");
            private static UIImage _iconImageRed = UIImage.FromFile("Images/alarms1_active.png");

            public NavBarView() : base() { }

            public void SetTitleAndIcon(IEnumerable<AlertViewModel> alerts)
            {
                ExceptionUtility.Try(() =>
                {
                    UIImage icon = _iconImageGray;
                    if (alerts != null)
                    {
                        foreach (var alert in alerts)
                        {
                            if (alert.ShowRed)
                            {
                                icon = _iconImageRed;
                                break;
                            }
                        }
                    }

                    this.SetTitleAndImage("Alerts", icon);
                });
            }
        }

        private class GlobalAlertListTableViewController : TopLevelTableViewControllerBase<AlertViewModel, GlobalAlertListTableViewSource>
        {
            public GlobalAlertListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(GlobalAlertListTableViewCell), GlobalAlertListTableViewCell.TableCellKey);
            }

            protected override GlobalAlertListTableViewSource CreateTableSource(IList<AlertViewModel> values)
            {
                return new GlobalAlertListTableViewSource(values);
            }
        }

        private class GlobalAlertListTableViewSource : TableViewSourceBase<AlertViewModel>
        {
            
            public GlobalAlertListTableViewSource(IList<AlertViewModel> alerts) : base(alerts) { }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                
                GlobalAlertListTableViewCell cell = (GlobalAlertListTableViewCell)tableView.DequeueReusableCell(GlobalAlertListTableViewCell.TableCellKey, indexPath);

                AlertViewModel alert = null;
                if (indexPath.Row < Values.Count)
                    alert = Values[indexPath.Row];

                //create cell style
                if (alert != null)
                {
                    cell.LoadCellValues(alert);
                }

                return cell;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 95;
            }
            //added
            #region Swipe Delete
            //added
            private void DismissSingleAlert(AlertViewModel alertItem)
            {
                var alerts = new List<string>();
                alerts.Add(alertItem.Id);
                try
                {
                    ServiceContainer.AlertService.DismissGlobalAlerts(alerts, onReconnect: () => { DismissSingleAlert(alertItem); }).ContinueWith((r) =>
                    {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            ProgressUtility.Dismiss();
                        });
                    });
                }
                catch(Exception ex)
                {
                  
                }
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                
                return Values.Count;
            }

            public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
            {

                UITableViewRowAction deleteButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Dismiss", delegate {
                    var alertItem = Values.ElementAt(indexPath.Row);
                    DismissSingleAlert(alertItem);
                });
                deleteButton.BackgroundColor = UIColor.FromRGB(17, 125, 184);
                return new UITableViewRowAction[] { deleteButton };
            }
            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                try
                {
                    bool output = false;
                    var item = Values[indexPath.Row];
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
            #endregion
        }

        private class GlobalAlertListTableViewCell : TableViewCellBase
        {
            public const string TableCellKey = "GlobalAlertListTableViewSource";

            private const int LeftMargin = 8;
            private const int RightMargin = 12;
            private const int LeftTextMargin = 28;
            private const int TopMargin = 15;
            private const int BlueCircleSize = 12;

            private readonly AquamonixLabel _dateTimeLabel = new AquamonixLabel();
            private readonly AquamonixLabel _descriptionLine1Label = new AquamonixLabel();
            private readonly AquamonixLabel _descriptionLine2Label = new AquamonixLabel();
            private readonly UIImageView _blueCircleImageView = new UIImageView();

            private static readonly UIImage _blueCircleImage = GraphicsUtility.CreateColoredRect(
                Colors.BlueButtonText,
                new CoreGraphics.CGSize()
                {
                    Height = BlueCircleSize,
                    Width = BlueCircleSize
                });

            public GlobalAlertListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    //labels
                    _dateTimeLabel.SetFontAndColor(DateTimeFont);
                    _descriptionLine1Label.SetFontAndColor(AlertTextNormalLine1Font);
                    _descriptionLine2Label.SetFontAndColor(AlertTextNormalLine2Font);

                    //blue circle
                    _blueCircleImageView.Image = _blueCircleImage;
                    _blueCircleImageView.SetFrameSize(BlueCircleSize, BlueCircleSize);
                    _blueCircleImageView.SetFrameLocation(LeftMargin, TopMargin + 1);
                    _blueCircleImageView.MakeRoundedCorners(UIRectCorner.AllCorners, BlueCircleSize / 2);

                    this.ContentView.AddSubviews(_dateTimeLabel, _descriptionLine1Label, _descriptionLine2Label, _blueCircleImageView);
                });
            }

            public void LoadCellValues(AlertViewModel alert)
            {
                ExceptionUtility.Try(() =>
                {
                    //datetime label 
                    _dateTimeLabel.Text = alert.DisplayDate;
                    _dateTimeLabel.SizeToFit();

                    if (alert.Active)
                    {
                        _descriptionLine1Label.SetFontAndColor(AlertTextBoldLine1Font);
                        _descriptionLine2Label.SetFontAndColor(AlertTextBoldLine2Font);
                    }
                    else
                    {
                        _descriptionLine1Label.SetFontAndColor(AlertTextNormalLine1Font);
                        _descriptionLine2Label.SetFontAndColor(AlertTextNormalLine2Font);
                    }

                    if (alert.ShowRed)
                        _descriptionLine1Label.TextColor = Colors.ErrorTextColor;
                    else
                        _descriptionLine1Label.TextColor = Colors.StandardTextColor;

                    _descriptionLine1Label.Text = alert.DisplayText;
                    _descriptionLine1Label.SizeToFit();

                    _descriptionLine2Label.Text = String.Format("at {0}", alert.DeviceName);
                    _descriptionLine2Label.SizeToFit();

                    _blueCircleImageView.Hidden = (!alert.Active);
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                //datetime label 
                this._dateTimeLabel.SetFrameHeight(14);
                this._dateTimeLabel.SetFrameLocation(LeftTextMargin, TopMargin);
                this._dateTimeLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);

                //description line 1
                this._descriptionLine1Label.SetFrameHeight(20);
                this._descriptionLine1Label.SetFrameLocation(LeftTextMargin, _dateTimeLabel.Frame.Bottom + 4);
                this._descriptionLine1Label.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);

                //description line 2
                this._descriptionLine2Label.SetFrameHeight(20);
                this._descriptionLine2Label.SetFrameLocation(LeftTextMargin, _descriptionLine1Label.Frame.Bottom + 4);
                this._descriptionLine2Label.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
            }
        }
    }
}
