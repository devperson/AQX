using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.ViewModels;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    public partial class PivotProgramsViewController : ListViewControllerBase
    {
        private readonly UIButton _setButton = new UIButton();
        private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
        private static PivotProgramsViewController _instance;
        private int _program = 0;

        private bool ShowNavFooter
        {
            get
            {
                return true;
            }
        }

        public UIColor BackgroundColor { get; private set; }

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


        private PivotProgramsViewController(string deviceId) : base(deviceId)
        {
         
        }

        public static PivotProgramsViewController CreateInstance(string deviceId)
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
                _instance = new PivotProgramsViewController(deviceId);

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();

                var pendingCommands = DataCache.GetCommandProgressesSubIds(CommandType.StartPrograms, this.DeviceId);
                foreach (var p in Device.Programs)
                {
                    p.StopStartProgram = this.StopStartProgram;
                    if (pendingCommands != null)
                    {
                        if (pendingCommands.Contains(p.Id))
                            p.Starting = true;
                    }
                }

                this._tableViewController.LoadData(Device.Programs);

                // this._navBarView.SetTitle("Program");
                this._navBarView.SetTitle(this.Device.Name);

                this._navFooterView.Device = this.Device;

                this._summaryView.SetMessages(this.Device?.ProgramsFeature?.HeaderSummaryTexts);
                this.HandleViewDidLayoutSubviews();
            });
        }
       
        protected override bool ScreenIsEmpty(out string emptyMessage)
        {
            emptyMessage = StringLiterals.ProgramsEmptyMessage;
            bool isEmpty = true;
            if (this.Device != null)
            {
                if (this.Device.PivotPrograms != null)
                    isEmpty = !this.Device.PivotPrograms.Any();
            }

            return isEmpty;
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();
            // this._intervalPickerView.SetFrameLocation(0, this._intervalPickerView.Frame.Bottom);
            // this._intervalPickerView.SetFrameSize(this.PrimeView.Frame.Width, 216);

            _summaryView.SizeToFit();
        }

        protected override void InitializeViewController()
        {
            base.InitializeViewController();

            ExceptionUtility.Try(() =>
            {
                this._navFooterView.Hidden = (!this.ShowNavFooter);

                this.NavigationBarView = this._navBarView;
                this.TableViewController = this._tableViewController;
                this.HeaderView = this._summaryView;
                this.SetCustomBackButton();

                if (this.ShowNavFooter)
                    this.FooterView = _navFooterView;
            });
        }

        protected override void GotDeviceUpdate(Device device)
        {
            ExceptionUtility.Try(() =>
            {
                base.GotDeviceUpdate(device);

                if (this.Device != null)
                {
                    foreach (var p in this.Device.Programs)
                        p.StopStartProgram = this.StopStartProgram;
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


        private void StopStartProgram(ProgramViewModel program, bool start)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.NavigateTo(ProgramRepeatSelectViewController.CreateInstance(this.Device?.Id, start, program));
            });
        }

        private void StartButton_TouchUpInside(object sender, int e)
        {
            _program = e;
        }

        private void StopStartProgramConfirmed(ProgramViewModel program, bool start)
        {
            ExceptionUtility.Try(() =>
            {
                program.Starting = true;
                var c = this._tableViewController.GetCell(program) as PivotProgramListTableViewCell;
                c.LoadCellValues(program);

                if (start)
                {
                    ProgressUtility.SafeShow("Starting Programs", () =>
                    {
                        ServiceContainer.ProgramService.StartPivotProgram(this.Device.Device, program.Id, (r) => { this.HandleUpdatesForStopStartProgram(r, program, start); }).ContinueWith((r) =>
                        {
                            MainThreadUtility.InvokeOnMain(() =>
                            {
                                ProgressUtility.Dismiss();
                            });
                        });
                    }, blockUI: true);
                    this.NavigationController.PopViewController(true);
                }
                else
                {
                    ProgressUtility.SafeShow("Stopping Programs", () =>
                    {
                        ServiceContainer.ProgramService.StopProgram(this.Device.Device, program.Id, (r) => { this.HandleUpdatesForStopStartProgram(r, program, start); }).ContinueWith((r) =>
                        {
                            MainThreadUtility.InvokeOnMain(() =>
                            {
                                ProgressUtility.Dismiss();
                            });
                        });
                    }, blockUI: true);
                }
            });
        }

        private void HandleUpdatesForStopStartProgram(ProgressResponse response, ProgramViewModel program, bool start)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                if (response != null)
                {
                    DataCache.AddCommandProgress(new CommandProgress(CommandType.StartPrograms, response.Body, new string[] { program.Id }));

                    if (response.IsFinal)
                    {
                        Action resetProgram = () =>
                        {
                            program.Starting = false;
                            //ServiceContainer.DeviceService.RequestDevice(this.Device.Device);
                        };

                        if (response.IsSuccessful)
                        {
                            //AlertUtility.ShowProgressResponse("Stop/Start Program " + program.Id, response);
                            this.RegisterForAsyncUpdate(resetProgram, 5000);
                        }
                        else
                        {
                            this.RegisterForAsyncUpdate(resetProgram);

                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }
                    }
                }
            });
        }



        private class NavBarView : NavigationBarViewWithIcon
        {
            private static UIImage _iconImage = UIImage.FromFile("Images/programs.png");

            public NavBarView() : base() { }

            public void SetTitle(string title)
            {
                this.SetTitleAndImage(title, _iconImage);
            }
        }

        private class ProgramListTableViewController : TopLevelTableViewControllerBase<ProgramViewModel, ProgramListTableViewSource>
        {
            public ProgramListTableViewController() : base()
            {
                TableView.ScrollEnabled = true;
                TableView.LayoutIfNeeded();
                TableView.RegisterClassForCellReuse(typeof(PivotProgramListTableViewCell), PivotProgramListTableViewCell.TableCellKey);
            }

            public PivotProgramListTableViewCell GetCell(ProgramViewModel program)
            {
                PivotProgramListTableViewCell output = null;
                TableView.ScrollEnabled = true;
                TableView.LayoutIfNeeded();

                for (int n = 0; n < this.Values.Count(); n++)
                {
                    var cell = this.TableView.CellAt(NSIndexPath.FromRowSection(n, 0)) as PivotProgramListTableViewCell;
                    if (cell != null)
                    {
                        if (Object.ReferenceEquals(cell.Program, program))
                        {
                            output = cell;
                            break;
                        }
                    }
                }

                return output;
            }

            protected override ProgramListTableViewSource CreateTableSource(IList<ProgramViewModel> values)
            {
                return new ProgramListTableViewSource(values);
            }
        }

        private class ProgramListTableViewSource : TableViewSourceBase<ProgramViewModel>
        {
            public ProgramListTableViewSource(IList<ProgramViewModel> programs) : base(programs) { }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                string firstElementOfList = Values[0].Name == null ? string.Empty : Values[0].Name.Replace(" ", string.Empty);
                nfloat height = 0f;

                if (firstElementOfList.Trim().ToLower() == "manualprogram")
                {
                    if (indexPath.Row == 0)
                    {
                        return 0f;
                    }

                    else
                    {
                        var calculateHeight = Values[indexPath.Row]._program.Steps;
                        if (calculateHeight != null)
                        {
                            height = calculateHeight.Count * 100;
                        }
                        else
                        {
                            height = 100;
                        }
                    }
                }
                else
                {
                    var calculateHeight = Values[indexPath.Row]._program.Steps;

                    if (calculateHeight != null)
                    {
                        height = calculateHeight.Count * 100;
                    }
                    else
                    {
                        height = 100;
                    }
                }

                return height;
            }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.ScrollEnabled = true;

                PivotProgramListTableViewCell cell = (PivotProgramListTableViewCell)tableView.DequeueReusableCell(PivotProgramListTableViewCell.TableCellKey, indexPath);

                ProgramViewModel program = null;
                string FirstElementOfList = Values[0].Name == null ? string.Empty : Values[0].Name.Replace(" ", string.Empty);
                if (FirstElementOfList.Trim().ToLower() == "manualprogram")
                {
                    if (indexPath.Row > 0 && indexPath.Row < Values.Count)
                        program = Values[indexPath.Row];
                }
                else
                {
                    if (indexPath.Row < Values.Count)
                        program = Values[indexPath.Row];
                }

                //create cell style
                if (program != null)
                {
                    if (program.Id != "0")
                    {
                        cell.LoadCellValues(program);
                    }
                }

                return cell;
            }
        }

        private class PivotProgramListTableViewCell : NumberedTextTableViewCells
        {
            public const string TableCellKey = "ProgramListTableViewCell";

            private readonly static UIImage _clockIcon = UIImage.FromFile("Images/clock.png");
            private readonly UIButton _startStopButton = new UIButton();
            private readonly UIImageView _clockIconImageView = new UIImageView();
            private readonly UILabel lblname = new UILabel();

            public ProgramViewModel Program { get; private set; }

            public PivotProgramListTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    //start/stop button 
                    this._startStopButton.Font = StartStopButtonFont;
                    this._startStopButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
                    this._startStopButton.TouchUpInside += (o, e) =>
                    {
                        LogUtility.LogMessage("User clicked start/stop button (programs).");
                        if (this.Program != null && this.Program.StopStartProgram != null)
                        {
                            bool start = (!this.Program.Running);
                            this._startStopButton.Enabled = false;
                            this.LoadCellValues(this.Program);

                            this.Program.StopStartProgram(this.Program, start);
                        }
                    };

                    //clock icon 
                    _clockIconImageView.Image = _clockIcon;
                    _clockIconImageView.SizeToFit();
                    _clockIconImageView.Hidden = true;

                    this.ContentView.AddSubviews(_startStopButton, _clockIconImageView, lblname);
                });
            }

            public void LoadCellValues(ProgramViewModel program)
            {
                this.Program = program;
                this.LoadCellValues(program.Id, program.Name, program.Description, program._program.Steps);
                this.LayoutSubviews();

                this.ContentView.AddSubviews(_startStopButton, _clockIconImageView, lblname);
                this.SetLine1Font(CellLine1StandardFont);

                if (program.Starting) //TODO: replace with IsUpdatingStatus
                {
                    this._startStopButton.SetTitle(StringLiterals.Starting.ToUpper(), UIControlState.Disabled);
                    this._startStopButton.SetTitleColor(Colors.StandardTextColor, UIControlState.Disabled);
                    this._startStopButton.Enabled = false;
                    this._startStopButton.Frame = new CoreGraphics.CGRect(0, 10, 65, 20);
                    this._startStopButton.AlignToRightOfParent(13);
                    
                }
                else
                {
                    if (program.Running)
                    {
                        this.SetLine1Font(CellLine1GreenFont);
                        if (Aquamonix.Mobile.Lib.Environment.AppSettings.ProgramStopEnabled)
                        {
                            this._startStopButton.SetTitle(StringLiterals.StopButtonText, UIControlState.Normal);
                            this._startStopButton.SetTitleColor(Colors.ErrorTextColor, UIControlState.Normal);
                            this._startStopButton.Enabled = true;
                            this._startStopButton.Frame = new CoreGraphics.CGRect(0, 10, 65, 20);
                            this._startStopButton.AlignToRightOfParent(13);
                           
                        }
                        else
                        {
                            this._startStopButton.SetTitle(StringLiterals.Running.ToUpper(), UIControlState.Disabled);
                            this._startStopButton.SetTitleColor(Colors.StandardTextColor, UIControlState.Disabled);
                            this._startStopButton.Enabled = false;
                            this._startStopButton.Frame = new CoreGraphics.CGRect(0, 10, 65, 20);
                            this._startStopButton.AlignToRightOfParent(13);
                        }
                    }
                    else
                    {
                        this._startStopButton.SetTitle(StringLiterals.StartButtonText.ToUpper(), UIControlState.Normal);
                        this._startStopButton.SetTitleColor(Colors.AquamonixGreen, UIControlState.Normal);
                        this._startStopButton.Enabled = true;
                        this._startStopButton.Frame = new CoreGraphics.CGRect(0, 10, 65, 20);
                        this._startStopButton.AlignToRightOfParent(13);
                        
                    }
                }

                _clockIconImageView.Hidden = (!program.Scheduled);               
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                //stop/start button 
                this._startStopButton.SetFrameWidth(65);
                this._startStopButton.SetFrameHeight(20);
                this._startStopButton.Frame = new CoreGraphics.CGRect(0, 10, 65, 20);
                this._startStopButton.AlignToRightOfParent(13);
                //clock icon 
                this._clockIconImageView.SetFrameLocation(this.FirstLineLabel.Frame.Right + 3, this.FirstLineLabel.Frame.Y + 9);

                // this.FirstLineLabel.EnforceMaxXCoordinate(this._startStopButton.Frame.X - 12);
                // this.SecondLineLabel.EnforceMaxXCoordinate(this._startStopButton.Frame.X - 12);
            }
        }
    }
}