using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Stations screen. 
    /// </summary>
	public partial class StationListViewController : ListViewControllerBase
    {
        private static StationListViewController _instance;

        private bool ShowNavFooter
        {
            get
            {
                return (Device.SupportsIrrigationStop || Device.SupportsIrrigationPrev || Device.SupportsIrrigationNext);
            }
        }

        protected override nfloat ReconBarVerticalLocation
        {
            get
            {
                var output = (_summaryView.IsDisposed) ? 0 : _summaryView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            }
        }


        private StationListViewController(string deviceId) : base(deviceId)
        {
        }

        public static StationListViewController CreateInstance(string deviceId)
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
                _instance = new StationListViewController(deviceId);

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                IEnumerable<StationViewModel> stations = this.GetStations();

                this._tableViewController.LoadData(stations);
                this._summaryView.SetMessages(this.Device?.StationsFeature?.HeaderSummaryTexts);  //new string[] { "Stations 1 & 5 active" });
                this._navBarView.SetTitle(this.Device.Name);
                this._navFooterView.Device = this.Device;
                this.HandleViewDidLayoutSubviews();
            });
        }

        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = StringLiterals.StationsEmptyMessage;
            bool isEmpty = true;
            if (this.Device != null)
            {
                if (this.Device.Stations != null)
                    isEmpty = !this.Device.Stations.Any();
            }

            return isEmpty;
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();

            this._summaryView.SizeToFit();
            this._buttonFooterView.SetFrameSize(this.PrimeView.Frame.Width, Sizes.FooterHeight);
            this._buttonFooterView.SetFrameLocation(0, this.PrimeView.Frame.Bottom - _buttonFooterView.Frame.Height);
        }

        protected override void InitializeViewController()
        {
            base.InitializeViewController();


            ExceptionUtility.Try(() =>
            {
                this._navFooterView.Hidden = (!this.ShowNavFooter);

                if (this.Device != null)
                {
                    foreach (var station in this.Device.Stations)
                    {
                        station.SelectedChanged = this.StationSelectedChanged;
                    }
                }

                this._selectionHeaderView.OnCancel = CancelSelection;
                this._selectionHeaderView.OnSelectAll = SelectAll;

                this._buttonFooterView.OnTestClicked = TestSelectedStations;
                this._buttonFooterView.OnWateringClicked = LaunchTimerScreen;

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


        private IEnumerable<StationViewModel> GetStations()
        {
            IEnumerable<StationViewModel> stations = (Device != null ? Device.Stations : new List<StationViewModel>());

            ExceptionUtility.Try(() =>
            {
                var startStations = DataCache.GetCommandProgressesSubIds(CommandType.StartStations, this.DeviceId);
                var testStations = DataCache.GetCommandProgressesSubIds(CommandType.TestStations, this.DeviceId);

                foreach (var station in stations)
                {
                    if (startStations != null && startStations.Contains(station.Id))
                        station.Starting = true;
                    else if (testStations != null && testStations.Contains(station.Id))
                        station.Starting = true;
                }
            });

            return stations;
        }

        private void CancelSelection()
        {
            ExceptionUtility.Try(() =>
            {
                foreach (var station in this.Device.Stations)
                    station.Selected = false;

                this.ShowHideSelectionHeader(0);
                this.ShowHideButtonFooter(0);
                this.LoadData();
            });
        }

        private void SelectAll()
        {
            ExceptionUtility.Try(() =>
            {
                foreach (var station in this.Device.Stations)
                    station.Selected = true;

                this.ShowHideSelectionHeader(this.Device.Stations.Count());
                this.LoadData();
            });
        }

        private void TestSelectedStations()
        {
            ExceptionUtility.Try(() =>
            {
                IEnumerable<string> stationNames, stationNumbers;
                this.GetSelectedStationsList(out stationNames, out stationNumbers);
                //var selectedStationIds = this.Device.Stations.Where((s) => s.Selected).Select((s) => s.Id); 

                AlertUtility.ShowConfirmationAlert(StringLiterals.TestStationConfirmationTitle, StringLiterals.FormatTestStationConfirmationMessage(stationNames, stationNumbers), (b) =>
                {
                    if (b)
                    {
                        this.TestSelectedStationsConfirmed();
                    }
                }, okButtonText: StringLiterals.TestButtonText);
            });
        }

        private void WaterSelectedStations(IEnumerable<PumpViewModel> pumps, int durationMinutes)
        {
            ExceptionUtility.Try(() =>
            {
                IEnumerable<string> stationNames, stationNumbers;
                this.GetSelectedStationsList(out stationNames, out stationNumbers);

                AlertUtility.ShowConfirmationAlert(StringLiterals.StartStationConfirmationTitle, StringLiterals.FormatStartStationConfirmationMessage(stationNames, stationNumbers), (b) =>
                {
                    if (b)
                    {
                        this.WaterSelectedStationsConfirmed(pumps, durationMinutes);
                    }
                }, okButtonText: StringLiterals.StartButtonText);
            });
        }

        private void WaterSelectedStationsConfirmed(IEnumerable<PumpViewModel> pumps, int durationMinutes)
        {
            ExceptionUtility.Try(() =>
            {
                IEnumerable<string> stationNames, stationNumbers;
                this.GetSelectedStationsList(out stationNames, out stationNumbers);
                var selectedStationIds = this.Device.Stations.Where((s) => s.Selected).Select((s) => s.Id);

                var pumpIds = new List<string>();
                if (pumps != null)
                    pumpIds = pumps.Select((p) => p.Id)?.ToList();

                this._buttonFooterView.PermaDisabled = true;
                this._selectionHeaderView.Enabled = false;

                ProgressUtility.SafeShow("Starting Stations", () =>
                {
                    var selectedIdsArray = selectedStationIds.ToArray();
                    Action<ProgressResponse> callbackHandler = ((p) =>
                    {
                        this.HandleUpdatesForStartStations(p, pumps, durationMinutes, selectedIdsArray);
                    });


                    ServiceContainer.StationService.StartStations(
                        this.Device.Device, 
                        selectedStationIds, 
                        pumpIds, 
                        durationMinutes,
                        handleUpdates: callbackHandler, 
                        onReconnect:() => { this.WaterSelectedStationsConfirmed(pumps, durationMinutes); }).ContinueWith((r) =>
                    {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            this._tableViewController.UpdateStationCells((s) =>
                            {
                                bool selected = s.Selected;
                                if (selected)
                                {
                                    s.Starting = true;
                                    s.Selected = false;
                                }

                                return selected;
                            });

                            this._selectionHeaderView.Enabled = true;
                            ProgressUtility.Dismiss();
                        });
                    });
                }, blockUI: true);
            });
        }

        private void TestSelectedStationsConfirmed()
        {
            ExceptionUtility.Try(() =>
            {
                IEnumerable<string> stationNames, stationNumbers;
                this.GetSelectedStationsList(out stationNames, out stationNumbers);
                var selectedStationIds = this.Device.Stations.Where((s) => s.Selected).Select((s) => s.Id);

                this._buttonFooterView.PermaDisabled = true;
                this._selectionHeaderView.Enabled = false;

                ProgressUtility.SafeShow("Testing Stations", () =>
                {
                    var selectedIdsArray = selectedStationIds.ToArray();
                    Action<ProgressResponse> callbackHandler = ((p) =>
                    {
                        this.HandleUpdatesForTestStations(p, selectedIdsArray);
                    });

                    ServiceContainer.StationService.TestStations(
                        this.Device.Device, 
                        selectedStationIds, 
                        0,
                        handleUpdates: callbackHandler,
                        onReconnect: () => { TestSelectedStationsConfirmed(); }).ContinueWith((r) => {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            this._tableViewController.UpdateStationCells((s) =>
                            {
                                bool selected = s.Selected;
                                if (selected)
                                {
                                    s.Selected = false;
                                    s.Starting = true;
                                }

                                return selected;
                            });
                            this._selectionHeaderView.Enabled = true;
                            ProgressUtility.Dismiss();
                        });
                    });
                }, blockUI: true);
            });
        }

        private void LaunchTimerScreen()
        {
            ExceptionUtility.Try(() =>
            {
                var timerVc = WateringTimerViewController.CreateInstance(this.Device, this.WaterSelectedStations);

                this.NavigateTo(timerVc);
            });
        }

        private void GetSelectedStationsList(out IEnumerable<string> stationNames, out IEnumerable<string> stationNumbers)
        {
            var namesList = new List<string>();
            var numbersList = new List<string>();

            stationNames = namesList;
            stationNumbers = numbersList;

            foreach (var s in this.Device.Stations)
            {
                if (s.Selected)
                {
                    namesList.Add(s.Name);
                    numbersList.Add(s.Id);
                }
            }
        }

        private void StationSelectedChanged()
        {
            ExceptionUtility.Try(() =>
            {
                int countSelected = Device.Stations.Where((s) => s.Selected).Count();
                this.ShowHideSelectionHeader(countSelected);
                this.ShowHideButtonFooter(countSelected);
            });
        }

        private void ShowHideSelectionHeader(int countSelected)
        {
            ExceptionUtility.Try(() =>
            {
                bool show = (countSelected > 0);
                this._selectionHeaderView.Hidden = (!show);
                this._navBarView.Hidden = show;
                this._selectionHeaderView.SetCountSelected(countSelected);

                if (this._selectionHeaderView.Hidden)
                    this.SetNavigationBarView(this._navBarView);
                else
                    this.SetNavigationBarView(this._selectionHeaderView);

                this.NavigationItem.HidesBackButton = show;
            });
        }

        private void ShowHideButtonFooter(int countSelected)
        {
            ExceptionUtility.Try(() =>
            {
                this._buttonFooterView.Hidden = (countSelected <= 0);
                this._buttonFooterView.ShowTestButton = (this.Device.StationsFeature.Variation != "MpG");
                this._buttonFooterView.EnableDisableTestButton((countSelected == 1));
                this._buttonFooterView.EnableDisableWateringButton((countSelected > 0));
            });
        }

        private void HandleUpdatesForStartStations(ProgressResponse response, IEnumerable<PumpViewModel> pumps, int durationMinutes, string[] stationIds)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                if (response != null)
                {
                    DataCache.AddCommandProgress(new CommandProgress(CommandType.StartStations, response.Body, stationIds));

                    if (response.IsFinal)
                    {
                        Action resetStations = () =>
                        {
                            foreach (string id in stationIds)
                            {
                                var station = this.Device?.Device?.Stations.Values.Where((d) => d.Id == id).FirstOrDefault();
                                if (station != null)
                                {
                                    station.Selected = false;
                                    station.Starting = false;
                                }
                            }
                        };

                        if (response.IsSuccessful)
                        {
                            //AlertUtility.ShowAlert("Starting Stations", "Starting Stations");
                            this.RegisterForAsyncUpdate(resetStations, 5000);
                        }
                        else
                        {
                            this.RegisterForAsyncUpdate(resetStations);

                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }

                        DataCache.TriggerDeviceUpdate(this.DeviceId);
                    }
                }
            });
        }

        private void HandleUpdatesForTestStations(ProgressResponse response, string[] stationIds)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                if (response != null)
                {
                    DataCache.AddCommandProgress(new CommandProgress(CommandType.TestStations, response.Body, stationIds));

                    if (response.IsFinal)
                    {
                        Action resetStations = () =>
                        {
                            foreach (string id in stationIds)
                            {
                                var station = this.Device?.Device?.Stations.Values.Where((d) => d.Id == id).FirstOrDefault();
                                if (station != null)
                                {
                                    station.Selected = false;
                                    station.Starting = false;
                                }
                            }
                        };

                        if (response.IsSuccessful)
                        {
                            //AlertUtility.ShowAlert("Testing Stations", "Testing Stations");
                            this.RegisterForAsyncUpdate(resetStations, 5000);
                        }
                        else
                        {
                            this.RegisterForAsyncUpdate(resetStations);

                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }

                        DataCache.TriggerDeviceUpdate(this.DeviceId);

                        int countSelected = Device.Stations.Where((s) => s.Selected).Count();
                        this._buttonFooterView.PermaDisabled = false;
                        this.ShowHideButtonFooter(countSelected);
                        this._selectionHeaderView.Enabled = true;

                        /*
						this.LoadData();
						*/
                    }
                }
            });
        }

        protected override void GotDeviceUpdate(Device device)
        {
            ExceptionUtility.Try(() =>
            {
                base.GotDeviceUpdate(device);

                if (this.Device != null)
                {
                    foreach (var station in this.Device.Stations)
                    {
                        station.SelectedChanged = this.StationSelectedChanged;
                    }
                }
            });
        }


        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImage = UIImage.FromFile("Images/stations.png");

            public NavBarView() : base() { }

            public void SetTitle(string title)
            {
                this.SetTitleAndImage(title, _iconImage);
            }
        }

        private class StationListTableViewController : TopLevelTableViewControllerBase<StationViewModel, StationListTableViewSource>
        {
            public StationListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(StationListTableViewCell), StationListTableViewCell.TableCellKey);
            }

            public void UpdateStationCells(Func<StationViewModel, bool> updateStation)
            {
                this.TableViewSource.UpdateStationCells(this.TableView, updateStation);
            }

            protected override StationListTableViewSource CreateTableSource(IList<StationViewModel> values)
            {
                return new StationListTableViewSource(values);
            }
        }

        private class StationListTableViewSource : TableViewSourceBase<StationViewModel>
        {
            public StationListTableViewSource(IList<StationViewModel> stations) : base(stations) { }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                ExceptionUtility.Try(() =>
                {
                    var cell = tableView.CellAt(indexPath) as StationListTableViewCell;
                    if (cell != null)
                    {
                        cell.ToggleCheckbox();
                    }
                });
            }

            public void UpdateStationCells(UITableView tableView, Func<StationViewModel, bool> updateStation)
            {
                ExceptionUtility.Try(() =>
                {
                    for (int n = 0; n < this.Values.Count; n++)
                    {
                        var station = this.Values[n];
                        var cell = tableView.CellAt(NSIndexPath.FromRowSection(n, 0)) as StationListTableViewCell;
                        if (cell != null && station != null)
                        {
                            if (updateStation(station))
                                cell.LoadCellValues(station);
                        }
                        else if (station != null)
                        {
                            updateStation(station);
                        }
                    }
                });
            }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                StationListTableViewCell cell = (StationListTableViewCell)tableView.DequeueReusableCell(StationListTableViewCell.TableCellKey, indexPath);

                StationViewModel station = null;
                if (indexPath.Row < Values.Count)
                    station = Values[indexPath.Row];

                //create cell style
                if (station != null)
                {
                    cell.LoadCellValues(station);
                }

                return cell;
            }
        }

        private class StationListTableViewCell : NumberedTextTableViewCell
        {
            public const string TableCellKey = "StationListTableViewCell";

            private readonly RoundCheckBox _checkbox = new RoundCheckBox(Images.GreenCheckboxChecked, Images.GreenCheckboxUnchecked);
            private readonly UIView _colorView = new UIView();
            private readonly AquamonixLabel _statusLabel = new AquamonixLabel();
            private StationViewModel _station;

            public StationListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._statusLabel.SetFontAndColor(StatusLabelFont);
                    this._statusLabel.Hidden = true;

                    this._colorView.SetFrameWidth(5);
                    this._colorView.SetFrameLocation(0, 0);

                    this._checkbox.OnCheckedChanged = () =>
                    {
                        if (_station != null)
                        {
                            this._station.Selected = this._checkbox.Checked;

                            if (this._station.SelectedChanged != null)
                                this._station.SelectedChanged();
                        }
                    };

                    this.ContentView.AddSubviews(_checkbox, _colorView, _statusLabel);
                });
            }

            public void LoadCellValues(StationViewModel station)
            {
                ExceptionUtility.Try(() =>
                {
                    this._station = station;

                    if (station.Running)
                    {
                        this.SetLine1Font(BoldCellTextFont);
                        this._checkbox.Hidden = true;
                    }
                    else
                    {
                        this.SetLine1Font(NormalCellTextFont);
                        this._checkbox.Hidden = false;
                    }

                    this.LoadCellValues(station.Id, station.Name, station.Description);
                    _checkbox.SetChecked(station.Selected);

                    if (station.ZoneColor == null)
                        _colorView.BackgroundColor = UIColor.White;
                    else
                        _colorView.BackgroundColor = Colors.FromHex(station.ZoneColor.Value);

                    if (station.Running)
                        this.SetStatus(StringLiterals.Running.ToUpper());
                    else if (station.Starting) //TODO: replace with IsUpdatingStatus
                        this.SetStatus(StringLiterals.Starting.ToUpper());
                    else
                        this.SetStatus(null);
                });
            }

            public void ToggleCheckbox()
            {
                //if (!this._checkbox.Hidden)
                this._checkbox.SetChecked(!_checkbox.Checked);

                if (!this._statusLabel.Hidden)
                {
                    this._checkbox.Hidden = (!this._checkbox.Checked);
                    this.HandleLayoutSubviews();
                }
            }

            private void SetStatus(string status)
            {
                if (String.IsNullOrEmpty(status))
                {
                    this._statusLabel.Hidden = true;
                }
                else
                {
                    this._statusLabel.Hidden = false;
                    this._statusLabel.Text = status;
                    this._statusLabel.SizeToFit();
                }

                this._checkbox.Hidden = !this._statusLabel.Hidden;
                this.HandleLayoutSubviews();
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                this._colorView.SetFrameHeight(this.ContentView.Frame.Height);

                this._checkbox.SizeToFit();

                this._checkbox.AlignToRightOfParent(RightMargin);
                this._checkbox.CenterVerticallyInParent();

                this._statusLabel.AlignToRightOfParent(RightMargin);
                this._statusLabel.CenterVerticallyInParent();

                if (!this._checkbox.Hidden)
                    this._statusLabel.SetFrameX(this._statusLabel.Frame.X - this._checkbox.Frame.Width - 3);

                this._statusLabel.CenterVerticallyInParent();
            }
        }

        private class SelectionHeader : NavigationBarView
        {
            public const int Margin = 16;

            private readonly UIButton _cancelButton = new UIButton();
            private readonly UIButton _selectAllButton = new UIButton();
            private readonly AquamonixLabel _selectedLabel = new AquamonixLabel();
            private bool _enabled = true;
            private Action _onCancel;
            private Action _onSelectAll;

            public Action OnCancel
            {
                get { return this._onCancel; }
                set { this._onCancel = WeakReferenceUtility.MakeWeakAction(value); }
            }
            public Action OnSelectAll
            {
                get { return this._onSelectAll; }
                set { this._onSelectAll = WeakReferenceUtility.MakeWeakAction(value); }
            }

            public bool Enabled
            {
                get { return this._enabled; }
                set
                {
                    this._enabled = value;
                    this._cancelButton.Enabled = value;
                    this._selectAllButton.Enabled = value;
                }
            }

            public SelectionHeader() : base(fullWidth: true)
            {
                ExceptionUtility.Try(() =>
                {
                    //cancel button 
                    this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Normal);
                    this._cancelButton.SetFontAndColor(NavHeaderFont);
                    this._cancelButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked cancel button (stations).");
                        if (this.OnCancel != null)
                            this.OnCancel();
                    };

                    //selected label 
                    this._selectedLabel.SetFontAndColor(NavHeaderBoldFont);

                    //select all button 
                    this._selectAllButton.SetTitle(StringLiterals.SelectAllButtonText, UIControlState.Normal);
                    this._selectAllButton.SetFontAndColor(NavHeaderFont);
                    this._selectAllButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked select all button (stations).");
                        if (this.OnSelectAll != null)
                            this.OnSelectAll();
                    };

                    this.BackgroundColor = Colors.AquamonixGreen;

                    this.AddSubviews(_selectedLabel, _cancelButton, _selectAllButton);
                });
            }

            public void SetCountSelected(int count)
            {
                this._selectedLabel.Text = StringLiterals.FormatNumberSelected(count);
                this._selectedLabel.SizeToFit();
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //cancel button 
                    this._cancelButton.SizeToFit();
                    this._cancelButton.SetFrameHeight(Height);
                    this._cancelButton.SetFrameLocation(Margin, 0);

                    //select all button 
                    this._selectAllButton.SizeToFit();
                    this._selectAllButton.SetFrameHeight(Height);
                    this._selectAllButton.AlignToRightOfParent(Margin);
                    this._selectAllButton.SetFrameY(0);

                    //selection label 
                    this._selectedLabel.SizeToFit();
                    this._selectedLabel.SetFrameHeight(Height);
                    this._selectedLabel.SetFrameY(0);
                    this._selectedLabel.CenterInParent();
                    this._selectedLabel.EnforceMaxXCoordinate(this._selectAllButton.Frame.Left);
                });
            }
        }

        private class SelectionButtonFooter : UIView
        {
            private const int Margin = 14;

            private readonly AquamonixButton _testButton = new AquamonixButton(AquamonixButtonStyle.RoundedSolidColor, Colors.AquamonixBrown);
            private readonly AquamonixButton _wateringButton = new AquamonixButton(AquamonixButtonStyle.RoundedSolidColor, Colors.AquamonixGreen);
            private readonly DividerLine _dividerLine = new DividerLine();
            private bool _enabled = true;
            private bool _permaDisabled = false;
            private Action _onTestClicked;
            private Action _onWateringClicked;

            public Action OnTestClicked
            {
                get { return this._onTestClicked; }
                set { this._onTestClicked = WeakReferenceUtility.MakeWeakAction(value); }
            }
            public Action OnWateringClicked
            {
                get { return this._onWateringClicked; }
                set { this._onWateringClicked = WeakReferenceUtility.MakeWeakAction(value); }
            }

            public bool Enabled
            {
                get { return this._enabled; }
                set
                {
                    if (this.PermaDisabled)
                        value = false;

                    this._enabled = value;
                    this._testButton.Enabled = value;
                    this._wateringButton.Enabled = value;
                }
            }

            public bool PermaDisabled
            {
                get
                {
                    return this._permaDisabled;
                }
                set
                {
                    this._permaDisabled = value;
                    this.Enabled = !value;
                }
            }

            public bool ShowTestButton
            {
                set
                {
                    if (this._testButton != null)
                        this._testButton.Hidden = (!value);
                }
            }

            public SelectionButtonFooter() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    this.BackgroundColor = UIColor.White;

                    this._testButton.SetTitle(StringLiterals.TestButtonText.ToUpper(), UIControlState.Normal);
                    this._wateringButton.SetTitle(StringLiterals.WateringButtonText.ToUpper(), UIControlState.Normal);

                    this._testButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked test button (stations).");
                        if (this.OnTestClicked != null)
                            this.OnTestClicked();
                    };

                    this._wateringButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked watering button (stations).");
                        if (this.OnWateringClicked != null)
                            this.OnWateringClicked();
                    };

                    this.AddSubviews(_testButton, _wateringButton, _dividerLine);
                });
            }

            public void EnableDisableTestButton(bool enabled)
            {
                this._testButton.Enabled = enabled;
            }

            public void EnableDisableWateringButton(bool enabled)
            {
                this._wateringButton.Enabled = enabled;
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //test button 
                    this._testButton.SetFrameSize(this.Frame.Width / 2 - (Margin * 2), Sizes.StandardButtonHeight);
                    this._testButton.SetFrameX(Margin);
                    this._testButton.CenterVerticallyInParent();

                    //watering button
                    this._wateringButton.SetFrameSize(this._testButton.Frame.Width, Sizes.StandardButtonHeight);
                    this._wateringButton.SetFrameLocation(this.Frame.Width / 2 + (Margin), _testButton.Frame.Y);

                    //divider line 
                    this._dividerLine.SetFrameLocation(0, 0);
                    this._dividerLine.SetSize();
                });
            }
        }
    }
}

