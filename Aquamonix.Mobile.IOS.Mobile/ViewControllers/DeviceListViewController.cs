using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using UIKit;
using CoreGraphics;
using Foundation;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Device list screen.
    /// </summary>
	public partial class DeviceListViewController : ListViewControllerBase
    {
        private static DeviceListViewController _instance;
        private List<DeviceViewModel> _allDevices = null;

        private bool CallDeviceBriefs
        {
            get
            {
                if (this.Predecessor != null)
                {
                    if (this.Predecessor is StartViewController || this.Predecessor is UserConfigViewController)
                        return false;
                }

                return true;
            }
        }

        private List<DeviceViewModel> AllDevices
        {
            get
            {
                //_allDevices = new List<DeviceViewModel>();
                if (_allDevices == null)
                {
                    var allDevices = DataCache.GetAllDevicesFromCache();
                    if (allDevices != null)
                    {
                        _allDevices = new List<DeviceViewModel>();

                        foreach (var device in allDevices)
                        {
                            _allDevices.Add(new DeviceViewModel(device));
                        }
                    }
                }

                return _allDevices;
            }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get {
                //return 200; 
                var output = (_searchBoxView.IsDisposed) ? 0 : _searchBoxView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            } 
        }

        private DeviceListViewController() : base()
        {
            ExceptionUtility.Try(() =>
            {
                //DataCache.SubscribeToAllDeviceUpdates(this.GotDeviceUpdate);
                this.Initialize();
            });
        }

        public static DeviceListViewController CreateInstance()
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance == null)
                {
                    _instance = new DeviceListViewController();
                }
            });

            return _instance;
        }

        public override void NavigateHome()
        {
            //do nothing, we're here already 
        }


        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = String.Empty;
            bool output = this._allDevices != null && this._allDevices.Count == 0;
            if (output)
            {
                emptyMessage = StringLiterals.DevicesEmptyMessage;
            }

            return output;
        }

        protected override void DoLoadData()
        {
            this.DoLoadData(false);
        }

        protected void DoLoadData(bool reloadFromServer)
        {
            ExceptionUtility.Try(() =>
            {
                this._allDevices = null;
                base.DoLoadData();

                this._navBarView.Refresh();

                if (User.Current != null)
                {
                    Action doLoad = () =>
                    {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            if (this._searchBoxView.Text.Length > 0)
                                this.DoSearch(this._searchBoxView.Text);
                            else
                                _tableViewController.LoadData(this.AllDevices);
                        });
                    };

                    if (this.CallDeviceBriefs || reloadFromServer)
                    {
                        ProgressUtility.SafeShow("Refreshing Device List", () =>
                        {
                            ServiceContainer.DeviceService.RequestDeviceBriefs(this.AllDevices.Select(d => d.Id)).ContinueWith((r) =>
                            {
                                ProgressUtility.Dismiss();
                                this._allDevices = null;
                                doLoad();
                            });
                        });
                    }
                    else
                    {
                        doLoad();
                    }

                    _tableViewController.TableViewSource.NavigateToDeviceDetails = NavigateToDeviceDetails;
                }
                this.HandleViewDidLayoutSubviews();
            });
        }

        protected void GotDeviceUpdate(IEnumerable<Aquamonix.Mobile.Lib.Domain.Device> devices)
        {
            ExceptionUtility.Try(() =>
            {
                if (this.IsShowing)
                {
                    LogUtility.LogMessage("Handling device update for devices ");
                    _allDevices = null;
                    this.LoadData();
                }
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

                this.HeaderView = this._searchBoxView;
                this.TableViewController = this._tableViewController;

                this.TabBar.SelectTab(1);
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

        protected override void HandleViewDidAppear(bool animated)
        {
            base.HandleViewDidAppear(animated);

            if (this.ShowConnectionError)
            {
                this.ShowConnectionError = false;
                AlertUtility.ShowErrorAlert(StringLiterals.ConnectionError, StringLiterals.AuthFailureMessage);

                Aquamonix.Mobile.IOS.Utilities.WebSockets.ConnectionManager.ReconnectProcess.Begin(showBannerStraightAway:true); 
            }
        }
        /*
        protected override void AdjustTableForReconBar(TableViewControllerBase tableVc, bool show)
        {
            if (tableVc.AllRowsVisible.HasValue)
            {
                if (tableVc.AllRowsVisible.Value)
                {
                    /*
                    var tableView = tableVc.TableView;
                    var tableFrame = tableView.Frame;
                    //_tableViewController.TableView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);
                    //tableView.Frame = new CoreGraphics.CGRect(tableFrame.Location, new CoreGraphics.CGSize(tableFrame.Width, 900));
                    if (!tableView.ScrollEnabled)
                       tableView.ScrollEnabled = true;
                    if (!tableView.AlwaysBounceVertical)
                        tableView.AlwaysBounceVertical = true;
                    //tableView.ContentOffset = new CoreGraphics.CGPoint(0, 50);
                    tableView.ContentSize = new CoreGraphics.CGSize(tableView.ContentSize.Width,200); 
                    */
                    /*
                    var tableView = tableVc.TableView;
                    //tableView.SetFrameY(200);

                    //TODO: adjust for if it goes past bottom of screen  (1)
                }
                else
                {
                    base.AdjustTableForReconBar(tableVc, show);
                }
            }
        }
        */


        protected override void OnReconnected()
        {
            base.OnReconnected();

            MainThreadUtility.InvokeOnMain(() => 
            {
                if (this.IsShowing)
                {
                    this.DoLoadData(true);
                }
            });
        }


        private void DoSearch(string searchString)
        {
            ExceptionUtility.Try(() =>
            {
                searchString = searchString.Trim().ToLower();
                List<DeviceViewModel> searchResults = new List<DeviceViewModel>();

                if (!String.IsNullOrEmpty(searchString))
                {
                    //starts with
                    var startsWith = this.AllDevices.Where((d) => d.Name != null && d.Name.ToLower().StartsWith(searchString) || d.Id.ToLower().StartsWith(searchString)).ToList();

                    //contains 
                    var contains = this.AllDevices.Where((d) => d.Name != null && d.Name.ToLower().Contains(searchString) || d.Id.ToLower().Contains(searchString)).ToList();

                    searchResults.AddRange(startsWith);

                    foreach (var dev in contains)
                    {
                        if (!searchResults.Where((r) => r.Id == dev.Id).Any())
                            searchResults.Add(dev);
                    }
                }
                else
                    searchResults = this.AllDevices;

                _tableViewController.LoadData(searchResults);
            });
        }

        private void NavigateToDeviceDetails(DeviceViewModel device)
        {
            try
            {
                ProgressUtility.SafeShow("Getting Device Details", async () =>
                {
                    var response = await ServiceContainer.DeviceService.RequestDevice(device.Device, () => { NavigateToDeviceDetails(device); });

                    MainThreadUtility.InvokeOnMain(() =>
                    {
                        if (response != null && response.IsSuccessful)
                        {
                            var d = response.Body.Devices.Items.FirstOrDefault().Value;
                            if (d == null)
                            {
                                ProgressUtility.Dismiss();
                                AlertUtility.ShowErrorAlert(StringLiterals.Error, StringLiterals.DeviceNotFoundErrorTitle);
                            }
                            else
                            {
                                this.Predecessor = null;
                                ProgressUtility.Dismiss();
                                this.NavigateTo(DeviceDetailsViewController.CreateInstance(d.Id));
                            }
                        }
                    });
                });
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
        }


        private class NavBarView : NavigationBarView
        {
            private const int RightMargin = 12;

            private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
            private readonly AquamonixLabel _nameLabel = new AquamonixLabel();

            public NavBarView() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    this._titleLabel.SetFontAndColor(NavHeaderFirstLineFont);
                    this._titleLabel.Text = StringLiterals.Devices;
                    this._titleLabel.TextAlignment = UITextAlignment.Center;
                    this._titleLabel.SizeToFit();

                    this._nameLabel.SetFontAndColor(NavHeaderSecondLineFont);
                    this._nameLabel.TextAlignment = UITextAlignment.Center;

                    this.Refresh();

                    this.AddSubviews(_titleLabel, _nameLabel);
                });
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    this._titleLabel.SetFrameLocation(0, 2);
                    this._titleLabel.SetFrameHeight(18);
                    this._titleLabel.SetFrameWidth(this.Frame.Width);
                    this._titleLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);

                    this._nameLabel.SetFrameX(0);
                    this._nameLabel.SetFrameHeight(13);
                    this._nameLabel.SetFrameWidth(this.Frame.Width);
                    this._nameLabel.SetFrameY(this._titleLabel.Frame.Bottom + 2);
                    this._nameLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
                });
            }

            public void Refresh()
            {
                if (User.Current != null)
                    this._nameLabel.Text = User.Current.Name;

                this._nameLabel.SizeToFit();
            }
        }

        private class DeviceListTableViewController : TopLevelTableViewControllerBase<DeviceViewModel, DeviceListTableViewSource>
        {
            public DeviceListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(DeviceListTableViewCell), DeviceListTableViewCell.TableCellKey);
            }

            protected override DeviceListTableViewSource CreateTableSource(IList<DeviceViewModel> values)
            {
                return new DeviceListTableViewSource(values);
            }
        }

        private class DeviceListTableViewSource : TableViewSourceBase<DeviceViewModel>
        {
            private const int NormalRowHeight = 106;
            private const int ShortRowHeight = 80;

            public Action<DeviceViewModel> NavigateToDeviceDetails { get; set; }

            public DeviceListTableViewSource(IList<DeviceViewModel> values) : base(values) { }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var device = this.Values[indexPath.Row];
                //return NormalRowHeight;
                return device.IsGroup ? ShortRowHeight : (device.Badges != null && device.Badges.Any() ? NormalRowHeight : ShortRowHeight);
            }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell output = null;

                ExceptionUtility.Try(() =>
                {
                    DeviceViewModel device = null;
                    if (indexPath.Row < Values.Count)
                        device = Values[indexPath.Row];

                    DeviceListTableViewCell cell = (DeviceListTableViewCell)tableView.DequeueReusableCell(DeviceListTableViewCell.TableCellKey, indexPath);
                    output = cell;

                    if (device != null)
                        cell.LoadCellValues(device);
                });

                return output;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                ExceptionUtility.Try(() =>
                {
                    if (this.Values != null && indexPath.Row < this.Values.Count)
                    {
                        var device = this.Values[indexPath.Row];
                        if (NavigateToDeviceDetails != null)
                            NavigateToDeviceDetails(device);
                    }
                });
            }
        }

        private class DeviceListTableViewCell : TableViewCellBase
        {
            public const string TableCellKey = "DeviceListTableViewCell";

            private const int LeftMargin = 19;
            private const int RightMargin = 13;
            private const int TopMargin = 17;

            private readonly AquamonixLabel _deviceNameLabel = new AquamonixLabel();
            private readonly AquamonixLabel _deviceDescriptionLabel = new AquamonixLabel();
            private readonly UIImageView _statusImageView = new UIImageView();
            private readonly List<IconWithLabel> _badgeViews = new List<IconWithLabel>();
            private readonly UIImageView _rightArrowImageView = new UIImageView();

            public DeviceListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    _deviceNameLabel.SetFontAndColor(DeviceNameFont);

                    _deviceDescriptionLabel.SetFontAndColor(DeviceDescriptionFont);

                    _rightArrowImageView.Image = Images.TableRightArrow;
                    _rightArrowImageView.SizeToFit();

                    this.ContentView.AddSubviews(_deviceNameLabel, _rightArrowImageView, _deviceDescriptionLabel, _statusImageView);
                });
            }

            public void LoadCellValues(DeviceViewModel device)
            {
                ExceptionUtility.Try(() =>
                {
                    this._deviceNameLabel.Text = device.Name;
                    this._deviceNameLabel.SizeToFit();

                    this._deviceDescriptionLabel.Text = device.Description;
                    this._deviceDescriptionLabel.SizeToFit();

                    foreach (var item in this._badgeViews)
                        item.RemoveFromSuperview();
                    this._badgeViews.Clear();

                    if (device.IsGroup)
                    {
                        this._statusImageView.Hidden = true;
                    }
                    else
                    {
                        foreach (var i in device.Badges)
                        {
                            var displayItem = new IconWithLabel(BadgeFont);
                            displayItem.SetIconAndText(GraphicsUtility.IconForDeviceBadge(i), i.Text);
                            displayItem.SetTextColor(GraphicsUtility.TextColorForSeverity(i.SeverityLevel));

                            this._badgeViews.Add(displayItem);
                            this.ContentView.AddSubview(displayItem);
                        }

                        if (device.DisplaySeverityLevel == SeverityLevel.Missing)
                            this._statusImageView.Hidden = true;
                        else
                        {
                            this._statusImageView.Hidden = false;
                            this._statusImageView.Image = GraphicsUtility.ImageForColorBar(device.DisplaySeverityLevel);
                            this._statusImageView.SizeToFit();
                        }
                    }
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                _statusImageView.SetFrameLocation(LeftMargin, TopMargin);

                var labelX = (this._statusImageView.Hidden) ? LeftMargin : 37;

                this._deviceNameLabel.SetFrameHeight(26);
                this._deviceNameLabel.SetFrameLocation(labelX, 14);

                this._deviceDescriptionLabel.SetFrameHeight(20);
                this._deviceDescriptionLabel.SetFrameLocation(_deviceNameLabel.Frame.X, 42);

                this._rightArrowImageView.SetFrameY(22);
                this._rightArrowImageView.AlignToRightOfParent(RightMargin);

                this._deviceNameLabel.EnforceMaxXCoordinate(_rightArrowImageView.Frame.X);
                this._deviceDescriptionLabel.EnforceMaxXCoordinate(_rightArrowImageView.Frame.X);

                for (int n = 0; n < _badgeViews.Count; n++)
                {
                    this._badgeViews[n].SizeToFit();
                    this._badgeViews[n].SetFrameX(n == 0 ? _statusImageView.Frame.X - 2 : _badgeViews[n - 1].Frame.Right + 12);
                    this._badgeViews[n].SetFrameY(_deviceDescriptionLabel.Frame.Bottom + 4);
                    this._badgeViews[n].SetFrameHeight(IconWithLabel.Height);
                }
            }
        }
    }
}

