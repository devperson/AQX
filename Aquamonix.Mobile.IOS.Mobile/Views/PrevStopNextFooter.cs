using System;

using Foundation;
using UIKit;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Footer that shows the prev/stop/next buttons.
    /// </summary>
	public class PrevStopNextFooter : AquamonixView
    {
		private const int ButtonMargin = 6; 

		private bool _enabled = true;
		private DeviceDetailViewModel _device = null;

		private readonly AquamonixButton _prevButton = new AquamonixButton(AquamonixButtonStyle.Prev);
		private readonly AquamonixButton _nextButton = new AquamonixButton(AquamonixButtonStyle.Next);
		private readonly AquamonixButton _stopButton = new AquamonixButton(AquamonixButtonStyle.RoundedSolidColor, UIColor.Red);
		private readonly DividerLine _dividerLine = new DividerLine();
		private readonly UIActivityIndicatorView _stopButtonSpinner = new UIActivityIndicatorView();
		private readonly UIActivityIndicatorView _nextButtonSpinner = new UIActivityIndicatorView();
		private readonly UIActivityIndicatorView _prevButtonSpinner = new UIActivityIndicatorView();

		private Action _onPrevClicked;
		private Action _onNextClicked;
		private Action _onStopClicked;
		private DeviceFeatureViewModel _stopFeature;
		private DeviceFeatureViewModel _nextFeature;
		private DeviceFeatureViewModel _prevFeature;
		private StopNextPrevType _stopNextPrevType = StopNextPrevType.Irrigations;

		public Action OnPrevClicked 
		{
			get { return this._onPrevClicked; }
			set { this._onPrevClicked = WeakReferenceUtility.MakeWeakAction(value); }
		}
		public Action OnNextClicked
		{
			get { return this._onNextClicked; }
			set { this._onNextClicked = WeakReferenceUtility.MakeWeakAction(value); }
		}
		public Action OnStopClicked 
		{
			get { return this._onStopClicked; }
			set { this._onStopClicked = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public DeviceDetailViewModel Device
		{
			get { return _device;}
			set
			{
				ExceptionUtility.Try(() =>
				{
					this._device = value;

					this.ShowStopButton = false;
					this.ShowPrevButton = false;
					this.ShowNextButton = false;

					this.Device.Preving = DataCache.HasPendingCommands(CommandType.DevicePrev, this.Device.Id);
					this.Device.Nexting = DataCache.HasPendingCommands(CommandType.DeviceNext, this.Device.Id);
					this.Device.Stopping = DataCache.HasPendingCommands(CommandType.DeviceStop, this.Device.Id);

					foreach (var feature in _device.Features)
					{
						if (feature.Type == DeviceFeatureTypes.DevicePrevButton )
						{
							this.ShowPrevButton = true;
							this._prevButton.SetTitle(feature.ButtonText.ToUpper(), UIControlState.Normal);
							this._prevButton.SetTitle(String.Empty, UIControlState.Disabled);
							this._prevFeature = feature;

							if (this._device.Preving)
								this.SetButtonMode(this._prevButton, this._prevButtonSpinner, true); 
						}
						else if (feature.Type == DeviceFeatureTypes.DeviceStopButton )
						{
							this.ShowStopButton = true;
							this._stopButton.SetTitle(feature.ButtonText.ToUpper(), UIControlState.Normal);
							this._stopButton.SetTitle(String.Empty, UIControlState.Disabled);
							this._stopFeature = feature;

							if (this._device.Stopping)
								this.SetButtonMode(this._prevButton, this._prevButtonSpinner, true);
						}
						else if (feature.Type == DeviceFeatureTypes.DeviceNextButton )
						{
							this.ShowNextButton = true;
							this._nextButton.SetTitle(feature.ButtonText.ToUpper(), UIControlState.Normal);
							this._nextButton.SetTitle(String.Empty, UIControlState.Disabled);
							this._nextFeature = feature;

							if (this._device.Nexting)
								this.SetButtonMode(this._prevButton, this._prevButtonSpinner, true);
						}
					}

					this.DisableButtonsBasedOnFeature(); 
				});
			}
		}

		public bool ShowStopButton 
		{
			set {
				if (this._stopButton != null)
					this._stopButton.Hidden = (!value);
			}
		}
		public bool ShowNextButton 
		{
			set {
				if (this._nextButton != null)
					this._nextButton.Hidden = (!value);
			}
		}
		public bool ShowPrevButton
		{
			set {
				if (this._prevButton != null)
					this._prevButton.Hidden = (!value);
			}
		}
		public bool Enabled
		{ 
			get
			{
				return this._enabled;
			}
			set
			{
				this._enabled = value;
				this._nextButton.Enabled = value;
				this._prevButton.Enabled = value;
				this._stopButton.Enabled = value;

				this.DisableButtonsBasedOnFeature();
			}
		}

		public PrevStopNextFooter(StopNextPrevType type = StopNextPrevType.Irrigations) : base()
		{
			this._stopNextPrevType = type;

			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = UIColor.White;

				this._stopButton.SetTitle(StringLiterals.Stop.ToUpper(), UIControlState.Normal);
				this._stopButton.SetTitle(StringLiterals.Stop.ToUpper(), UIControlState.Disabled);

				this._stopButton.TouchUpInside += (o, e) =>
				{
					this.StopButtonClicked();
				};

				this._nextButton.TouchUpInside += (o, e) =>
				{
					this.NextButtonClicked();
				};

				this._prevButton.TouchUpInside += (o, e) =>
				{
					this.PrevButtonClicked();
				};

				this._stopButtonSpinner.Hidden = true;
				this._nextButtonSpinner.Hidden = true;
				this._prevButtonSpinner.Hidden = true; 

				this._stopButton.AddSubview(this._stopButtonSpinner);
				this._nextButton.AddSubview(this._nextButtonSpinner);
				this._prevButton.AddSubview(this._prevButtonSpinner); 

				this.AddSubviews(_prevButton, _stopButton, _nextButton, _dividerLine);
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				var imageSize = new CoreGraphics.CGSize(91, 55);

				//prev button 
				this._prevButton.SetFrameSize(imageSize.Width, imageSize.Height);
				this._prevButton.SetFrameX(ButtonMargin);
				this._prevButton.CenterVerticallyInParent();

				//next button 
				this._nextButton.SetFrameSize(imageSize.Width, imageSize.Height);
				this._nextButton.AlignToRightOfParent(ButtonMargin);
				this._nextButton.CenterVerticallyInParent();

				//divider line
				this._dividerLine.SetFrameLocation(0, 0);
				this._dividerLine.SetSize();

				//stop button 
				this._stopButton.SetFrameSize(this.Frame.Size.Width - (ButtonMargin * 4) - (_prevButton.Frame.Width) - (_nextButton.Frame.Width), imageSize.Height);
				this._stopButton.CenterInParent();

				//spinners
				this._stopButtonSpinner.SetFrameSize(this._stopButton.Frame.Height, this._stopButton.Frame.Height);
				this._stopButtonSpinner.CenterInParent(); 

				this._prevButtonSpinner.SetFrameSize(this._stopButtonSpinner.Frame.Size);
				this._prevButtonSpinner.CenterInParent();

				this._nextButtonSpinner.SetFrameSize(this._stopButtonSpinner.Frame.Size);
				this._nextButtonSpinner.CenterInParent();
			});
		}

		private void HandleUpdatesForStop(ProgressResponse response)
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				if (response != null)
				{
					DataCache.AddCommandProgress(new CommandProgress(CommandType.DeviceStop, response.Body));

					if (response.IsFinal)
                    {
						this.Device.Stopping = false;
						this.SetButtonMode(this._stopButton, this._stopButtonSpinner, false);

                        if (response.IsSuccessful)
						{
						}
						else
                        {
                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }
					}
				}

				AlertUtility.ShowProgressResponse(this._stopFeature.ProgressText, response);
			});
		}

		private void HandleUpdatesForNext(ProgressResponse response)
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				if (response != null)
				{
					DataCache.AddCommandProgress(new CommandProgress(CommandType.DeviceNext, response.Body));

					if (response.IsFinal)
					{
						this.Device.Nexting = false;
						this.SetButtonMode(this._nextButton, this._nextButtonSpinner, false);

						if (response.IsSuccessful)
						{
						}
						else
                        {
                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }
					}
				}
			                               
				AlertUtility.ShowProgressResponse(this._nextFeature.ProgressText, response);
			});
		}

		private void HandleUpdatesForPrev(ProgressResponse response)
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				if (response != null)
				{
					DataCache.AddCommandProgress(new CommandProgress(CommandType.DevicePrev, response.Body));

					if (response.IsFinal)
					{
						this.Device.Preving = false;
						this.SetButtonMode(this._prevButton, this._prevButtonSpinner, false);

						if (response.IsSuccessful)
						{
						}
						else
                        {
                            if ((bool)!response?.IsReconnectResponse && (bool)!response?.IsServerDownResponse)
                                AlertUtility.ShowAppError(response?.ErrorBody);
                        }
					}
				}

				AlertUtility.ShowProgressResponse(this._prevFeature.ProgressText, response);
			});
		}

		private void SetButtonMode(UIButton button, UIActivityIndicatorView spinner, bool active)
		{
			button.Enabled = !active;
			spinner.Hidden = !active;

			if (active)
			{
				spinner.StartAnimating();
			}
			else {
				spinner.StopAnimating(); 
			}

			this.DisableButtonsBasedOnFeature(); 
		}

		private void DisableButtonsBasedOnFeature()
		{
			/*
			if (this._nextFeature != null && !this._nextFeature.Editable)
				this._nextButton.Enabled = false;

			if (this._prevFeature != null && !this._prevFeature.Editable)
				this._prevButton.Enabled = false;

			if (this._stopFeature != null && !this._stopFeature.Editable)
				this._stopButton.Enabled = false;
			*/
		}

		private void StopButtonClicked()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("User clicked STOP button (footer).");
				string deviceId = this.Device?.Id;
				if (!String.IsNullOrEmpty(deviceId))
				{
					AlertUtility.ShowConfirmationAlert(this._stopFeature.ProgressText, _stopFeature.PromptText, (b) =>
					{
						if (b)
						{
							this.StopConfirmed();
						}
					}, okButtonText: _stopFeature.PromptConfirm, cancelButtonText: _stopFeature.PromptCancel);
				}
			});
		}

		private void NextButtonClicked()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("User clicked NEXT button (footer).");
				string deviceId = this.Device?.Id;
				if (!String.IsNullOrEmpty(deviceId))
				{
					AlertUtility.ShowConfirmationAlert(this._nextFeature.ProgressText, _nextFeature.PromptText, (b) =>
					{
						if (b)
						{
							this.NextConfirmed();
						}
					}, okButtonText: _nextFeature.PromptConfirm, cancelButtonText: _nextFeature.PromptCancel);
				}
			});
		}

		private void PrevButtonClicked()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("User clicked PREV button (footer).");
				string deviceId = this.Device?.Id;
				if (!String.IsNullOrEmpty(deviceId))
				{
					AlertUtility.ShowConfirmationAlert(this._prevFeature.ProgressText, _prevFeature.PromptText, (b) =>
					{
						if (b)
						{
							this.PrevConfirmed(); 
						}
					}, okButtonText: _prevFeature.PromptConfirm, cancelButtonText: _prevFeature.PromptCancel);
				}
			});
		}

		private void StopConfirmed()
		{
			ExceptionUtility.Try(() =>
			{
				this.SetButtonMode(this._stopButton, this._stopButtonSpinner, true);
				this._device.Stopping = true;
				ProgressUtility.SafeShow(this._stopFeature.ProgressText, () =>
				{
					if (this._stopNextPrevType == StopNextPrevType.Irrigations)
					{
						ServiceContainer.DeviceService.StopIrrigations(this.Device.Device, this.HandleUpdatesForStop).ContinueWith((r) =>
						{
							MainThreadUtility.InvokeOnMain(() =>
							{
								ProgressUtility.Dismiss();
							});
						});
					}

					else if (this._stopNextPrevType == StopNextPrevType.Circuits)
					{
						ServiceContainer.CircuitService.StopCircuits(this.Device.Device, this.HandleUpdatesForStop).ContinueWith((r) =>
						{
							MainThreadUtility.InvokeOnMain(() =>
							{
								ProgressUtility.Dismiss();
							});
						});
					}

					if (this.OnStopClicked != null)
						this.OnStopClicked();
				},true);
			}); 
		}

		private void NextConfirmed()
		{
			ExceptionUtility.Try(() =>
			{
				this._device.Nexting = true;
				this.SetButtonMode(this._nextButton, this._nextButtonSpinner, true);
				ProgressUtility.SafeShow(this._nextFeature.ProgressText, () =>
				{
					if (this._stopNextPrevType == StopNextPrevType.Irrigations)
					{
						ServiceContainer.DeviceService.NextIrrigations(this.Device.Device, this.HandleUpdatesForNext).ContinueWith((r) =>
						{
							MainThreadUtility.InvokeOnMain(() =>
							{
								ProgressUtility.Dismiss();
							});
						});
					}
					else if (this._stopNextPrevType == StopNextPrevType.Circuits)
					{
						//Nothing to do here!
					}

					if (this.OnStopClicked != null)
						this.OnStopClicked();
				});
			});
		}

		private void PrevConfirmed()
		{
			ExceptionUtility.Try(() =>
			{
				this._device.Preving = true;
				this.SetButtonMode(this._prevButton, this._prevButtonSpinner, true);
				ProgressUtility.SafeShow(this._prevFeature.ProgressText, () =>
				{
					if (this._stopNextPrevType == StopNextPrevType.Irrigations)
					{
						ServiceContainer.DeviceService.PrevIrrigations(this.Device.Device, this.HandleUpdatesForPrev).ContinueWith((r) =>
						{
							MainThreadUtility.InvokeOnMain(() =>
							{
								ProgressUtility.Dismiss();
							});
						});
					}
					else if (this._stopNextPrevType == StopNextPrevType.Circuits)
					{
						//Nothing to do here!
					}

					if (this.OnStopClicked != null)
						this.OnStopClicked();
				});
			});
		}


		public enum StopNextPrevType
		{
			Irrigations,
			Circuits
		}
	}
}
