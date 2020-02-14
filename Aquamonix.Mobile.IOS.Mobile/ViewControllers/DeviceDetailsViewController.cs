using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.IOS.UI;


using MapKit;
using CoreLocation;
using CoreGraphics;
using Aquamonix.Mobile.IOS.Model;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Device details screen. 
    /// </summary>
    public partial class DeviceDetailsViewController : ListViewControllerBase
    {

        private static DeviceDetailsViewController _instance;

        private bool ShowNavFooter
        {
            get
            {
                return (Device.SupportsIrrigationStop || Device.SupportsIrrigationPrev || Device.SupportsIrrigationNext);
            }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get { 
                //TODO: add IsDisposed method
                return _tableViewController.TableView.Frame.Top; 
            }
        }

        private DeviceDetailsViewController(string deviceId) : base(deviceId)
        {
            // temporarily moved to parent class
            //ExceptionUtility.Try(() =>
            //{
            //this.DeviceId = deviceId;
            //this.Device = new DeviceDetailViewModel(DataCache.GetDeviceFromCache(deviceId));

            //this.Initialize();

            //NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.Reconnected.ToString()), this.OnReconnect);
            //NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.Activated.ToString()), this.OnReconnect); // temporary fix
            //});
        }

        public static DeviceDetailsViewController CreateInstance(string deviceId)
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance != null && _instance.DeviceId != deviceId)
                {
                    _instance.Dispose();
                    _instance = null;
                }
            });

            if (_instance == null)
                _instance = new DeviceDetailsViewController(deviceId);

            return _instance;
        }

        protected override void HandleViewWillAppear(bool animated)
        {
            base.HandleViewWillAppear(animated);
            //  _tableViewController.TableView.Frame = new CGRect(0, 0, 400, 500);

        }

        protected override void HandleViewDidAppear(bool animated)
        {
            base.HandleViewDidAppear(animated);
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                this.Device = new DeviceDetailViewModel(DataCache.GetDeviceFromCache(this.DeviceId));
                this.Device.ChangeFeatureSettingValue = this.FeatureSettingValueChanged;
                if (this.Device == null)
                {
                    AlertUtility.ShowAlert(StringLiterals.DeviceNotFoundErrorTitle, StringLiterals.FormatDeviceNotFoundErrorMessage(this.DeviceId));
                }
                else
                {
                    this._tableViewController.LoadData(this.Device);
                    this._navBarView.SetDevice(this.Device);
                    this._navFooterView.Device = this.Device;
                    var pendingCommands = DataCache.GetCommandProgressesSubIds(CommandType.DeviceFeatureSetting, this.DeviceId);
                    foreach (var feature in this.Device.Features)
                    {
                        if (pendingCommands.Contains(feature.Id))
                            feature.ValueChanging = true;
                    }

                    this._tableViewController.TableViewSource.NavigateToAction = NavigateTo;
                }
                this.HandleViewDidLayoutSubviews();
            });
        }

        protected override void InitializeViewController()
        {
            ExceptionUtility.Try(() =>
            {
                base.InitializeViewController();

                this.NavigationBarView = this._navBarView;

                this._navFooterView.Hidden = (!this.ShowNavFooter);
                this.SetCustomBackButton();

                if (this.ShowNavFooter)
                    this.FooterView = this._navFooterView;

                this.TableViewController = this._tableViewController;
            });
        }

        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = null;
            bool isEmpty = true;
            if (this.Device != null)
            {
                //if it has visible features, it's not empty 
                if (this.Device.Features != null)
                {
                    if (this.Device.Features.Where((f) => f.IsVisible(this.Device?.Device)).Any())
                        isEmpty = false;
                }

                //if it's a group, and it has displayable devices, it's not empty
                if (this.Device.IsGroup)
                {
                    if (isEmpty)
                    {
                        if (this.Device.DevicesInGroup != null && this.Device.DevicesInGroup.Any())
                            isEmpty = false;
                    }
                }
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


        private void NavigateTo(DeviceFeatureViewModel feature)
        {
            switch (feature.Destination)
            {
                case StringLiterals.Alerts:
                    this.NavigateToAlerts();
                    break;

                case StringLiterals.Schedules:
                    this.NavigateToSchedules();
                    break;

                case StringLiterals.Circuits:
                    this.NavigateToCircuits();
                    break;

                case StringLiterals.Stations:
                    this.NavigateToStations(this.Device, feature);
                    break;


                case StringLiterals.Programs:
                    this.NavigateToPrograms(this.Device, feature);
                    break;

                case StringLiterals.PivotPrograms:
                    this.NavigateToPivotPrograms(this.Device, feature);
                    break;

                case StringLiterals.Pivot:
                    this.NavigateToPivotControls(this.Device, feature);
                    break;

                case "ProgramDisable":
                    this.LaunchEnableTimerScreen(this.Device, feature);
                    break;
            }
        }

        private void NavigateToPivotPrograms(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(PivotProgramsViewController.CreateInstance(this.Device?.Id));
                // this.NavigateTo(ProgramListViewController.CreateInstance(this.Device?.Id));
            });
        }

        private void NavigateToPivotControls(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                PivotControlViewController timerVc = PivotControlViewController.CreateInstance(device, feature as ProgramDisableFeatureViewModel, this.Device?.Id);
                this.NavigateTo(timerVc);
            });
        }

        private void NavigateToAlerts()
        {
            ProgressUtility.SafeShow("Getting Alerts", () =>
            {
                ServiceContainer.AlertService.RequestAlerts(this.Device.Device, NavigateToAlerts).ContinueWith((r) =>
                {
                    MainThreadUtility.InvokeOnMain(() =>
                    {
                        ProgressUtility.Dismiss();
                        if (r.Result != null && r.Result.IsSuccessful)
                        {
                            this.NavigateTo(AlertListViewController.CreateInstance(this.Device?.Id));
                        }
                    });
                });
            }, true);
        }

        private void NavigateToPrograms(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(ProgramListViewController.CreateInstance(this.Device?.Id));
            });
        }

        private void NavigateToStations(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(StationListViewController.CreateInstance(this.Device?.Id));
            });
        }

        private void NavigateToCircuits()
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(CircuitListViewController.CreateInstance(this.Device?.Id));
            });
        }

        private void NavigateToSchedules()
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(ScheduleListViewController.CreateInstance(this.Device?.Id));
            });
        }

        private void LaunchEnableTimerScreen(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
        {
            ExceptionUtility.Try(() =>
            {
                EnableTimerViewController timerVc = EnableTimerViewController.CreateInstance(device, feature as ProgramDisableFeatureViewModel);

                this.NavigateTo(timerVc);
            });
        }

        private void FeatureSettingValueChanged(DeviceDetailViewModel device, DeviceFeatureViewModel feature, DeviceSetting newSetting)
        {
            ExceptionUtility.Try(() =>
            {
                //TODO: is this always null? 
                DeviceSettingValue originalSettingValue = null;
                //var originalValue = feature?.Feature?.Setting?.GetValue(out originalSettingValue);
                var settingValue = newSetting?.GetValue();

                string confirmText = originalSettingValue?.Dictionary?.PromptConfirm?.LastOrDefault() ?? "Confirm";
                string cancelText = originalSettingValue?.Dictionary?.PromptCancel?.LastOrDefault() ?? "Cancel";

                if (settingValue != null)
                {
                    string promptText = String.Empty;
                    if (!String.IsNullOrEmpty(feature?.SettingsValueDictionary?.PromptText?.LastOrDefault()))
                    {
                        promptText = feature.SettingsValueDictionary.PromptText?.LastOrDefault();
                    }
                    else
                    {
                        promptText = String.Format("{0} {1}{2} {3}",
                                                   feature?.SettingsValueDictionary?.PromptPrefix?.LastOrDefault(),
                                                  feature.PromptValue,
                                                   newSetting?.Values?.Items?.FirstOrDefault().Value.Units,
                                                  feature?.SettingsValueDictionary?.PromptSuffix?.LastOrDefault());
                    }

                    AlertUtility.ShowConfirmationAlert(originalSettingValue?.Title, promptText,
                    (b) =>
                    {
                        if (b)
                        {
                            this.FeatureSettingValueChangeConfirmed(device, feature, originalSettingValue, newSetting, newSetting.Id);
                        }
                        else
                        {
                            var cell = this._tableViewController.GetCellFromFeatureId(feature.Id) as FeatureCell;
                            if (cell != null)
                            {
                                feature.ValueChanging = false;
                                cell.LoadCellValues(device, device.Features.Where((f) => f.Id == feature.Id).FirstOrDefault());
                            }
                        }
                    }, okButtonText: confirmText, cancelButtonText: cancelText);
                }
            });
        }

        private void FeatureSettingValueChangeConfirmed(DeviceDetailViewModel device, DeviceFeatureViewModel feature, DeviceSettingValue originalSettingValue, DeviceSetting newSetting, string settingId)
        {
            ExceptionUtility.Try(() =>
            {
                feature.ValueChanging = true;
                var cell = this._tableViewController.GetCellFromFeatureId(feature.Id) as FeatureCell;

                ProgressUtility.SafeShow(String.Empty, () =>
                {
                    feature.ValueChanging = true;
                    ServiceContainer.StatusService.SetSettings(
                        device.Device,
                        newSetting,
                        settingId,
                        handleUpdates: (r) => { this.HandleUpdatesForFeatureSetting(r, device, feature, originalSettingValue, newSetting, settingId); },
                        onReconnect: () => { FeatureSettingValueChangeConfirmed(device, feature, originalSettingValue, newSetting, settingId); }).ContinueWith((r) =>
                        {
                            MainThreadUtility.InvokeOnMain(() =>
                            {
                                ProgressUtility.Dismiss();

                                if (r.Result != null && !r.Result.IsFinal)
                                {
                                    if (cell != null)
                                    {
                                        cell.Enabled = false;

                                        if (cell is ProgramDisableViewCell)
                                        {
                                            var disableFeature = (feature as ProgramDisableFeatureViewModel);
                                            cell.DisabledText = newSetting?.Values?.Items?.FirstOrDefault().Value?.Value == disableFeature.EnableValue ? "Enabling..." : "Disabling...";
                                            cell.LoadCellValues(this.Device, feature);
                                        }
                                        else
                                        {
                                            cell.DisabledText = StringLiterals.UpdatingText; //"Updating...";
                                            cell.LoadCellValues(this.Device, feature);
                                        }
                                    }
                                }
                            });
                        });
                });
            });
        }

        private void HandleUpdatesForFeatureSetting(ProgressResponse response, DeviceDetailViewModel device, DeviceFeatureViewModel feature, DeviceSettingValue originalSettingValue, DeviceSetting newSetting, string settingId)
        {
            if (response != null)
            {
                var commandProgress = new CommandProgress(CommandType.DeviceFeatureSetting, response?.Body);
                commandProgress.SubItemIds = new string[] { feature.Id };
                DataCache.AddCommandProgress(commandProgress);

                if (response.IsFinal)
                {
                    var cell = this._tableViewController.GetCellFromFeatureId(feature.Id) as FeatureCell;

                    feature.ValueChanging = false;

                    if (cell != null)
                    {
                        cell.Enabled = true;
                        cell.LoadCellValues(this.Device, feature);
                    }

                    if (response.IsSuccessful)
                    {
                        //AlertUtility.ShowProgressResponse("Stop/Start Program " + program.Id, response);
                    }
                    else
                    {
                        if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                            AlertUtility.ShowAppError(response?.ErrorBody);
                    }
                }
            }

            AlertUtility.ShowProgressResponse(feature.Id, response);
        }

        // temporarily moved to parent class
        //private void OnReconnect(NSNotification notification)
        //{
        //    ExceptionUtility.Try(() =>
        //    {
        //        if (this.IsShowing)
        //        {
        //            if (this.Device?.Device != null)
        //            {
        //                ServiceContainer.DeviceService.RequestDevice(this.Device.Device, silentMode: true);
        //            }
        //        }
        //    });
        //}

        private class NavBarView : NavigationBarView
        {
            private const int RightMargin = 0; /// 12; 

            private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
            private readonly AquamonixLabel _subtextLabel = new AquamonixLabel();

            public NavBarView() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    this._titleLabel.SetFontAndColor(NavHeaderFontBig);
                    this._titleLabel.TextAlignment = UITextAlignment.Center;
                    this._titleLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;

                    this._subtextLabel.SetFontAndColor(NavHeaderFontSmall);
                    this._subtextLabel.TextAlignment = UITextAlignment.Center;

                    this.AddSubviews(_titleLabel, _subtextLabel);
                });
            }

            public void SetDevice(DeviceDetailViewModel device)
            {
                ExceptionUtility.Try(() =>
                {
                    if (device != null)
                    {
                        this._titleLabel.Text = device.Name;
                        this._titleLabel.SetFrameHeight(Height);
                        this._titleLabel.SizeToFit();

                        this._subtextLabel.Text = this.GetSubtext(device);
                        this._subtextLabel.SizeToFit();
                    }
                });
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    this._titleLabel.SetFrameLocation(0, 0);
                    this._titleLabel.SetFrameWidth(this.Frame.Width);
                    this._titleLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);

                    this._subtextLabel.SetFrameLocation(0, this._titleLabel.Frame.Bottom);
                    this._subtextLabel.CenterHorizontallyInParent();
                    this._subtextLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
                });
            }

            private string GetSubtext(DeviceDetailViewModel device)
            {
                string output = String.Empty;

                if (device != null)
                {
                    if (!String.IsNullOrEmpty(device.StatusText))
                        output += device.StatusText + ". ";

                    if (device.StatusLastUpdated != null)
                    {
                        output += String.Format("Updated {0} ago.", DateTimeUtil.HowLongAgo(device.StatusLastUpdated.Value));
                    }
                }

                return output.Trim();
            }
        }

        private class DeviceDetailsTableViewController : TableViewControllerBase
        {
            public DeviceDetailsTableViewSource TableViewSource
            {
                get
                {
                    if (this.TableView != null)
                        return this.TableView.Source as DeviceDetailsTableViewSource;

                    return null;
                }
            }

            public DeviceDetailsTableViewController() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    this.TableView.RegisterClassForCellReuse(typeof(SensorsViewCell), SensorsViewCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(NormalFeatureCell), NormalFeatureCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(SpecialViewCell), SpecialViewCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(SliderFeatureViewCell), SliderFeatureViewCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(ProgramDisableViewCell), ProgramDisableViewCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(DeviceListViewCell), DeviceListViewCell.TableCellKey);
                    this.TableView.RegisterClassForCellReuse(typeof(SpecialViewCellMap), SpecialViewCellMap.TableCellKeymm);

                    this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                });
            }
            public void LoadData(DeviceDetailViewModel device)
            {
                ExceptionUtility.Try(() =>
                {
                    DeviceDetailsTableViewSource source = new DeviceDetailsTableViewSource(device);

                    TableView.Source = source;

                    TableView.ReloadData();
                });
            }

            protected override void HandleViewDidLoad()
            {
                base.HandleViewDidLoad();

            }

            public UITableViewCell GetCellFromFeatureId(string featureId)
            {
                UITableViewCell output = null;

                ExceptionUtility.Try(() =>
                {
                    if (this.TableViewSource != null)
                        output = this.TableViewSource.GetCellFromFeatureId(this.TableView, featureId);
                });

                return output;
            }
        }

        private class DeviceDetailsTableViewSource : TableViewSourceBase<DeviceFeatureViewModel>
        {
            private DeviceDetailViewModel _device;
            private Action<DeviceFeatureViewModel> _navigateToAction;
            public const int PivotFeatureRowHeight = 390;

            public Action<DeviceFeatureViewModel> NavigateToAction
            {
                get { return this._navigateToAction; }
                set { this._navigateToAction = WeakReferenceUtility.MakeWeakAction(value); }
            }

            public DeviceDetailsTableViewSource(DeviceDetailViewModel device) : base()
            {
                ExceptionUtility.Try(() =>
                {
                    this._device = device;

                    foreach (var feature in device.Features)
                    {
                        if (feature.DisplayType != DeviceFeatureRowDisplayType.None)
                            this.Values.Add(feature);
                    }

                    if (device.IsGroup)
                    {
                        this.Values.Add(new DeviceFeatureViewModel()
                        {
                            DisplayType = DeviceFeatureRowDisplayType.Devices
                        });
                    }
                });
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                // reversed changes from build 84
                //tableView.ScrollEnabled = true;
                //tableView.Frame = new CGRect(0, 70, UIScreen.MainScreen.Bounds.Width, 560);
                return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return ExceptionUtility.Try<nfloat>(() =>
                {
                    //tableView.ScrollEnabled = true; // reversed changes from build 84
                    if (Values != null && indexPath.Row < Values.Count)
                    {
                        if (Values[indexPath.Row].DisplayType == DeviceFeatureRowDisplayType.Sensors)
                            return SensorsPanel.CalculateHeight(this._device.Sensors);

                        if (Values[indexPath.Row].DisplayType == DeviceFeatureRowDisplayType.Normal)
                            return DefaultRowHeight;

                        if (Values[indexPath.Row].DisplayType == DeviceFeatureRowDisplayType.Devices)
                            return DeviceGroupListPanel.CalculateHeight(this._device.Sensors);

                        if (Values[indexPath.Row].DisplayType == DeviceFeatureRowDisplayType.PivotFeature)
                        {
                            //tableView.Frame = new CGRect(0, 70, UIScreen.MainScreen.Bounds.Width, 390);
                            return PivotFeatureRowHeight;
                        }
                    }
                    return DefaultRowHeight;

                });
            }


            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Row < this.Values.Count)
                {

                    var feature = this.Values[indexPath.Row];

                    switch (feature.DisplayType)
                    {
                        case DeviceFeatureRowDisplayType.Sensors:
                            var sensorsCell = (SensorsViewCell)tableView.DequeueReusableCell(SensorsViewCell.TableCellKey, indexPath);
                            sensorsCell.SetSensors(_device, feature, _device.Sensors);
                            return sensorsCell;

                        case DeviceFeatureRowDisplayType.PivotFeature:
                            var PivotMapCell = (SpecialViewCellMap)tableView.DequeueReusableCell(SpecialViewCellMap.TableCellKeymm, indexPath);
                            PivotMapCell.LoadCellValues(_device, feature);
                            return PivotMapCell;

                        case DeviceFeatureRowDisplayType.Devices:
                            var devicesCell = (DeviceListViewCell)tableView.DequeueReusableCell(DeviceListViewCell.TableCellKey, indexPath);
                            devicesCell.SetDevices(_device);
                            return devicesCell;

                        case DeviceFeatureRowDisplayType.Normal:
                            var normalCell = (NormalFeatureCell)tableView.DequeueReusableCell(NormalFeatureCell.TableCellKey, indexPath);
                            normalCell.LoadCellValues(_device, feature);
                            return normalCell;

                        case DeviceFeatureRowDisplayType.Slider:
                            var sliderCell = (SliderFeatureViewCell)tableView.DequeueReusableCell(SliderFeatureViewCell.TableCellKey, indexPath);
                            sliderCell.LoadCellValues(_device, feature);
                            return sliderCell;

                        case DeviceFeatureRowDisplayType.Special:
                            if (feature.Id == "ProgramDisable")
                            {
                                var disableCell = (ProgramDisableViewCell)tableView.DequeueReusableCell(ProgramDisableViewCell.TableCellKey, indexPath);
                                disableCell.LoadCellValues(_device, feature);
                                return disableCell;
                            }
                            else
                            {
                                var specialCell = (SpecialViewCell)tableView.DequeueReusableCell(SpecialViewCell.TableCellKey, indexPath);
                                specialCell.LoadCellValues(_device, feature);
                                return specialCell;
                            }
                    }
                }

                return null;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                ExceptionUtility.Try(() =>
                {
                    if (indexPath.Row < this.Values.Count)
                    {
                        var cell = tableView.CellAt(indexPath) as FeatureCell;
                        if (cell != null)
                        {
                            if (cell.Enabled)
                            {
                                var feature = this.Values[indexPath.Row];

                                if (!feature.ValueChanging)
                                {
                                    if (!String.IsNullOrEmpty(feature.Destination))
                                    {
                                        if (this.NavigateToAction != null)
                                            this.NavigateToAction(feature);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var mapcell = tableView.CellAt(indexPath) as FeatureCellforMap;

                            var feature = this.Values[indexPath.Row];

                            if (!feature.ValueChanging)
                            {
                                if (!String.IsNullOrEmpty(feature.Destination))
                                {
                                    if (this.NavigateToAction != null)
                                        this.NavigateToAction(feature);
                                }
                            }

                        }
                    }
                });
            }

            public UITableViewCell GetCellFromFeatureId(UITableView tableView, string featureId)
            {
                UITableViewCell output = null;

                ExceptionUtility.Try(() =>
                {
                    int index = 0;
                    for (int n = 0; n < this.Values.Count; n++)
                    {
                        if (featureId == this.Values[n].Id)
                        {
                            index = n;
                            break;
                        }
                    }
                    if (index >= 0)
                    {
                        output = tableView.CellAt(NSIndexPath.FromRowSection(index, 0));
                    }
                });

                return output;
            }
        }

        private class SensorsViewCell : TableViewCellBase
        {
            public const string TableCellKey = "SensorsViewCell";

            private readonly SensorsPanel _sensorsPanel = new SensorsPanel();
            private DeviceDetailViewModel _device;

            public SensorsViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._sensorsPanel.OnUpdateClicked = () =>
                    {
                        ExceptionUtility.Try(() =>
                        {
                            this._sensorsPanel.SetUpdatingMode(true);

                            ProgressUtility.SafeShow("Updating Sensors", () =>
                            {
                                ServiceContainer.SensorService.RequestSensors(
                                    this._device.Device,
                                    handleUpdates: this.HandleUpdatesForRequestSensors,
                                    onReconnect: this._sensorsPanel.OnUpdateClicked
                                    ).ContinueWith((r) =>
                                {
                                    if (r.Result != null)
                                    {
                                        DataCache.AddCommandProgress(new CommandProgress(CommandType.UpdateSensors, r.Result.Body));
                                    }

                                    MainThreadUtility.InvokeOnMain(() =>
                                    {
                                        ProgressUtility.Dismiss();

                                        if (r.Result != null && r.Result.IsSuccessful)
                                        {
                                        }
                                        //else
                                        //    AlertUtility.ShowAppError(r.Result?.ErrorBody);
                                    });
                                });
                            }, blockUI: false);
                        });
                    };

                    this.ContentView.AddSubview(_sensorsPanel);
                });
            }

            public void SetSensors(DeviceDetailViewModel device, DeviceFeatureViewModel sensorsFeature, IEnumerable<SensorGroupViewModel> sensors)
            {
                ExceptionUtility.Try(() =>
                {
                    this._device = device;
                    this._sensorsPanel.SetSensors(sensors, sensorsFeature);

                    bool isUpdating = device.IsUpdatingStatus;
                    if (!isUpdating)
                    {
                        if (DataCache.HasPendingCommands(CommandType.UpdateSensors, _device.Id))
                            isUpdating = true;
                    }
                    this._sensorsPanel.SetUpdatingMode(isUpdating);
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                _sensorsPanel.SetFrameLocation(0, 0);
                _sensorsPanel.SetFrameSize(this.ContentView.Frame.Width, _sensorsPanel.CalculateHeight());
            }

            private void HandleUpdatesForRequestSensors(ProgressResponse response)
            {
                MainThreadUtility.InvokeOnMain(() =>
                {
                    if (response != null)
                    {
                        DataCache.AddCommandProgress(new CommandProgress(CommandType.UpdateSensors, response.Body));

                        if (response.IsFinal)
                        {
                            this._sensorsPanel.SetUpdatingMode(false);
                            //this._sensorsPanel.SetSensors();

                            //if (!response.IsSuccessful)
                            //    AlertUtility.ShowAppError(response.ErrorBody);
                        }
                    }
                });
            }
        }

        private class DeviceListViewCell : TableViewCellBase
        {
            public const string TableCellKey = "DeviceListViewCell";

            private readonly DeviceGroupListPanel _devicesPanel = new DeviceGroupListPanel();
            //private DeviceDetailViewModel _device;
            //private AquamonixLabel _measuringLabel = new AquamonixLabel();

            public DeviceListViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this.ContentView.AddSubview(_devicesPanel);
                });
            }

            public void SetDevices(DeviceDetailViewModel device)
            {
                ExceptionUtility.Try(() =>
                {
                    //this._device = device;
                    this._devicesPanel.SetDevices(device.Sensors);
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                _devicesPanel.SetFrameLocation(0, 350);
                _devicesPanel.SetFrameSize(this.ContentView.Frame.Width, _devicesPanel.CalculateHeight());
            }
        }

        private class SliderFeatureViewCell : FeatureCell
        {
            public const string TableCellKey = "SliderViewCell";
            private const int SliderDelayMs = 100;

            private Dictionary<long, float> _savedValues = new Dictionary<long, float>();
            private bool _hideText;
            private AquamonixLabel _measuringLabel = new AquamonixLabel();

            private readonly CustomSlider _slider = new CustomSlider();

            public override bool Enabled
            {
                get
                {
                    return base.Enabled;
                }
                set
                {
                    this._slider.Enabled = value;
                    base.Enabled = value;
                }
            }

            public SliderFeatureViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._arrowImageView.Hidden = true;

                    this._slider.Continuous = true;

                    this._slider.OnTouchesEnded = () =>
                    {
                        if (this.Device != null && this.Device.ChangeFeatureSettingValue != null)
                        {
                            if (this.Feature.Feature.Setting != null)
                            {
                                var newSetting = new DeviceSetting() { Id = this.Feature.Feature.Setting.Id, Values = new ItemsDictionary<DeviceSettingValue>() };

                                DeviceSettingValue settingValueObj = null;
                                this.Feature?.Feature?.Setting?.GetValue(out settingValueObj);

                                if (settingValueObj != null)
                                {
                                    var settingValueClone = settingValueObj.Clone();

                                    //get rid of unnecessary & big properties
                                    settingValueClone.Dictionary = null;
                                    settingValueClone.Validation = null;
                                    settingValueClone.Enum = null;
                                    settingValueClone.DisplayType = null;
                                    settingValueClone.DisplayPrecision = null;
                                    settingValueClone.EditPrecision = null;
                                    settingValueClone.Title = null;

                                    double valueFactor = this.GetValueFactor();

                                    int precision = 10;
                                    int p;
                                    if (Int32.TryParse(settingValueObj?.EditPrecision, out p))
                                        precision = p;

                                    var sliderValue = GetSliderValueFromEarlier();

                                    settingValueClone.Value = (MathUtil.RoundToNearest((int)(sliderValue * valueFactor), precision)).ToString();
                                    newSetting.Values.Add("Value", settingValueClone);
                                    this.Feature.PromptValue = settingValueClone.Value;

                                    this.Device.ChangeFeatureSettingValue(this.Device, this.Feature, newSetting);
                                    this._slider.Enabled = false;

                                    this.SetSettingValueText(settingValueClone.Value, settingValueClone);
                                }
                            }
                        }

                        this._savedValues.Clear();
                    };

                    this._slider.ValueChanged += (o, e) =>
                    {
                        this.SetCurrentSliderValueText();
                    };

                    this._measuringLabel.Font = this._label.Font;
                    this._measuringLabel.Text = StringLiterals.UpdatingText;

                    this.AddSubview(this._slider);
                });
            }

            public override void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                ExceptionUtility.Try(() =>
                {
                    base.LoadCellValues(device, feature);

                    DeviceSettingValue settingValueObj = null;
                    var settingValue = feature?.Feature?.Setting?.GetValue(out settingValueObj);


                    if (!this.Enabled)
                    {
                        if (!String.IsNullOrEmpty(this.DisabledText))
                            this.SetText(this.DisabledText);

                        this._label.TextColor = Colors.LightGrayTextColor;
                    }
                    else
                    {
                        this._label.TextColor = Colors.StandardTextColor;
                        this.SetSettingValueText(settingValue, settingValueObj);

                        var valueFactor = this.GetValueFactor();

                        float value = 0;
                        Single.TryParse(settingValue, out value);

                        this._slider.SetValue(value / (float)valueFactor, false);
                    }

                    this._slider.Enabled = !feature.ValueChanging && feature.Editable && this.Enabled;
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                var minSliderWidth = (UIScreen.MainScreen.Bounds.Width / 2);

                var textLen = this._measuringLabel.Text.StringSize(this._measuringLabel.Font).Width;

                this._slider.SetFrameX(this._label.Frame.X + textLen + 20);
                this._slider.SetFrameWidth(this.ContentView.Frame.Width - this._slider.Frame.X - RightMargin);
                this._slider.CenterVerticallyInParent();

                //ensure slider is at least 50% of screen width 
                if (this._slider.Frame.Width < minSliderWidth)
                {
                    if (!_hideText)
                    {
                        _hideText = true;
                        this.SetCurrentSliderValueText(false);
                    }
                    this._slider.SetFrameWidth(minSliderWidth);
                    this._slider.AlignToRightOfParent(RightMargin);
                }
            }

            private void SetCurrentSliderValueText(bool save = true)
            {
                DeviceSettingValue settingValueObj = null;
                this.Feature?.Feature?.Setting?.GetValue(out settingValueObj);
                var valueFactor = this.GetValueFactor();
                var value = this._slider.Value;

                if (save)
                {
                    while (_savedValues.Count > 200)
                    {
                        _savedValues.Remove(_savedValues.Keys.First());
                    }

                    //save the values on a timescale
                    long time = DateTime.Now.ToFileTime();
                    if (_savedValues.ContainsKey(time))
                        _savedValues[time] = value;
                    else
                        _savedValues.Add(time, value);
                }

                int precision = 10;
                int p;
                if (Int32.TryParse(settingValueObj?.EditPrecision, out p))
                    precision = p;

                value = (int)(value * (double)valueFactor);
                value = MathUtil.RoundToNearest((int)value, precision);

                this.SetSettingValueText(value.ToString(), settingValueObj);
            }

            private float GetSliderValueFromEarlier()
            {
                return ExceptionUtility.Try<float>(() =>
                {
                    var keys = _savedValues.Keys.ToArray();
                    var before = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(200)).ToFileTime();

                    //for (int n = keys.Length - 1; n >= 0; n--)
                    //{
                    //    if (keys[n] <= before)
                    //    {
                    //        if (_savedValues.ContainsKey(keys[n]))
                    //        {
                    //            var output = _savedValues[keys[n]];
                    //            return output;
                    //        }
                    //    }
                    //}

                    return _slider.Value;
                });
            }

            private void SetSettingValueText(string valueString, DeviceSettingValue valueObj)
            {
                //find shortest text 
                string shortestText = String.Empty;

                if (!_hideText)
                {
                    int shortestLength = 10000;
                    if (valueObj?.Dictionary?.ValuePrefix != null)
                    {
                        foreach (string s in valueObj.Dictionary.ValuePrefix)
                        {
                            if (!String.IsNullOrEmpty(s) && s.Length < shortestLength)
                            {
                                shortestText = s;
                                shortestLength = s.Length;
                            }
                        }
                    }
                }

                this._label.Text = String.Format("{0} {1}{2}", shortestText.Trim(), valueString, valueObj?.Units);
                this._label.SizeToFit();


            }

            private double GetValueFactor()
            {
                DeviceSettingValue settingValueObj = null;
                this.Feature?.Feature?.Setting?.GetValue(out settingValueObj);

                double valueFactor = 100;
                if (settingValueObj?.Validation != null)
                    valueFactor = settingValueObj.Validation.Max;

                return valueFactor;
            }

            private class CustomSlider : UISlider
            {
                public CustomSlider() : base() { }

                public Action OnTouchesEnded { get; set; }

                public override void TouchesEnded(NSSet touches, UIEvent evt)
                {
                    base.TouchesEnded(touches, evt);

                    if (this.OnTouchesEnded != null)
                        this.OnTouchesEnded();
                }
            }
        }

        private class SpecialViewCell : FeatureCell
        {
            public const string TableCellKey = "SpecialViewCell";

            public SpecialViewCell(IntPtr handle) : base(handle)
            {
            }

            public override void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                base.LoadCellValues(device, feature);
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();
            }
        }

        private class ProgramDisableViewCell : SpecialViewCell
        {
            public new const string TableCellKey = "ProgramDisableViewCell";

            public ProgramDisableViewCell(IntPtr handle) : base(handle)
            {
            }

            public override void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                var programDisableViewModel = (feature as ProgramDisableFeatureViewModel);
                if (device.IsGroup)
                    programDisableViewModel.DisplayText = programDisableViewModel.SettingName;

                base.LoadCellValues(device, feature);

                if (!this.Enabled)
                {
                    if (!String.IsNullOrEmpty(this.DisabledText))
                        this.SetText(this.DisabledText);

                    this._label.TextColor = Colors.LightGrayTextColor;
                }
                else
                    this._label.TextColor = Colors.StandardTextColor;

                //disabled for 9h, 20m
                //disabled indefinitely 
                //enabled 
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();
            }
        }

        private class NormalFeatureCell : FeatureCell
        {
            public const string TableCellKey = "NormalFeatureCell";

            public NormalFeatureCell(IntPtr handle) : base(handle)
            {
            }

            public override void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                ExceptionUtility.Try(() =>
                {
                    base.LoadCellValues(device, feature);

                    this._descriptionLabel.Hidden = true;
                    if (!String.IsNullOrEmpty(feature.Description))
                    {
                        this._descriptionLabel.Hidden = false;
                        this._descriptionLabel.Text = feature.Description;
                        this._descriptionLabel.SizeToFit();
                    }
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();
            }
        }

        private class FeatureCell : TableViewCellBase
        {
            protected const int RightMargin = 13;
            protected const int RightTextMargin = 37;
            protected const int LeftMargin = 10;

            protected readonly UIImageView _arrowImageView = new UIImageView();
            protected readonly UIImageView _iconImageView = new UIImageView();
            protected readonly AquamonixLabel _label = new AquamonixLabel();
            protected readonly AquamonixLabel _descriptionLabel = new AquamonixLabel();
            protected readonly AquamonixLabel _descriptionByLineLabel = new AquamonixLabel();

            private static UIImage _alertsIcon = UIImage.FromFile("Images/alarms1.png");
            private static UIImage _alertsIconRed = UIImage.FromFile("Images/alarms1_active.png");
            private static UIImage _stationsIcon = UIImage.FromFile("Images/stations.png");
            private static UIImage _schedulesIcon = UIImage.FromFile("Images/schedule.png");
            private static UIImage _programsIcon = UIImage.FromFile("Images/programs1.png");
            private static UIImage _circuitsIcon = UIImage.FromFile("Images/Circuits.png");
            private static UIImage _scaleFactorIcon = UIImage.FromFile("Images/percentage.png");
            private static UIImage _disableIcon = UIImage.FromFile("Images/disable.png");

            public DeviceDetailViewModel Device { get; protected set; }

            public DeviceFeatureViewModel Feature { get; protected set; }

            public virtual bool Enabled { get; set; }

            public string DisabledText { get; set; }

            protected virtual UIImage Image { get; }

            public FeatureCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this.Enabled = true;
                    _arrowImageView.Image = Images.TableRightArrow;
                    _arrowImageView.SizeToFit();

                    _descriptionLabel.SetFontAndColor(MainTextNormal);
                    _descriptionByLineLabel.SetFontAndColor(ByLineText);

                    _label.SetFontAndColor(MainTextNormal);

                    this.ContentView.AddSubviews(_iconImageView, _label, _descriptionLabel, _arrowImageView);

                });
            }

            public virtual void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                ExceptionUtility.Try(() =>
                {
                    this.Device = device;
                    this.Feature = feature;
                    this.Feature = feature;
                    var image = GetImageForFeature(feature);

                    this.SetTextAndImage(image, feature.DisplayText);

                    if (feature.ShowRed)
                        this._label.SetFontAndColor(MainTextBoldRed);
                    else
                        this._label.SetFontAndColor(MainTextNormal);

                    this._label.SizeToFit();
                });
            }

            protected UIImage GetImageForFeature(DeviceFeatureViewModel feature)
            {
                UIImage image = null;

                ExceptionUtility.Try(() =>
                {
                    switch (feature.Type)
                    {
                        case DeviceFeatureTypes.AlertList:
                            image = _alertsIcon;
                            if (feature.ShowRed)
                                image = _alertsIconRed;
                            break;
                        case DeviceFeatureTypes.StationList:
                            image = _stationsIcon;
                            break;
                        case DeviceFeatureTypes.ProgramList:
                            image = _programsIcon;
                            break;
                        case DeviceFeatureTypes.PivotProgramsFeature:
                            image = _programsIcon;
                            break;
                        case DeviceFeatureTypes.ScheduleList:
                            image = _schedulesIcon;
                            break;
                        case DeviceFeatureTypes.CircuitList:
                            image = _circuitsIcon;
                            break;
                        case DeviceFeatureTypes.Setting:
                            switch (feature.Id)
                            {
                                case DeviceFeatureIds.ProgramScaleFactor:
                                    image = _scaleFactorIcon;
                                    break;

                                case DeviceFeatureIds.ProgramDisable:
                                    image = _disableIcon;
                                    break;
                            }
                            break;
                    }
                });

                return image;
            }

            protected void SetTextAndImage(UIImage image, string text)
            {
                ExceptionUtility.Try(() =>
                {
                    this.SetImage(image);
                    this.SetText(text);
                });
            }

            protected void SetText(string text)
            {
                ExceptionUtility.Try(() =>
                {
                    this._label.Text = text;
                    this._label.SizeToFit();
                });
            }

            protected void SetImage(UIImage image)
            {
                ExceptionUtility.Try(() =>
                {
                    this._iconImageView.Image = image;
                    this._iconImageView.SizeToFit();
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();


                this._iconImageView.SetFrameLocation(LeftMargin, this.ContentView.Frame.Height / 2 - this._iconImageView.Frame.Height / 2);
                this._label.SetFrameHeight(20);
                this._label.SetFrameX(45);
                this._label.CenterVerticallyInParent();
                this._label.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
                //this._label.SetFrameY(-100);

                this._arrowImageView.CenterVerticallyInParent();
                this._arrowImageView.AlignToRightOfParent(RightMargin);
                //this._arrowImageView.SetFrameY(-50);

                this._descriptionLabel.SetFrameHeight(_label.Frame.Height);
                this._descriptionLabel.SetFrameY(_label.Frame.Y);
                this._descriptionLabel.EnforceMaxWidth(this.Frame.Width - RightTextMargin - (this._label.Frame.Right + 8));
                this._descriptionLabel.AlignToRightOfParent(RightTextMargin);

                if (Feature == null)
                {
                    this._arrowImageView.Hidden = true;
                }
                //this._descriptionLabel.EnforceMaxXCoordinate(this.Frame.Width - RightTextMargin);
            }
        }

        private class FeatureCellforMap : TableViewCellBase
        {
            // call api for status penal and circle //
            protected const int RightMargin = 13;
            protected const int RightTextMargin = 37;
            protected const int LeftMargin = 10;

            public DeviceDetailViewModel Device { get; protected set; }
            public DeviceFeatureViewModel Feature { get; protected set; }
            public DeviceFeatureViewModel Program { get; protected set; }
            public virtual bool Enabled { get; set; }
            public string DisabledText { get; set; }

            private CLLocationManager _location = new CLLocationManager();
            private string _status = string.Empty;
            private string _direction;
            private int _currentProgramId;
            private PivotMapDelegate _mapDel;

            protected readonly UIImageView _arrowImageView = new UIImageView();
            protected readonly UIView _topView = new UIView();
            protected UIView _pivotView = new MKAnnotationView();
            private readonly UILabel _lblRunning = new UILabel();
            protected readonly UIImageView _runningImageView = new UIImageView();
            private readonly UILabel _lblStop = new UILabel();
            protected readonly UIImageView _stopImageView = new UIImageView();
            private readonly UILabel _lblEndGun = new UILabel();
            private readonly UILabel _lblAux1 = new UILabel();
            private readonly UILabel _lblAux2 = new UILabel();
            private readonly UILabel _lblProgramStatus = new UILabel();
            private readonly UILabel _lblProgramStatusEdit = new UILabel();
            protected readonly UIImageView _waitingImageView = new UIImageView();
            protected virtual MKMapView _mapView { set; get; }
            private readonly UILabel _lblforward = new UILabel();
            protected readonly UIImageView _forwardImageView = new UIImageView();
            private readonly UILabel _lblRevers = new UILabel();
            protected readonly UIImageView _reversImageView = new UIImageView();
            private readonly UILabel _lblwet = new UILabel();
            private readonly UILabel _lblDry = new UILabel();
            protected readonly UIImageView _wetImageView = new UIImageView();
            private readonly UILabel _lblSpeed = new UILabel();
            private readonly UILabel _lblWatering = new UILabel();
            protected readonly UIImageView _speedImageView = new UIImageView();
            protected readonly UIImageView _dryImageView = new UIImageView();
            private readonly UIImageView _btnNext = new UIImageView();
            private readonly UIImageView _aux1 = new UIImageView();
            private readonly UIImageView _aux2 = new UIImageView();
            private readonly UIView _separator1 = new UIView();
            private readonly UIView _separator2 = new UIView();
            private readonly UIView _separator3 = new UIView();
            private readonly UIView _separator4 = new UIView();

            public virtual void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                // ProgranStart = program;
                var data = device.Device.MetaData.Device.Features;
                ExceptionUtility.Try(() =>
                {
                    //this.Program = program;
                    this.Device = device;
                    this.Feature = feature;
                    this._direction = device.DirectionValue;
                    //  var u = Aquamonix.Mobile.Lib.Environment.AppSettings.unit;
                    //const double lat = 28.612840;
                    //const double lon = 77.231127;
                    double lat = double.Parse(device.Device.MetaData.Device.Location.Latitude.ToString());
                    double lon = double.Parse(device.Device.MetaData.Device.Location.Longitude.ToString());
                    if (device.Device.MetaData.Device.Location.Latitude == 0 && device.Device.MetaData.Device.Location.Longitude == 0)
                    {
                        lat = 0.000001;
                        lon = 0.000001;
                    }
                    var mapCenter = new CLLocationCoordinate2D(lat, lon);
                    var length = int.Parse(Device.Device.MetaData.Device.LengthMetres?.Value) + 20;

                    // todo increase the size of the pivot mimic
                    _mapView.Frame = new CGRect(-150, 0, this.Frame.Width + 150, 390); //new CGRect(0 - this._topView.Frame.Width, 0, this.Frame.Width + this._topView.Frame.Width, 390);
                    var mapLength = 135 * length / 20; //length * 2 * (this.Frame.Width + this._topView.Frame.Width) / (this.Frame.Width - this._topView.Frame.Width) / 0.9;
                    var mapRegion = MKCoordinateRegion.FromDistance(mapCenter, mapLength, mapLength);
                    _mapView.CenterCoordinate = mapCenter;
                    _mapView.Region = mapRegion;
                    if (_mapView.Overlays != null && _mapView.Overlays.Length > 0)
                    {
                        foreach (var overlay in _mapView.Overlays.ToList())
                        {
                            _mapView.RemoveOverlay(overlay);
                        }
                    }
                    if (_mapView.Annotations != null && _mapView.Annotations.Length > 0)
                    {
                        foreach (var annotation in _mapView.Annotations.ToList())
                        {
                            _mapView.RemoveAnnotation(annotation);
                        }
                    }
                    _mapDel = new PivotMapDelegate(device);
                    _mapView.Delegate = _mapDel;

                    //var circleOverlay = MKCircle.Circle(mapCenter, int.Parse(Device.Device.MetaData.Device.LengthMetres.Value));
                    var circleOverlay = MKCircle.Circle(mapCenter, length);
                    _mapView.AddOverlay(circleOverlay);


                    _mapView.AddAnnotation(new PivotAnnotation("", mapCenter));
                    _mapView.AddAnnotation(new NextLineAnnotation("", mapCenter));
                    _mapView.AddAnnotation(new ZeroAnnotation("", mapCenter));
                    this._mapView.MapType = MKMapType.Satellite;
                    this._mapView.SizeToFit();

                    //TODO fix all of the below program name strings
                    if (device.Device.Programs.Status != null)
                    {
                        if (device.Device.Programs.Status.Value == StringLiterals.Running)
                        {
                            if (this.Device != null)
                            {
                                foreach (var p in this.Device.Programs) ;
                                //p.StopStartProgram = this._lblProgramStatus;

                                this._runningImageView.Image = Images.Running;
                                this.SetRunningLabelText(this.FormatStatusDescription(device.Device.Programs.StatusDescription)); // "Moving";
                                _currentProgramId = device.Device.Programs.CurrentProgramId != string.Empty ? Convert.ToInt32(device.Device.Programs.CurrentProgramId) - 1 : 0;
                                //CurrentProgramId = DataCache.CurrenprogramId-1; 
                                this._lblProgramStatus.Text = StringLiterals.ProgramRunning;// device.Device.MetaData.Device.Programs.Items.Values.ElementAt(CurrentProgramId).Name + " " + ProgramStatus.Running;
                                //  this._lblProgramStatus.Text = program.Name;
                            }
                        }
                        else if (device.Device.Programs.Status.Value == StringLiterals.Waiting)
                        {
                            this._runningImageView.Image = Images.Waiting;
                            this.SetRunningLabelText(this.FormatStatusDescription(device.Device.Programs.StatusDescription)); //device.Device.Programs.Status.Value;
                            this._lblProgramStatus.Text = StringLiterals.ProgramRunning;
                        }
                        else if (device.Device.Programs.Status.Value == StringLiterals.Stopped)
                        {
                            this._runningImageView.Image = Images.Stop;
                            this.SetRunningLabelText(this.FormatStatusDescription(device.Device.Programs.StatusDescription)); //StringLiterals.Stopped;
                            _currentProgramId = device.Device.Programs.CurrentProgramId != string.Empty ? Convert.ToInt32(device.Device.Programs.CurrentProgramId) - 1 : 0;
                            //CurrentProgramId = DataCache.CurrenprogramId-1;
                            this._lblProgramStatus.Text = StringLiterals.ProgramStopped;// device.Device.MetaData.Device.Programs.Items.Values.ElementAt(CurrentProgramId).Name + " " + ProgramStatus.Stopped;
                                                                                        //this._lblProgramStatus.Text = program.Name;
                        }
                        else
                        {
                            this._lblStop.Text = StringLiterals.Stopped;
                            this._runningImageView.Image = Images.Stop;
                            this._lblProgramStatus.Text = "Program Stopped";
                        }
                    }
                    else
                    {
                        this._runningImageView.Hidden = false;
                    }
                    if (device.DirectionStatus)
                    {
                        if (device.DirectionValue == StringLiterals.Forward || device.DirectionValue == "1")

                        {
                            this._lblforward.Text = StringLiterals.Forward;
                            this._forwardImageView.Image = Images.Farrow;

                        }
                        else
                        {
                            this._lblforward.Text = StringLiterals.Reverse;
                            this._forwardImageView.Image = Images.RevImage;

                        }
                    }
                    else
                    {
                        this._lblforward.Text = "";
                    }

                    if (device.WetDryStatus)
                    {
                        if (Device.SpeedStatus)
                        {
                            DeviceSettingValue settingValueObj = null;
                            this.Feature?.Feature?.Setting?.GetValue(out settingValueObj);
                            if (settingValueObj != null)
                            {
                                var settingValueClone = settingValueObj.Clone();
                            }
                            try
                            {
                                var unit = device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                                this._lblSpeed.Text = device.SpeedValue.ToString() + unit;
                            }
                            catch (Exception e)
                            {
                                LogUtility.LogException(e);
                            }

                            this._speedImageView.Image = Images.Speed;
                        }
                        else
                        {
                            this._lblSpeed.Text = device.AppAmountValue.ToString() + "mm";
                            //  this._lblSpeed.Text = Device.Device.CurrentStep.ApplicationAmount.Value.ToString() + "mm";
                            this._speedImageView.Image = Images.Speed;
                        }

                        if (device.WetDryValue == "true" || device.WetDryValue == "1")
                        {
                            this._lblwet.Text = StringLiterals.Wet;
                            this._wetImageView.Image = Images.Wet;
                        }
                        else
                        {
                            this._lblwet.Text = StringLiterals.Dry;
                            this._wetImageView.Image = Images.Dry;
                        }
                    }
                    else
                    {
                        this._lblSpeed.Text = "";
                        this._lblwet.Text = "";
                        this._wetImageView.Image = Images.Dry;
                    }
                    if (device.EndGunStatus)
                    {
                        if (device.EndGunValue == "true" || device.EndGunValue == "1")
                        {

                            this._lblEndGun.Text = StringLiterals.EndGun + StringLiterals.Auto; // "Auto";
                        }
                        else
                        {
                            this._lblEndGun.Text = StringLiterals.EndGun + " " + "Off";
                            //   this._lblEndGun.Text = device.Device.MetaData.Device.Features.Items.Values.FirstOrDefault().Program.Steps.Items["-1"].EndGun.Dictionary.Title.FirstOrDefault().ToString() + "off";
                        }
                    }
                    else
                    {
                        this._lblEndGun.Text = "";
                    }
                    if (device.Aux1Status)
                    {
                        //if ((device.Aux1Value == "true" || device.Aux1Value == "1") && device.Device.MetaData.Device.Features.Items.Values.Count > 0)
                        if (device.Aux1Value == "true" || device.Aux1Value == "1")
                        {
                            try
                            {
                                this._lblAux1.Text = device.Device.MetaData.Device.Features.Items.Values.FirstOrDefault().Program.Steps.Items["-1"].Auxiliary1.Dictionary.Title.FirstOrDefault().ToString() + " On";

                            }
                            catch (Exception)
                            {
                                this._lblAux1.Text = String.Format("{0} {1}", StringLiterals.Aux1, StringLiterals.On);  //"Aux1 On";
                            }

                            this._aux1.Image = Images.PowerOn;
                            this._aux1.SizeToFit();
                        }
                        else
                        {
                            this._lblAux1.Text = String.Format("{0} {1}", StringLiterals.Aux1, StringLiterals.Off);  //"Aux1 Off";
                            this._aux1.Image = Images.PowerOff;
                            this._aux1.SizeToFit();
                        }
                    }
                    else
                    {
                        this._lblAux1.Text = "";
                        this._aux1.Image = Images.PowerOff;
                        this._aux1.SizeToFit();
                    }
                    if (device.Aux2Status)
                    {

                        if (device.Aux2Value == "true" || device.Aux2Value == "1")
                        {
                            try
                            {
                                this._lblAux2.Text = device.Device.MetaData.Device.Features.Items.Values.FirstOrDefault().Program.Steps.Items["-1"].Auxiliary2.Dictionary.Title.FirstOrDefault().ToString() + " On";
                            }
                            catch (Exception)
                            {
                                this._lblAux2.Text = String.Format("{0} {1}", StringLiterals.Aux2, StringLiterals.On);  //"Aux2 On";
                            }

                            this._aux2.Image = Images.PowerOn;
                            this._aux2.SizeToFit();
                        }
                        else
                        {
                            this._lblAux2.Text = String.Format("{0} {1}", StringLiterals.Aux2, StringLiterals.Off);  //"Aux2 Off";

                            this._aux2.Image = Images.PowerOff;
                            this._aux2.SizeToFit();
                        }
                    }
                    else
                    {
                        this._lblAux2.Text = "";
                        this._aux2.Image = Images.PowerOff;
                        this._aux2.SizeToFit();
                    }

                    //this.CurrentStepId = int.Parse(device.Device.Programs.CurrentProgramId);
                    //try
                    //{
                    //    this.CurrentStepId = int.Parse(device.Device.Programs.Items.FirstOrDefault().Value.CurrentStepId);
                    //    this.CurrentProgId = int.Parse(device.Device.Programs.CurrentProgramId);
                    //    //this.CurrentStepId = device.Device.Programs.Items.FirstOrDefault().Value.CurrentStepId;
                    //    this._lblProgramStatus.Text = device.Device.MetaData.Device.Programs.Items.Values.ElementAt(CurrentStepId).Name;

                    //}
                    //catch(Exception e)
                    //{
                    //    this.CurrentStepId = int.Parse(device.Device.Programs.Items.FirstOrDefault().Value.CurrentStepId);
                    //    this.CurrentProgId = int.Parse(device.Device.Programs.CurrentProgramId);
                    //    //this.CurrentStepId = device.Device.Programs.Items.FirstOrDefault().Value.CurrentStepId;
                    //    this._lblProgramStatus.Text = device.Device.MetaData.Device.Programs.Items.Values.ElementAt(CurrentStepId).Name;
                    //}
                    //try
                    //{
                    //    this._lblProgramStatusEdit.Text = Device.Device.MetaData.Device.Programs.Items.Values.ElementAt(CurrentStepId).Steps.Items[CurrentStepId.ToString()].Name + "  Repeat " + device.Device.Programs.Items.Values.ElementAt(CurrentStepId).RepeatNumber.ToString() + " of " + device.Device.Programs.Items.Values.ElementAt(CurrentStepId).NumberOfRepeats.ToString();
                    //}
                    //catch (Exception ex)
                    //{
                    //    this._lblProgramStatusEdit.Text = string.Empty;
                    //    LogUtility.LogException(ex);
                    //}
                });
            }

            public FeatureCellforMap(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._mapView = new MKMapView();

                    //this.mapView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                    this._mapView.ZoomEnabled = false;

                    // Request permission to access device location  
                    _location.RequestWhenInUseAuthorization();

                    // Indicates User Location  
                    this._mapView.ShowsUserLocation = false;
                    this._mapView.ScrollEnabled = false;
                    this._mapView.ZoomEnabled = false;
                    this._mapView.UserInteractionEnabled = false;

                    this._topView.Layer.CornerRadius = 5;
                    this._topView.Layer.BorderColor = UIColor.Black.CGColor;
                    this._topView.BackgroundColor = UIColor.White;
                    this._topView.Alpha = .9f;

                    this._runningImageView.Image = Images.Running;

                    this._lblRunning.SetFrameWidth(100);
                    this._lblRunning.SetFrameHeight(30);
                    this._lblRunning.Font = UIFont.PreferredHeadline;
                    this._lblRunning.Lines = 0;
                    this._lblRunning.LineBreakMode = UILineBreakMode.WordWrap;

                    this._separator1.BackgroundColor = UIColor.Gray;

                    this._forwardImageView.Image = Images.Farrow;

                    this._lblforward.Font = UIFont.PreferredHeadline;

                    this._separator2.BackgroundColor = UIColor.Gray;

                    this._speedImageView.Image = Images.Speed;

                    this._lblSpeed.Font = UIFont.PreferredHeadline;

                    this._lblWatering.Font = UIFont.PreferredCaption2;

                    this._separator3.BackgroundColor = UIColor.Gray;

                    this._lblwet.Font = UIFont.PreferredHeadline;

                    this._lblDry.Font = UIFont.PreferredHeadline;

                    this._separator4.BackgroundColor = UIColor.Gray;

                    this._lblEndGun.Font = UIFont.PreferredHeadline;

                    this._btnNext.Image = Images.Slice;

                    this._lblAux1.Font = UIFont.PreferredHeadline;

                    this._lblAux2.Font = UIFont.PreferredHeadline;

                    this._lblProgramStatus.Font = UIFont.PreferredCallout;

                    this._lblProgramStatusEdit.Font = UIFont.PreferredCaption1;


                    this._topView.AddSubviews(_lblRunning, _runningImageView, _waitingImageView, _lblStop, _stopImageView, _separator1, _forwardImageView,
                        _lblforward, _wetImageView, _dryImageView, _lblwet, _lblDry, _separator2, _separator3,
                        _lblSpeed, _speedImageView, _lblEndGun, _lblAux1, _lblAux2, _lblProgramStatus,
                        _lblProgramStatusEdit, _btnNext, _separator4, _aux2, _aux1);
                    // this.mapView.AddSubviews(this._topView);

                    this.ContentView.AddSubviews(this._mapView);
                    this.ContentView.AddSubviews(this._topView);
                });
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();


                this._mapView.Frame = new CGRect(-150, 0, this.Frame.Width + 150, 390); // hardcoded: 150 is this._topView.Frame.Width that is set below
                                                                                        //this.mapView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                
                this._topView.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 150, 5, 150, 300);

                this._runningImageView.Frame = new CGRect(3, 8, 30, 30);

                this._lblRunning.SetFrameLocation(new CGPoint(this._runningImageView.Frame.Right, 5));

                this._waitingImageView.Frame = new CGRect(this._lblRunning.Frame.Right, 8, 20, 20);

                this._separator1.Frame = new CGRect(0, _lblRunning.Frame.Bottom, _topView.Frame.Width, .5);

                this._forwardImageView.Frame = new CGRect(3, this._separator1.Frame.Bottom + 3, 30, 30);

                this._lblforward.Frame = new CGRect(this._forwardImageView.Frame.Right, this._separator1.Frame.Bottom, 70, 30);

                this._separator2.Frame = new CGRect(0, _lblforward.Frame.Bottom, _topView.Frame.Width, .5);

                this._speedImageView.Frame = new CGRect( /*_forwardImageView.Frame.X _wetImageView.Frame.Right +*/ 2, this._separator2.Frame.Bottom, 35, 35);

                this._lblSpeed.Frame = new CGRect(_speedImageView.Frame.Right, this._separator2.Frame.Bottom, 70, 30);

                this._lblWatering.Frame = new CGRect(_lblSpeed.Frame.Right + 5, this._separator2.Frame.Bottom, 70, 30);

                this._separator3.Frame = new CGRect(0, _lblSpeed.Frame.Bottom, _topView.Frame.Width, .5);

                this._wetImageView.Frame = new CGRect(3, this._separator3.Frame.Bottom + 3, 30, 30);

                this._lblwet.Frame = new CGRect(this._wetImageView.Frame.Right, this._separator3.Frame.Bottom, 70, 30);

                this._dryImageView.Frame = new CGRect(_lblwet.Frame.Right + 1, this._separator3.Frame.Bottom + 5, 25, 25);

                this._lblDry.Frame = new CGRect(this._dryImageView.Frame.Right, this._separator3.Frame.Bottom, 70, 30);

                this._separator4.Frame = new CGRect(0, _lblDry.Frame.Bottom, _topView.Frame.Width, .5);

                this._lblEndGun.Frame = new CGRect(5, this._separator4.Frame.Bottom, 120, 30);

                this._btnNext.Frame = new CGRect(_topView.Bounds.Right - 40, this._separator4.Frame.Bottom + 4, 30, 30);

                this._aux1.Frame = new CGRect(9, _lblEndGun.Frame.Bottom + 7, 20, 30);

                this._lblAux1.Frame = new CGRect(_aux1.Frame.Right + 3, _lblEndGun.Frame.Bottom, 120, 30);

                this._aux2.Frame = new CGRect(9, _lblAux1.Frame.Bottom + 7, 20, 30);

                this._lblAux2.Frame = new CGRect(_aux2.Frame.Right + 3, _lblAux1.Frame.Bottom, 120, 30);

                this._aux1.SizeToFit();

                this._aux2.SizeToFit();

                this._lblProgramStatus.Frame = new CGRect(5, _lblAux2.Frame.Bottom, 190, 30);

                this._lblProgramStatusEdit.Frame = new CGRect(5, _lblProgramStatus.Frame.Bottom, 180, 30);

                if (this._topView.Frame.Height < this._lblProgramStatusEdit.Frame.Bottom)
                {
                    this._topView.SetFrameHeight(this._lblProgramStatusEdit.Frame.Bottom);
                }
            }

            private string FormatStatusDescription(IEnumerable<string> descriptionTexts)
            {
                return ExceptionUtility.Try<string>(() => {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (string s in descriptionTexts)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(s);
                    }

                    string output = sb.ToString();
                    output = output.Replace("Go ", "Moving ");
                    return output;
                });
            }

            private void SetRunningLabelText(string text)
            {
                this._lblRunning.SetFrameWidth(100);
                this._lblRunning.Text = text;  //"Moving to 120 at midnight on the seventeenth of April and what was the name of that place again?";  
                this._lblRunning.SizeToFit();
                if (this._lblRunning.Frame.Height < 30)
                    this._lblRunning.SetFrameHeight(30);
                this._lblRunning.SetFrameWidth(100);
            }
        }

        private class SpecialViewCellMap : FeatureCellforMap
        {
            public const string TableCellKeymm = "SpecialViewCellmm";

            public SpecialViewCellMap(IntPtr handle) : base(handle)
            {
            }

            public override void LoadCellValues(DeviceDetailViewModel device, DeviceFeatureViewModel feature)
            {
                base.LoadCellValues(device, feature);
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();
            }
        }

        public class PivotMapDelegate : MKMapViewDelegate
        {
            //protected UIView _pivotView = new UIView();
            string pId = "PinAnnotation";
            string mId = "PivotAnnotation";

            //private string Direction;
            public DeviceDetailViewModel Device { get; protected set; }
            public PivotMapDelegate(DeviceDetailViewModel dev)
            {
                Device = dev;
            }

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
            {
                MKAnnotationView AnnotationView;
                //this._pivotView = new DrawingView(200, 200, Direction);
                //this._pivotView.Frame = new CGRect(-80, -80, 150, 150);
                //this._pivotView.Layer.BorderWidth = 8;
                //this._pivotView.Layer.BorderColor = UIColor.Gray.CGColor;
                //this._pivotView.Alpha = .9f;
                //this._pivotView.Transform.Translate(-80, -80);
                //this._pivotView.Layer.CornerRadius = this._pivotView.Frame.Width / 2;
                //this._pivotView.Layer.MasksToBounds = true;
                //this._pivotView.UserInteractionEnabled = false;
                if (annotation is MKUserLocation)
                    return null;

                if (annotation is PivotAnnotation)
                {
                    try
                    {
                        AnnotationView = mapView.DequeueReusableAnnotation(mId);

                        if (AnnotationView == null)
                            AnnotationView = new MKAnnotationView(annotation, mId);

                        if (Device.Device.CurrentStep != null && Device.Device.CurrentStep.Direction != null && Device.Device.CurrentStep.Direction.Visible)
                        {
                            if (Device.Device.CurrentStep.Direction.Value == "Forward" || Device.Device.CurrentStep.Direction.Value == "1")
                            {
                                AnnotationView.Image = UIImage.FromFile("Images/CurrentAngleFwd.png");
                                double fromAngle = double.Parse(Device.Device.CurrentAngle.Value);
                                var currentAngle = Math.PI * (fromAngle) / 180.0;
                                AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)currentAngle), 6, -25);
                            }
                            else
                            {
                                AnnotationView.Image = UIImage.FromFile("Images/CurrentAngleRev.png");
                                double fromAngle = double.Parse(Device.Device.CurrentAngle.Value);
                                var currentAngle = Math.PI * (fromAngle) / 180.0;
                                AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)currentAngle), -6, -25);
                            }
                        }
                        else
                        {
                            AnnotationView.Image = UIImage.FromFile("Images/CurrentAngleLine.png");
                            double fromAngle = double.Parse(Device.Device.CurrentAngle.Value);
                            double offset = double.Parse(Device.Device.AngleOffset == null ? "0" : Device.Device.AngleOffset.Value);
                            var currentAngle = Math.PI * (fromAngle + offset) / 180.0;
                            AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)currentAngle), 0, -25);
                        }
                        //AnnotationView.SetFrameHeight(int.Parse(Device.Device.MetaData.Device.LengthMetres.Value) / 10);
                    }

                    catch (Exception e)
                    {
                        AnnotationView = mapView.DequeueReusableAnnotation(mId);

                        //  double FromAngle = 0;
                        AnnotationView.Image = UIImage.FromFile("Images/CurrentAngleLine.png");
                        double fromAngle = double.Parse(Device.Device.CurrentAngle.Value);
                        double offset = double.Parse(Device.Device.AngleOffset == null ? "0" : Device.Device.AngleOffset.Value);
                        var currentAngle = Math.PI * (fromAngle + offset) / 180.0;
                        AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)currentAngle), -6, -25);

                        LogUtility.LogException(e);
                    }
                }
                else if (annotation is NextLineAnnotation)
                {
                    AnnotationView = mapView.DequeueReusableAnnotation(mId);
                    try
                    {
                        if (AnnotationView == null)
                            AnnotationView = new MKAnnotationView(annotation, mId);

                        // AnnotationView.Image = UIImage.FromFile("Images/NextAngle.png");
                        if (Device.Device.CurrentStep?.ToAngle?.Visible ?? false)
                        {
                            AnnotationView.Image = UIImage.FromFile("Images/NextAngle.png");
                        }
                        else
                        {
                            AnnotationView.Image = UIImage.FromFile("Images/bx3.png");
                        }

                        double.TryParse(Device.Device.CurrentStep?.ToAngle?.Value, out double toAngle);
                        var nextAngle = Math.PI * toAngle / 180.0;
                        AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)nextAngle), 0, -26);
                    }
                    catch (Exception)
                    {
                        AnnotationView.Image = UIImage.FromFile("Images/NextAngle.png");

                        double toAngle = 0;
                        var nextAngle = Math.PI * toAngle / 180.0;
                        AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)nextAngle), 0, -26);
                    }
                    //AnnotationView.SetFrameHeight(int.Parse(Device.Device.MetaData.Device.LengthMetres.Value) / 10);
                }
                else if (annotation is ZeroAnnotation)
                {
                    AnnotationView = mapView.DequeueReusableAnnotation(mId);
                    try
                    {
                        if (AnnotationView == null)
                            AnnotationView = new MKAnnotationView(annotation, mId);

                        //AnnotationView.Image = UIImage.FromFile("Images/Zeromark.png");
                        //double ToAngle = double.Parse(Device.Device.CurrentStep == null ? "0" : Device.Device.CurrentStep.ToAngle.Value);
                        //var Zeromark = Math.PI * ToAngle / 180.0;
                        //AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)Zeromark), 0, -62);

                        AnnotationView.Image = UIImage.FromFile("Images/Zeromark.png");
                        double toAngle = double.Parse(Device.Device.AngleOffset == null ? "1" : Device.Device.AngleOffset.Value);
                        var zeromark = Math.PI * toAngle / 180.0;
                        AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)zeromark), 0, -62);

                        //AnnotationView.Image = UIImage.FromFile("Images/CurrentAngleFwd.png");
                        //double FromAngle = double.Parse(Device.Device.CurrentAngle.Value);
                        //var CurrentAngle = Math.PI * (FromAngle) / 180.0;
                        //AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)CurrentAngle), 6, -25);
                        //  AnnotationView.Transform = CGAffineTransform.MakeTranslation(0, -(int.Parse(Device.Device.MetaData.Device.LengthMetres.Value)-10));
                    }
                    catch (Exception)
                    {
                        AnnotationView.Image = UIImage.FromFile("Images/Zeromark.png");
                        double toAngle = 0;
                        var zeromark = Math.PI * toAngle / 180.0;
                        AnnotationView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)zeromark), 0, -62);
                    }
                }
                else
                {
                    AnnotationView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(pId);

                    if (AnnotationView == null)
                        AnnotationView = new MKPinAnnotationView(annotation, pId);

                    ((MKPinAnnotationView)AnnotationView).PinColor = MKPinAnnotationColor.Red;
                    AnnotationView.CanShowCallout = true;
                }
                return AnnotationView;
            }

            public override MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
            {
                var circleOverlay = overlay as MKCircle;
                var circleView = new MKCircleView(circleOverlay);

                if (Device.Device.CurrentStep != null && Device.Device.CurrentStep.Wet != null && Device.Device.CurrentStep.Wet.Visible)
                {
                    if (Device.Device.CurrentStep.Wet.Value == "1" || Device.Device.CurrentStep.Wet.Value == "true")
                    {
                        circleView.FillColor = UIColor.FromRGB(57, 198, 249);
                    }
                    else if (Device.Device.CurrentStep.Wet.Value == "0" || Device.Device.CurrentStep.Wet.Value == "false")
                    {
                        circleView.FillColor = UIColor.FromRGB(93, 163, 40);
                    }
                }
                else if (Device.IsFaultActive)
                {
                    circleView.FillColor = UIColor.Red;
                }
                else
                {
                    circleView.FillColor = UIColor.Gray;

                }

                circleView.Alpha = 0.9f;
                circleView.StrokeColor = UIColor.Gray;

                return circleView;
            }
        }
    }
}


