using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Circuits screen.
    /// </summary>
	public partial class CircuitListViewController : ListViewControllerBase
    {
        private static CircuitListViewController _instance;

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


        private CircuitListViewController(string deviceId) : base(deviceId)
        {

        }

        public static CircuitListViewController CreateInstance(string deviceId)
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
                _instance = new CircuitListViewController(deviceId);

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                IEnumerable<CircuitViewModel> circuits = this.GetCircuits();

                this._tableViewController.LoadData(circuits);
                this._navBarView.SetTitle(this.Device.Name);
                this._navFooterView.Device = this.Device;

                this._summaryView.SetMessages(this.Device?.CircuitsFeature?.HeaderSummaryTexts);
                this.HandleViewDidLayoutSubviews();
            });
        }

        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = StringLiterals.CircuitsEmptyMessage; //"This device has no circuits to display at the moment";
            bool isEmpty = true;
            if (this.Device != null)
            {
                if (this.Device.Circuits != null)
                    isEmpty = !this.Device.Circuits.Any();
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
                    foreach (var circuit in this.Device.Circuits)
                    {
                        circuit.SelectedChanged = this.CircuitSelectedChanged;
                    }
                }

                this._selectionHeaderView.OnCancel = CancelSelection;
                this._selectionHeaderView.OnSelectAll = SelectAll;
                this._buttonFooterView.OnTestClicked = LaunchTimerScreen;
                this.SetCustomBackButton();

                this.TableViewController = this._tableViewController;
                this.HeaderView = this._summaryView;

                if (this.ShowNavFooter)
                    this.FooterView = this._navFooterView;
            });
        }

        protected override void GotDeviceUpdate(Device device)
        {
            ExceptionUtility.Try(() =>
            {
                base.GotDeviceUpdate(device);

                if (this.Device != null)
                {
                    foreach (var circuit in this.Device.Circuits)
                    {
                        circuit.SelectedChanged = this.CircuitSelectedChanged;
                    }
                }
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


        private IEnumerable<CircuitViewModel> GetCircuits()
        {
            IEnumerable<CircuitViewModel> circuits = (Device != null ? Device.Circuits : new List<CircuitViewModel>());

            ExceptionUtility.Try(() =>
            {
                var startCircuits = DataCache.GetCommandProgressesSubIds(CommandType.StartCircuits, this.DeviceId);

                foreach (var circuit in circuits)
                {
                    if (startCircuits != null && startCircuits.Contains(circuit.Id))
                        circuit.Starting = true;
                }
            });

            return circuits;
        }

        private void LaunchTimerScreen()
        {
            ExceptionUtility.Try(() =>
            {
                var timerVc = CircuitsTimerViewController.CreateInstance(this.Device, this.TestSelectedCircuits);

                this.NavigateTo(timerVc);
            });
        }

        private void CancelSelection()
        {
            ExceptionUtility.Try(() =>
            {
                foreach (var circuit in this.Device.Circuits)
                    circuit.Selected = false;

                this.ShowHideSelectionHeader(0);
                this.ShowHideButtonFooter(false);
                this.LoadData();
            });
        }

        private void SelectAll()
        {
            ExceptionUtility.Try(() =>
            {
                foreach (var circuit in this.Device.Circuits)
                    circuit.Selected = true;

                this.ShowHideSelectionHeader(this.Device.Circuits.Count());
                this.LoadData();
            });
        }

        private void TestSelectedCircuits(int durationMinutes)
        {
            this.TestSelectedCircuitsConfirmed(durationMinutes);
        }

        private void TestSelectedCircuitsConfirmed(int durationMinutes)
        {
            ExceptionUtility.Try(() =>
            {
                this._buttonFooterView.PermaDisabled = true;
                this._selectionHeaderView.Enabled = false;
                var selectedCircuitIds = this.Device.Circuits.Where((c) => c.Selected).Select((c) => c.Id);

                ProgressUtility.SafeShow("Testing Circuits", () =>
                {
                    var selectedIdsArray = selectedCircuitIds.ToArray();
                    Action<ProgressResponse> callbackHandler = ((p) =>
                    {
                        this.HandleUpdatesForStartCircuits(p, selectedIdsArray, durationMinutes);
                    });

                    ServiceContainer.CircuitService.StartCircuits(
                        this.Device.Device, 
                        selectedCircuitIds, 
                        durationMinutes,
                        handleUpdates: callbackHandler, 
                        onReconnect: () => { this.TestSelectedCircuitsConfirmed(durationMinutes); } ).ContinueWith((r) =>
                    {
                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            this._tableViewController.UpdateCircuitCells((c) =>
                            {
                                bool selected = c.Selected;
                                if (selected)
                                {
                                    c.Starting = true;
                                    c.Selected = false;
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

        private void CircuitSelectedChanged()
        {
            ExceptionUtility.Try(() =>
            {
                int countSelected = Device.Circuits.Where((s) => s.Selected).Count();
                this.ShowHideSelectionHeader(countSelected);
                this.ShowHideButtonFooter(countSelected > 0);
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
                //this.NavigationController.NavigationBar.AddSubview(_headerView);
                else
                    this.SetNavigationBarView(this._selectionHeaderView);
                //this.NavigationController.NavigationBar.AddSubview(_selectionHeader);

                this.NavigationItem.HidesBackButton = show;
            });
        }

        private void ShowHideButtonFooter(bool show)
        {
            ExceptionUtility.Try(() =>
            {
                this._buttonFooterView.Hidden = (!show);
            });
        }

        private void HandleUpdatesForStartCircuits(ProgressResponse response, string[] circuitIds, int durationMinutes)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                if (response != null)
                {
                    DataCache.AddCommandProgress(new CommandProgress(CommandType.StartCircuits, response.Body, circuitIds));

                    if (response.IsFinal)
                    {
                        Action resetCircuits = () =>
                        {
                            foreach (string id in circuitIds)
                            {
                                var circuit = this.Device?.Device?.Circuits.Values.Where((d) => d.Id == id).FirstOrDefault();
                                if (circuit != null)
                                {
                                    circuit.Selected = false;
                                    circuit.Starting = false;
                                }
                            }
                        };

                        if (response.IsSuccessful)
                        {
                            //AlertUtility.ShowAlert("Start Circuits", "Start Circuits");
                            this.RegisterForAsyncUpdate(resetCircuits, 5000);
                        }
                        else
                        {
                            this.RegisterForAsyncUpdate(resetCircuits);

                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }

                        DataCache.TriggerDeviceUpdate(this.DeviceId);

                        int countSelected = Device.Circuits.Where((s) => s.Selected).Count();
                        this._buttonFooterView.PermaDisabled = false;
                        this.ShowHideButtonFooter(countSelected > 0);
                        this._selectionHeaderView.Enabled = true;
                    }
                }

                AlertUtility.ShowProgressResponse("Start Circuits", response);
            });
        }


        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImage = UIImage.FromFile("Images/Circuits.png");

            public NavBarView() : base() { }

            public void SetTitle(string title)
            {
                this.SetTitleAndImage(title, _iconImage);
            }
        }

        private class CircuitListTableViewController : TopLevelTableViewControllerBase<CircuitViewModel, CircuitListTableViewSource>
        {
            public CircuitListTableViewController() : base()
            {
                TableView.RegisterClassForCellReuse(typeof(CircuitListTableViewCell), CircuitListTableViewCell.TableCellKey);
            }

            public void UpdateCircuitCells(Func<CircuitViewModel, bool> updateCircuit)
            {
                this.TableViewSource.UpdateCircuitCells(this.TableView, updateCircuit);
            }

            protected override CircuitListTableViewSource CreateTableSource(IList<CircuitViewModel> values)
            {
                return new CircuitListTableViewSource(values);
            }
        }

        private class CircuitListTableViewSource : TableViewSourceBase<CircuitViewModel>
        {
            public CircuitListTableViewSource(IList<CircuitViewModel> circuits) : base(circuits) { }

            public void UpdateCircuitCells(UITableView tableView, Func<CircuitViewModel, bool> updateCircuit)
            {
                ExceptionUtility.Try(() =>
                {
                    for (int n = 0; n < this.Values.Count; n++)
                    {
                        var circuit = this.Values[n];
                        var cell = tableView.CellAt(NSIndexPath.FromRowSection(n, 0)) as CircuitListTableViewCell;
                        if (cell != null && circuit != null)
                        {
                            if (updateCircuit(circuit))
                                cell.LoadCellValues(circuit);
                        }
                        else if (circuit != null)
                        {
                            updateCircuit(circuit);
                        }
                    }
                });
            }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                CircuitListTableViewCell cell = (CircuitListTableViewCell)tableView.DequeueReusableCell(CircuitListTableViewCell.TableCellKey, indexPath);

                CircuitViewModel circuit = null;
                if (indexPath.Row < Values.Count)
                    circuit = Values[indexPath.Row];

                //create cell style
                if (circuit != null)
                {
                    cell.LoadCellValues(circuit);
                }

                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                ExceptionUtility.Try(() =>
                {
                    var cell = tableView.CellAt(indexPath) as CircuitListTableViewCell;
                    if (cell != null)
                    {
                        cell.ToggleCheckbox();
                    }
                });
            }
        }

        private class CircuitListTableViewCell : NumberedTextTableViewCell
        {
            public const string TableCellKey = "CircuitListTableViewCell";
            private const int TopMargin = 8;

            private readonly RoundCheckBox _checkbox = new RoundCheckBox(Images.BrownCheckboxChecked, Images.BrownCheckboxUnchecked);
            private readonly AquamonixLabel _statusLabel = new AquamonixLabel();
            private CircuitViewModel _circuit;
            private readonly static UIImage _powerIcon = UIImage.FromFile("Images/power.png");
            private readonly UIImageView _powerIconImageView = new UIImageView();

            public CircuitListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._statusLabel.SetFontAndColor(StatusLabelFont);
                    this._statusLabel.Hidden = true;

                    _checkbox.OnCheckedChanged = () =>
                    {
                        if (_circuit != null)
                        {
                            _circuit.Selected = _checkbox.Checked;

                            if (_circuit.SelectedChanged != null)
                                _circuit.SelectedChanged();
                        }
                    };


                    //power icon 
                    _powerIconImageView.Image = _powerIcon;
                    _powerIconImageView.SizeToFit();
                    _powerIconImageView.Hidden = true;

                    this.ContentView.AddSubviews(_checkbox, _statusLabel, _powerIconImageView);
                });
            }

            public void LoadCellValues(CircuitViewModel circuit)
            {
                ExceptionUtility.Try(() =>
                {
                    this._circuit = circuit;

                    if (circuit.Running)
                    {
                        this.SetLine1Font(BoldCellTextFont);
                        this._checkbox.Hidden = true;
                    }
                    else
                    {
                        this.SetLine1Font(NormalCellTextFont);
                        this._checkbox.Hidden = false;
                    }

                    if (circuit.HasFault)
                    {
                        this.SetLine2TextColor(Colors.ErrorTextColor);
                    }
                    else
                    {
                        this.SetLine2TextColor(Colors.StandardTextColor);
                    }

                    if (circuit.HighlightGreen)
                    {
                        this.SetTextColor(Colors.AquamonixGreen);
                    }
                    else
                    {
                        this.SetTextColor(Colors.AquamonixBrown);
                    }

                    this.LoadCellValues(this._circuit.Id, this._circuit.Name, this._circuit.Description);

                    _checkbox.SetChecked(circuit.Selected);

                    if (circuit.Running)
                        this.SetStatus(StringLiterals.Running.ToUpper()); //"RUNNING");
                    else if (circuit.Starting)
                        this.SetStatus(StringLiterals.Starting.ToUpper()); //"STARTING");
                    else
                        this.SetStatus(null);

                    _powerIconImageView.Hidden = (!circuit.DisplayPowerIcon);
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

                this._checkbox.SizeToFit();
                this._checkbox.AlignToRightOfParent(RightMargin);
                this._checkbox.CenterVerticallyInParent();

                //power icon 
                this._powerIconImageView.SetFrameLocation(this.FirstLineLabel.Frame.Right + 6, this.FirstLineLabel.Frame.Y + 9);

                this._statusLabel.AlignToRightOfParent(RightMargin);
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
            private Action _onCancel, _onSelectAll;

            private bool _enabled = true;

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
                        LogUtility.LogMessage("User clicked cancel button (circuits).");
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
                        LogUtility.LogMessage("User clicked select all button (circuits).");
                        if (this.OnSelectAll != null)
                            this.OnSelectAll();
                    };

                    this.BackgroundColor = Colors.AquamonixBrown;

                    this.AddSubviews(_selectedLabel, _cancelButton, _selectAllButton);
                });
            }

            public void SetCountSelected(int count)
            {
                ExceptionUtility.Try(() =>
                {
                    this._selectedLabel.Text = StringLiterals.FormatNumberSelected(count);
                    this._selectedLabel.SizeToFit();
                });
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
            private const int Margin = 16;

            private readonly AquamonixButton _testButton = new AquamonixButton(AquamonixButtonStyle.RoundedSolidColor, Colors.AquamonixBrown);
            private readonly DividerLine _dividerLine = new DividerLine();
            private Action _onTestClicked;
            private bool _permaDisabled = false;
            private bool _enabled = true;

            public Action OnTestClicked
            {
                get { return this._onTestClicked; }
                set { this._onTestClicked = WeakReferenceUtility.MakeWeakAction(value); }
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

                    this._testButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked test button (circuits).");
                        if (this.OnTestClicked != null)
                            this.OnTestClicked();
                    };

                    this.AddSubviews(_testButton, _dividerLine);
                });
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //test button 
                    this._testButton.SetFrameSize(167, Sizes.StandardButtonHeight);
                    this._testButton.CenterInParent();
                });
            }
        }
    }
}

