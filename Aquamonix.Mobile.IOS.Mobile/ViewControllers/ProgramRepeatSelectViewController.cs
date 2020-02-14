using System;
using System.Diagnostics.Contracts;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.ViewModels;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    public partial class ProgramRepeatSelectViewController : ListViewControllerBase
    {
        private static ProgramRepeatSelectViewController _instance;
        private bool _startPivot;
        private ProgramViewModel _programStart;
        private int _selectedRepeats = 0;

        private bool ShowNavFooter
        {
            get
            {
                return true;
            }
        }

        private class NavBarView : NavigationBarViewWithIcon
        {
            public NavBarView() : base() { }

            public void SetTitle(string title)
            {
                this.SetTitleAndImage(title);
            }
        }

        private ProgramRepeatSelectViewController(string deviceId, bool start, ProgramViewModel program) : base(deviceId)
        {
            _startPivot = start;
            _programStart = program;
        }

        public static ProgramRepeatSelectViewController CreateInstance(string deviceId, bool start, ProgramViewModel program)
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
                _instance = new ProgramRepeatSelectViewController(deviceId, start, program);

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();

                this._navBarView.SetTitle(this._programStart.Name);
                this._summaryView.SetMessages(this.Device?.ProgramsFeature?.HeaderSummaryTexts);
                this._summaryView.SizeToFit();
            });
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
                this.NavigationBarView = this._navBarView;
                this.HeaderView = this._summaryView;
                this.SetCustomBackButton();
            });
        }

        private void StartButton_TouchUpInside(object sender, EventArgs e)
        {
            ExceptionUtility.Try(() =>
            {
                string title = _startPivot ? StringLiterals.StartStationConfirmationTitle : StringLiterals.StopProgramConfirmationTitle;
                AlertUtility.ShowConfirmationAlert(title, StringLiterals.FormatStartStopProgramConfirmationMessage(_startPivot, _programStart.Name, _programStart.Id), (b) =>
                {
                    if (b)
                    {
                        StopStartProgramConfirmed(_programStart, _startPivot);
                    }
                    else
                    {
                        var cell = this.GetCell(_programStart) as ProgramListTableViewCell;
                        if (cell != null)
                        {
                            _programStart.Starting = false;
                            cell.LoadCellValues(_programStart);
                        }
                    }
                }, okButtonText: _startPivot ? StringLiterals.Start : StringLiterals.Stop);
            });
        }

        private void PickerModel_PickerChanged(object sender, int e)
        {
            _selectedRepeats = e;
        }

        private ProgramListTableViewCell GetCell(ProgramViewModel program)
        {
            ProgramListTableViewCell output = null;

            return output;
        }

        private void StopStartProgramConfirmed(ProgramViewModel program, bool start)
        {
            try
            {
                if (this.Device?.Device != null)
                {
                    program.Starting = true;

                    if (start)
                    {
                        ProgressUtility.SafeShow("Starting Programs", () =>
                        {
                            ServiceContainer.ProgramService.StartPivotProgram(this.Device.Device, _programStart.Id, _selectedRepeats,
                                handleUpdates: (r) => { this.HandleUpdatesForStopStartProgram(r, program, start); },
                                onReconnect: () => { StopStartProgramConfirmed(program, start); }).ContinueWith((r) =>
                            {
                                MainThreadUtility.InvokeOnMain(() =>
                                {
                                    ProgressUtility.Dismiss();
                                    Device.Device.Programs.CurrentProgramId = _programStart.Id;
                                    DataCache.CurrentProgramId = Convert.ToInt32(_programStart.Id);
                                    this.NavigationController?.PopViewController(true);
                                });
                            });
                        }, blockUI: true);
                    }
                }
                else
                {
                    LogUtility.LogMessage("Cannot start pivot program because device is null");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
                ProgressUtility.Dismiss();
                Device.Device.Programs.CurrentProgramId = _programStart.Id;
                this.NavigationController.PopViewController(true);
                System.Diagnostics.Debug.WriteLine("Error " + ex.Message);
                this.NavigationController.PopViewController(true);
            }
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
                            ServiceContainer.DeviceService.RequestDevice(this.Device.Device);
                        };

                        if (response.IsSuccessful)
                        {
                            AlertUtility.ShowProgressResponse("Stop/Start Program " + program.Id, response);
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



        private class ProgramListTableViewCell
        {
            public object Program { get; internal set; }

            public void LoadCellValues(ProgramViewModel program)
            {
                this.Program = program;
            }
        }
    }
}

