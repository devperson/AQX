using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.ViewModels;
using CoreGraphics;
using System;
using System.Linq;
using UIKit;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    public partial class PivotControlViewController : ListViewControllerBase
    {
        private static PivotControlViewController _instance;
        private const int LeftMargin = 16;
        private const int RightMargin = 10;

        public DeviceDetailViewModel _device;
        public ProgramDisableFeatureViewModel _feature;
        public readonly UIImage degIcon = UIImage.FromFile("Images/programs1.png");

        protected override nfloat ReconBarVerticalLocation
        {
            get { return Sizes.NotchOffset + NavigationBarView.Height; }
        }


        // todo: fix this class, cannot subscribe to device updates which trigger load data which redraws the view causing glitches
        public PivotControlViewController(DeviceDetailViewModel device, ProgramDisableFeatureViewModel feature, string deviceId) : base() // cannot pass deviceId to base as this will subscribe to deivce updates
        {
            this._feature = feature;
            this._device = device;
            this.Initialize();
        }

        public static PivotControlViewController CreateInstance(DeviceDetailViewModel device, ProgramDisableFeatureViewModel feature, string deviceId)
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }
                _instance = new PivotControlViewController(device, feature, deviceId);

            });

            return _instance;
        }

        protected override void InitializeViewController()
        {
            ExceptionUtility.Try(() =>
            {
                this.AutomaticallyAdjustsScrollViewInsets = false;
                base.InitializeViewController();
                this.SetCustomBackButton();
                this._aux1Switch.SetValue(true);
                //this._standByModeSwitch.SetText(StringLiterals.StandBy);
                //this._standByModeSwitch.OnSwitchChanged = () =>
                //{
                //    this.OnSwitchChanged();
                //};

                this._fwdRevSwitchView.SetDualLabelText(StringLiterals.Fwd, StringLiterals.Rev);
                this._endGunSwitch.SetText(StringLiterals.EndGun);
                this._wetDrySwitch.SetDualLabelText(StringLiterals.Wet, StringLiterals.Dry);
                this._autoStopRev.SetDualLabelText(StringLiterals.AutoStop, StringLiterals.AutoRev);
                this._speedwateringSwitch.SetDualLabelText(StringLiterals.Speed, StringLiterals.Watering);
                this._aux1Switch.SetText(StringLiterals.Aux1);
                this._aux2Switch.SetText(StringLiterals.Aux2);

                //deafult values for controls

                this._goToAngleSlider.Value = (float)(this._device?.ToAngleValue ?? 0);
                this._degreesTextField.Placeholder = Math.Round(this._goToAngleSlider.Value, 0).ToString() + "°";

                if (this._device.SpeedStatus)
                {
                    this._speedwateringSwitch.On = true;
                    this._speedslider.Value = (float)this._device?.Speed;
                    //var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                    this._percentTextField.Placeholder = Math.Round(this._speedslider.Value, 0).ToString() + "%";
                }
                else
                {
                    this._speedwateringSwitch.On = false;
                    this._speedslider.Value = (float)this._device?.AppAmountValue;
                    this._percentTextField.Placeholder = Math.Round(this._speedslider.Value, 0).ToString() + "mm";
                }

                if (this._device.DirectionValue == "Forward" || this._device.DirectionValue == "1")
                {
                    this._fwdRevSwitchView.On = true;
                }
                else
                {
                    this._fwdRevSwitchView.On = false;
                }
                if (this._device.WetDryValue == "true" || this._device.WetDryValue == "1")
                {
                    this._wetDrySwitch.On = true;
                }
                else
                {
                    this._wetDrySwitch.On = false;
                }

                if (_device.EndGunValue == "true")
                {
                    this._endGunSwitch.On = false;
                }
                else
                {
                    this._endGunSwitch.On = true;
                }
                if (_device.Aux1Value == "true" || _device.Aux1Value == "1")
                {
                    this._aux1Switch.On = false;
                }
                else
                {
                    this._aux1Switch.On = true;
                }
                if (_device.Aux2Value == "true" || _device.Aux2Value == "1")
                {
                    this._aux2Switch.On = false;
                }
                else
                {
                    this._aux2Switch.On = true;
                }
                if (_device.AutoReverseValue)
                {
                    this._autoStopRev.On = false;
                }
                else
                {
                    this._autoStopRev.On = true;
                }

                this._wetDrySwitch.OnSwitchChanged = () =>
                {
                    this.WetDrySwitchChanged();
                };
                this._autoStopRev.OnSwitchChanged = () =>
                {
                    this.AutoStopRevSwitchChanged();
                };
                this._fwdRevSwitchView.OnSwitchChanged = () =>
                {
                    this.FwdRevSwitchChanged();
                };
                this._speedwateringSwitch.OnSwitchChanged = () =>
                {
                    this.SpeedwateringSwitchChanged();
                };
                this._endGunSwitch.OnSwitchChanged = () =>
                {
                    this.EndGunSwitChanged();
                };
                this._aux1Switch.OnSwitchChanged = () =>
                {
                    this.Aux1SwitchChanged();
                };
                this._aux2Switch.OnSwitchChanged = () =>
                {
                    this.Aux2SwitchChanged();
                };
            });
        }

        private void AutoStopRevSwitchChanged()
        {
            if (_autoStopRev.On)
            {
                this._device.AutoReverseValue = false;
            }
            else
            {
                this._device.AutoReverseValue = true;
            }
        }

        private void SpeedwateringSwitchChanged()
        {
            //bool conversionsSuccessful = float.TryParse(_device.Device.CurrentStep.Speed.Value, out float speed);
            if (_speedwateringSwitch.On)
            {
                //Switch if off

                // this._speedValue.Text = Math.Round(speed, 0).ToString() + "%";
                this._device.Speed = Math.Round(_speedslider.Value, 0);
                var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + "%";
            }
            else
            {
                //Switch is On
                //this._speedValue.Text = Math.Round(speed, 0).ToString() + "mm";

                this._device.AppAmountValue = Math.Round(_speedslider.Value, 0);
                //_device.AppAmountValue = Math.Round(_speedslider.Value, 0); // isn't this the same as line above?

                var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + "mm";
            }
        }

        private void WetDrySwitchChanged()
        {
            if (_wetDrySwitch.On)
            {
                //Switch if off
                _device.WetDryValue = "true";
            }
            else
            {
                ////Switch is On
                _device.WetDryValue = "false";
            }
        }

        private void FwdRevSwitchChanged()
        {
            if (_fwdRevSwitchView.On)
            {
                //Switch if off
                this._device.DirectionValue = "Forward";
            }
            else
            {
                ////Switch is On
                this._device.DirectionValue = "Reverse";
            }
        }

        private void Aux2SwitchChanged()
        {
            if (this._aux2Switch.On)
            {
                //Switch if off
                this._device.Aux2Value = "false";
            }
            else
            {
                ////Switch is On
                this._device.Aux2Value = "true";
            }
        }

        private void Aux1SwitchChanged()
        {
            if (this._aux1Switch.On)
            {
                //Switch if off
                this._device.Aux1Value = "false";
            }
            else
            {
                ////Switch is On
                this._device.Aux1Value = "true";
            }
        }

        private void EndGunSwitChanged()
        {
            if (this._endGunSwitch.On)
            {
                //Switch if off
                this._device.EndGunValue = "false";
            }
            else
            {
                //Switch is On
                this._device.EndGunValue = "true";
            }
        }

        protected override void DoLoadData()
        {
            base.DoLoadData();
            this.NavigationItem.Title = this._device.Name;
            GenerateView();
        }

        public void DismissKeyboard()
        {
            ExceptionUtility.Try(() =>
            {
                this._degreesTextField.ResignFirstResponder();
                this.HideCancelButton();
            });
        }

        private void ShowCancelButton(bool show = true)
        {
            //TODO: animate this 
            MainThreadUtility.InvokeOnMain(() =>
            {

            });
        }

        private void HideCancelButton()
        {
            this.ShowCancelButton(show: false);
        }
      
        private void GenerateView()
        {
            //TODO: all this should be elsewhere, this is a bit of an abomination

            this.PrimeView.SetFrameY(Sizes.NotchOffset); 
            this.scrollView.SetFrameSize(this.PrimeView.Frame.Width, this.PrimeView.Frame.Height);
            this.scrollView.SetFrameLocation(0, 150);
            this.scrollView.AutoresizingMask = UIViewAutoresizing.All;

            this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 750);//2* this.PrimeView.Frame.Height
            this.scrollView.BackgroundColor = UIColor.White;
            this.scrollView.ContentInset = UIEdgeInsets.Zero;
            this.scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;

            this._UISegmentedControl.Frame = new CGRect((this.PrimeView.Bounds.Width / 2) - 125, 0, 250, 40);
            this._UISegmentedControl.Layer.CornerRadius = 20;
            this._UISegmentedControl.Layer.BorderColor = UIColor.Black.CGColor;
            this._UISegmentedControl.Layer.BorderWidth = 1.0f;
            this._UISegmentedControl.Layer.MasksToBounds = true;
            this._UISegmentedControl.InsertSegment("Go To", 0, true);
            this._UISegmentedControl.InsertSegment("Continuous", 1, true);

            if (this._device.Continuous)
            {
                _UISegmentedControl.SelectedSegment = 1;
                this.Continuous();
            }
            else
            {
                _UISegmentedControl.SelectedSegment = 0;
                this.Goto();
            }
            //this._UISegmentedControl.SelectedSegment = 0;

            this._UISegmentedControl.ValueChanged += HandleControlValueChanged;
            this._UISegmentedControl.CenterHorizontallyInParent();

            //Standby Header Strip
            //StandByLabelBackground
            this._standByLabelBackground.Frame = new CGRect(0, 0, this.PrimeView.Frame.Width, 40);
            this._standByLabelBackground.BackgroundColor = UIColor.FromRGB(229, 227, 227);

            //StandByLabelText
            this._standByLabelText.Frame = new CGRect(0, 0, this.PrimeView.Frame.Width, 40);
            this._standByLabelText.CenterHorizontallyInParent();
            this._standByLabelText.CenterVerticallyInParent();
            this._standByLabelText.TextAlignment = UITextAlignment.Center;
            this._standByLabelText.Text = "Stand By";
            this._standByLabelText.TextColor = UIColor.Black;
            this._standByLabelBackground.AddSubview(_standByLabelText);
            this._standByLabelBackground.Hidden = true;

            //StandBySwitchView
            this._standByModeSwitch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._standByModeSwitch.SetFrameLocation(0, _UISegmentedControl.Frame.Bottom);

            //SliderView for Angle Go To mode
            this._goToAngleSliderView.SetFrameSize(UIScreen.MainScreen.Bounds.Width, 60);
            this._goToAngleSliderView.SetFrameLocation(0, _standByModeSwitch.Frame.Bottom + 10);
            this._goToAngleSliderView.SetFrameY(65);

            //SLider

            this._goToAngleSlider.MinValue = 0;
            this._goToAngleSlider.MaxValue = 360;
            // this._GoToAngleSlider.Continuous=false;
            this._goToAngleSlider.ValueChanged += GoToAngleSlider_ValueChanged;
            this._goToAngleSlider.SetFrameWidth(UIScreen.MainScreen.Bounds.Width - 90 - RightMargin*2 - LeftMargin*2);//(200); // hardcoded 90 is width of _GoToAngleSliderValueBackground
            this._goToAngleSlider.TouchUpInside += GoToAngleSlider_TouchUpOutside;
            this._goToAngleSlider.SetFrameX(LeftMargin);
            this._goToAngleSlider.CenterVerticallyInParent();
            // this._GoToAngleSlider.SetFrameY(-50);

            //ValueBox
            this._goToAngleSliderValue.SizeToFit();
            this._goToAngleSliderValue.TextColor = Colors.StandardTextColor;
            this._goToAngleSliderValue.SetFrameWidth(80);
            this._goToAngleSliderValue.SetFrameHeight(40);
            this._goToAngleSliderValue.SetFrameX(265);
            this._goToAngleSliderValue.TextAlignment = UITextAlignment.Center;

            this._degreesTextField.OnTextChanged += DegreesTextField_OnTextChanged;
            this._degreesTextField.OnDoneClicked += DegreesTextField_OnDoneClicked;

            this._degreesTextField.Frame = new CGRect(6, 1, 70, 37);
            this._degreesTextField.MaxLength = 5;
            this._degreesTextField.AutocorrectionType = UITextAutocorrectionType.Yes;
            this._degreesTextField.AutocapitalizationType = UITextAutocapitalizationType.Words;
            this._degreesTextField.ReturnKeyType = UIReturnKeyType.Done;
            this._degreesTextField.DismissOnButtonClick = true;
            //  this._degreesTextField.SecureTextEntry = false;
            // this._degreesTextField.KeyboardType = UIKeyboardType.NumberPad;
            this._degreesTextField.SetFrameX(UIScreen.MainScreen.Bounds.Width - _goToAngleSliderValue.Frame.Width - RightMargin);
            // this._GoToAngleSliderValue.SetFrameY(-30);
            this._goToAngleSliderValue.CenterVerticallyInParent();
            this._goToAngleSliderValueBackground.BackgroundColor = UIColor.White;
            this._goToAngleSliderValueBackground.Layer.CornerRadius = 5;
            this._goToAngleSliderValueBackground.Layer.BorderColor = UIColor.Black.CGColor;
            this._goToAngleSliderValueBackground.Layer.BorderWidth = 1;
            this._goToAngleSliderValueBackground.SetFrameWidth(90);
            this._goToAngleSliderValueBackground.SetFrameHeight(40);
            this._goToAngleSliderValueBackground.SetFrameX(UIScreen.MainScreen.Bounds.Width - _goToAngleSliderValueBackground.Frame.Width - RightMargin);
            this._goToAngleSliderValueBackground.CenterVerticallyInParent();
            this._dividerBottomView.Frame = new CGRect(0, _goToAngleSliderValueBackground.Frame.Bottom + 15, UIScreen.MainScreen.Bounds.Width, 1);
            this._goToAngleSliderView.AddSubviews(_goToAngleSlider, _goToAngleSliderValueBackground, _degreesTextField, _goToAngleSliderValue, _dividerBottomView);

            //this._GoToAngleSliderValueBackground.SetFrameY(-30);
            //autostop rev for cont mode
            this._autoStopRev.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._autoStopRev.SetFrameLocation(0, _standByModeSwitch.Frame.Bottom);
            //this._AutoStopRev.Hidden = true;

            //duration for standby Mode
            this._duration.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._duration.SetFrameLocation(0, _standByModeSwitch.Frame.Bottom);
            this._duration.Hidden = true;

            this._fwdRevSwitchView.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._fwdRevSwitchView.SetFrameLocation(0, _goToAngleSliderView.Frame.Bottom);

            this._wetDrySwitch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._wetDrySwitch.SetFrameLocation(0, _fwdRevSwitchView.Frame.Bottom);

            this._speedwateringSwitch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._speedwateringSwitch.SetFrameLocation(0, _wetDrySwitch.Frame.Bottom);

            this._speedSliderView.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._speedSliderView.SetFrameLocation(0, _speedwateringSwitch.Frame.Bottom + 10);

            //Slider
            this._speedslider.MinValue = 0;
            this._speedslider.MaxValue = 100;
            // this._speedslider.Continuous = false;
            this._speedslider.ValueChanged += Speedslider_ValueChanged;
            this._speedslider.SetFrameWidth(UIScreen.MainScreen.Bounds.Width - 90 - RightMargin * 2 - LeftMargin * 2);//(200); // hardcoded 90 is width of _speedValueBackground;
            this._speedslider.SetFrameX(LeftMargin);

            this._speedslider.TouchUpInside += Speedslider_TouchUpInside;
            this._speedslider.TouchUpOutside += Speedslider_TouchUpOutside;
            this._speedslider.CenterVerticallyInParent();

            //textField
            this._percentTextField.OnTextChanged += PercentTextField_OnTextChanged;
            this._percentTextField.OnDoneClicked += PercentTextField_OnDoneClicked;

            //ValueBox
            this._speedValue.SizeToFit();
            this._speedValue.TextColor = Colors.StandardTextColor;
            this._speedValue.SetFrameWidth(80);
            this._speedValue.SetFrameHeight(40);
            this._speedValue.TextAlignment = UITextAlignment.Center;
            this._percentTextField.Frame = new CGRect(6, 1, 70, 37);
            this._percentTextField.MaxLength = 5;

            this._percentTextField.AutocorrectionType = UITextAutocorrectionType.No;
            this._percentTextField.AutocapitalizationType = UITextAutocapitalizationType.None;
            this._percentTextField.ReturnKeyType = UIReturnKeyType.Done;
            //this._degreesTextField.SecureTextEntry = false;
            //this._percentTextField.KeyboardType = UIKeyboardType.NumberPad;
            this._percentTextField.DismissOnButtonClick = true;

            this._percentTextField.SetFrameX(UIScreen.MainScreen.Bounds.Width - _goToAngleSliderValue.Frame.Width - RightMargin);
            this._speedValue.CenterVerticallyInParent();

            this._speedValueBackground.BackgroundColor = UIColor.White;
            this._speedValueBackground.Layer.CornerRadius = 5;
            this._speedValueBackground.Layer.BorderColor = UIColor.Black.CGColor; 
            this._speedValueBackground.Layer.BorderWidth = 1;
            this._speedValueBackground.SetFrameWidth(90);
            this._speedValueBackground.SetFrameHeight(40);
            this._speedValueBackground.SetFrameX(UIScreen.MainScreen.Bounds.Width - _speedValueBackground.Frame.Width - RightMargin);
            this._speedValueBackground.CenterVerticallyInParent();
            this._speeddividerBottomView.Frame = new CGRect(0, _speedValueBackground.Frame.Bottom + 15, UIScreen.MainScreen.Bounds.Width, 1);
            this._speedSliderView.AddSubviews(_speedslider, _speedValueBackground, _percentTextField, _speedValue, _speeddividerBottomView);

            this._endGunSwitch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._endGunSwitch.SetFrameLocation(0, _speedSliderView.Frame.Bottom);

            this._aux1Switch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._aux1Switch.SetFrameLocation(0, _endGunSwitch.Frame.Bottom);

            this._aux2Switch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
            this._aux2Switch.SetFrameLocation(0, _aux1Switch.Frame.Bottom);

            this._startButton.Frame = new CGRect((this.PrimeView.Bounds.Width / 2) - 100, _aux2Switch.Frame.Bottom + 15, 200, 50);
            this._startButton.BackgroundColor = UIColor.FromRGB(93, 163, 40);
            this._startButton.Layer.CornerRadius = 25;
            this._startButton.SetTitle("Start/Apply", UIControlState.Normal);
            this._startButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            this._startButton.CenterHorizontallyInParent();
            this._startButton.TouchUpInside += StartButton_TouchUpInside;

            scrollView.AddSubviews(_UISegmentedControl, _endGunSwitch, _fwdRevSwitchView, _aux1Switch, _aux2Switch, _wetDrySwitch, _speedwateringSwitch, _speedSliderView, _goToAngleSliderView, _autoStopRev, _startButton, _duration, _standByLabelBackground);

            this.PrimeView.AddSubviews(scrollView);
        }

        void GoToAngleSlider_TouchUpOutside(object sender, EventArgs e)
        {
            // AlertPopUp();
        }

        void Speedslider_TouchUpOutside(object sender, EventArgs e)
        {
        }

        void Speedslider_TouchUpInside(object sender, EventArgs e)
        {
            // AlertPopUpp();
        }

        private void PercentTextField_OnTextChanged(string obj1)
        {
            if (!string.IsNullOrEmpty(obj1))
            {
                int.TryParse(obj1, out int mynum);
                if (0 <= mynum && mynum <= 100)
                {
                    _speedslider.Value = float.Parse(obj1, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    AlertUtility.ShowAlert("Alert", "Value must be between 0 and 100", "Ok");
                }
            }
            else
            {
                _speedslider.Value = 0.00f;
            }

        }

        private void DegreesTextField_OnTextChanged(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                int.TryParse(obj, out int mynum);
                if (0 <= mynum && mynum <= 360)
                {
                    _goToAngleSlider.Value = float.Parse(obj, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    AlertUtility.ShowAlert("Alert", "Destination must be between 0 to 360", "Ok");
                }
            }
            else
            {
                _goToAngleSlider.Value = 0.00f;
            }
        }

        private void PercentTextField_OnDoneClicked()
        {
            //if (!string.IsNullOrEmpty(obj1))
            //{
            //    int.TryParse(obj1, out mynum);
            //    if (0 <= mynum && mynum <= 100)
            //    {
                    if (_speedwateringSwitch.On)
                    {
                        this._device.Speed = Math.Round(_speedslider.Value, 0);
                        var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                        this._percentTextField.Text = this._device.Speed.ToString() + unit;
                    }
                    else
                    {
                        this._device.AppAmountValue = Math.Round(_speedslider.Value, 0);
                        // var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                        this._percentTextField.Text = this._device.AppAmountValue.ToString() + "mm";
                    }
            //    }
            //}
        }

        private void DegreesTextField_OnDoneClicked()
        {
            this._device.ToAngleValue = Math.Round((_goToAngleSlider.Value % 360), 0);
            this._degreesTextField.Text = Math.Round(_goToAngleSlider.Value, 0).ToString() + "°";
        }

        private void Speedslider_ValueChanged(object sender, EventArgs e)
        {
            if (_speedwateringSwitch.On)
            {
                //this._device.Speed = Math.Round(_speedslider.Value, 0);
                //// this._speedValue.Text = Math.Round(_speedslider.Value, 0).ToString() + "%";
                //var unit = _device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                //this.speedvalue.Text = _device.SpeedValue.ToString() + unit;
                //speedvalue = _speedValue.Text;

                //this._device.Speed = Math.Round(_speedslider.Value, 0);
                //var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                //this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + unit;
                //speedvalue = _percentTextField.Text;

                this._device.Speed = Math.Round(_speedslider.Value, 0);
                var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + unit;

                //var result = _speedslider.Value;
                //if (result % 10 < 5)
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    this._percentTextField.Text = (div * 10).ToString() + "%";
                //    speedvalue = _percentTextField.Text + "%";
                //}
                //else
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    this._percentTextField.Text = ((div * 10) + 10).ToString() + "%";
                //    speedvalue = _percentTextField.Text + "%";
                //}

                ////////////
                //this._device.ToAngleValue = Math.Round(_speedslider.Value, 0);
                //var result = this._device.ToAngleValue;
                //if (result % 10 < 5)
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    sliderValue1 = (div * 10).ToString();
                //    this._percentTextField.Text = sliderValue1 + "%";
                //    speedvalue = _percentTextField.Text + "%";
                //}
                //else
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    sliderValue1 = ((div * 10) + 10).ToString();
                //    this._percentTextField.Text = sliderValue1 + "%";
                //    speedvalue = _percentTextField.Text + "%";
                //}
            }
            else
            {
                //this._device.Speed = Math.Round(_speedslider.Value, 0);
                //this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + "mm";
                //speedvalue = _percentTextField.Text;

                //this._device.Device.CurrentStep.ApplicationAmount.Value = Math.Round(_speedslider.Value, 0).ToString();
                //_device.Device.CurrentStep.ApplicationAmount.Value = Math.Round(_speedslider.Value, 0).ToString();

                //var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                //this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + "mm";
                //speedvalue = _percentTextField.Text;

                this._device.AppAmountValue = Math.Round(_speedslider.Value, 0);
                // var unit = this._device.Device.Features.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Unit;
                this._percentTextField.Text = Math.Round(_speedslider.Value, 0).ToString() + "mm";

                /////////////
                //this._device.ToAngleValue = Math.Round(_speedslider.Value, 0);
                //var result = this._device.ToAngleValue;
                //if (result % 10 < 5)
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    sliderValue1 = (div * 10).ToString();
                //    this._percentTextField.Text = sliderValue1 + "mm";
                //    speedvalue = _percentTextField.Text;
                //}
                //else
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    sliderValue1 = ((div * 10) + 10).ToString();
                //    this._percentTextField.Text = sliderValue1 + "mm";
                //    speedvalue = _percentTextField.Text;
                //}

                //var result = _speedslider.Value;
                //if (result % 10 < 5)
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    this._percentTextField.Text = (div * 10).ToString() + "mm";
                //    GotoAngle = _percentTextField.Text + "mm";
                //}
                //else
                //{
                //    int div = Convert.ToInt16(result / 10);
                //    sliderValue = ((div * 10) + 10).ToString() + "mm";
                //    speedvalue = _percentTextField.Text + "mm";
                //}
            }
            //AlertPopUpp();
        }

        private void GoToAngleSlider_ValueChanged(object sender, EventArgs e)
        {
            this._device.ToAngleValue = Math.Round(_goToAngleSlider.Value, 0);
            this._degreesTextField.Text = Math.Round(_goToAngleSlider.Value, 0).ToString() + "°";
        }

        private void AlertPopUp()
        {
            var okCancelAlertController = UIAlertController.Create("", "This will not affect programs already running", UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create("Confirm", UIAlertActionStyle.Default, alert => { _goToAngleSlider.Enabled = false; _degreesTextField.Enabled = false; }));
            okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => { _goToAngleSlider.Enabled = true; _degreesTextField.Enabled = true; }));
            PresentViewController(okCancelAlertController, true, null);
        }

        private void StartButton_TouchUpInside(object sender, EventArgs e)
        {
            //Here it will be Navigated to Device Detail Page and Start the Pivot
            this.StartPivot();
        }

        private void StartPivot()
        {
            AlertUtility.ShowConfirmationAlert("Start PIVOT", "Do you want to start the PIVOT?", (b) =>
            {
                if (b)
                {
                    this.StartAsync();
                }
            }, StringLiterals.OkButtonText, StringLiterals.CancelButtonText);
        }

        private async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                await ProgressUtility.SafeShow("Starting Pivot...", async () =>
                {
                    var response = await ServiceContainer.PivotService.StartPivotDevice(_device, _device.Id);

                    if (response.IsSuccessful)
                    {
                        NavigationController.PopViewController(true);
                        // this.NavigateTo(DeviceDetailsViewController.CreateInstance(_device.Id));
                        // 
                    }

                    ProgressUtility.Dismiss();
                });
            }
            catch (Exception e)
            {
                ProgressUtility.Dismiss();
                LogUtility.LogException(e);
            }
        }

        private void SetCurrentValue()
        {
            this._standByModeSwitch.SetValue(this._feature.IsEnabled);
            this.OnSwitchChanged();
        }

        private class SwitchView : UIView
        {
            public const int Height = 60;
            private const int LeftMargin = 16;
            private const int RightMargin = 10;

            private readonly AquamonixLabel _textLabel = new AquamonixLabel();
            private readonly UISwitch _onImmediateSwitch = new UISwitch();
            //private readonly DividerLine _dividerTopView = new DividerLine();
            private readonly DividerLine _dividerBottomView = new DividerLine();

            private Action _onSwitchChanged;

            public Action OnSwitchChanged
            {
                get { return this._onSwitchChanged; }
                set { this._onSwitchChanged = WeakReferenceUtility.MakeWeakAction(value); }
            }

            public bool On
            {
                get { return !this._onImmediateSwitch.On; }
                set { this._onImmediateSwitch.On = !value; }
            }

            public SwitchView() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    //label
                    this._textLabel.SizeToFit();
                    this._textLabel.TextColor = Colors.StandardTextColor;

                    //switch
                    this._onImmediateSwitch.On = false;
                    this._onImmediateSwitch.OnTintColor = UIColor.FromRGB(0, 120, 252);
                    this._onImmediateSwitch.ValueChanged += (sender, e) =>
                    {
                        if (this.OnSwitchChanged != null)
                            this.OnSwitchChanged();
                    };

                    this.AddSubviews(
                        this._textLabel,
                        this._onImmediateSwitch,
                        //this._dividerTopView,
                        this._dividerBottomView
                    );
                });
            }

            public void SetText(string text)
            {
                ExceptionUtility.Try(() =>
                {
                    this._textLabel.Text = text;
                    this._textLabel.SizeToFit();
                });
            }

            public void SetValue(bool value)
            {
                this.On = value;
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //label 
                    this._textLabel.SetFrameHeight(20);
                    this._textLabel.SetFrameX(LeftMargin);
                    this._textLabel.CenterVerticallyInParent();

                    //switch
                    this._onImmediateSwitch.AlignToRightOfParent(RightMargin);
                    this._onImmediateSwitch.CenterVerticallyInParent();
                    this._textLabel.EnforceMaxXCoordinate(this._onImmediateSwitch.Frame.X);

                    //top divider
                    //this._dividerTopView.SetFrameLocation(0, 0);
                    //this._dividerTopView.SetSize();

                    //bottom divider
                    this._dividerBottomView.SetFrameHeight(2);
                    this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
                    this._dividerBottomView.SetSize();
                });
            }
        }

        private class DualLabelSwitchView : UIView
        {
            private readonly AquamonixLabel _leftLabel = new AquamonixLabel();
            private readonly AquamonixLabel _rightLabel = new AquamonixLabel();
            private readonly UISwitch _dualLabelSwitch = new UISwitch();
            private readonly DividerLine _dividerBottomView = new DividerLine();

            private Action _onSwitchChanged;
            public Action OnSwitchChanged
            {
                get { return this._onSwitchChanged; }
                set { this._onSwitchChanged = WeakReferenceUtility.MakeWeakAction(value); }
            }

            public bool On
            {
                get { return !this._dualLabelSwitch.On; }
                set { this._dualLabelSwitch.On = !value; }
            }

            public DualLabelSwitchView() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    //fwdlabel
                    this._leftLabel.TextColor = UIColor.Black;
                    this._leftLabel.SizeToFit();
                    this._leftLabel.TextColor = Colors.StandardTextColor;

                    //fwdrevswitch
                    this._dualLabelSwitch.OnTintColor = UIColor.FromRGB(0, 120, 252);
                    this._dualLabelSwitch.ValueChanged += (sender, e) =>
                    {
                        if (this.OnSwitchChanged != null)
                            this.OnSwitchChanged();
                    };

                    // revLabel
                    //this._revLabel.SetFontAndColor(TextFont);
                    this._rightLabel.SizeToFit();
                    this._rightLabel.TextColor = Colors.StandardTextColor;

                    this.AddSubviews(this._leftLabel, this._rightLabel, this._dualLabelSwitch, _dividerBottomView);
                });
            }

            public void SetDualLabelText(string leftLabeltext, string rightLabeltext)
            {
                ExceptionUtility.Try(() =>
                {
                    this._leftLabel.Text = leftLabeltext;
                    this._rightLabel.Text = rightLabeltext;
                    this._leftLabel.SizeToFit();
                    this._rightLabel.SizeToFit();
                });
            }

            public void SetValue(bool value)
            {
                this.On = value;
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //fwdlabel 
                    this._leftLabel.SetFrameHeight(20);
                    this._leftLabel.SetFrameX(LeftMargin);
                    this._leftLabel.CenterVerticallyInParent();

                    //revlabel 
                    this._rightLabel.SetFrameHeight(20);
                    this._rightLabel.SetFrameWidth(100);
                    this._rightLabel.TextAlignment = UITextAlignment.Right;
                    this._rightLabel.AlignToRightOfParent(RightMargin);
                    this._rightLabel.CenterVerticallyInParent();

                    //switch
                    this._dualLabelSwitch.CenterHorizontallyInParent();
                    this._dualLabelSwitch.CenterVerticallyInParent();

                    this._dividerBottomView.SetFrameHeight(2);
                    this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
                    this._dividerBottomView.SetSize();

                });
            }
        }

        private class SliderView : UIView
        {
            private readonly UISlider _speedSlider = new UISlider();
            private readonly DividerLine _dividerBottomView = new DividerLine();
            private readonly UIView _slidervaluebackground = new UIView();
            private readonly UITextField _slidervalue = new UITextField();

            public SliderView() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    //slidervalue label background
                    this._slidervaluebackground.BackgroundColor = UIColor.White;
                    this._slidervaluebackground.Layer.CornerRadius = 5;
                    this._slidervaluebackground.Layer.BorderColor = UIColor.Black.CGColor;
                    this._slidervaluebackground.Layer.BorderWidth = 1;
                    //label
                    this._slidervalue.SizeToFit();
                    this._slidervalue.TextColor = Colors.StandardTextColor;
                    this._slidervalue.Text = "0" + "%";

                    this._speedSlider.MinValue = 10;
                    this._speedSlider.MaxValue = 100;
                    this._speedSlider.ValueChanged += _speedSlider_ValueChanged;

                    this.AddSubviews(
                        this._speedSlider,
                        this._slidervaluebackground,
                        this._dividerBottomView
                    );
                    this._slidervaluebackground.AddSubview(_slidervalue);
                });
            }

            private void _speedSlider_ValueChanged(object sender, EventArgs e)
            {
                _slidervalue.Text = Math.Round(_speedSlider.Value, 0).ToString() + "%";
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();
                    //slider
                    this._speedSlider.SetFrameWidth(200);
                    this._speedSlider.SetFrameX(LeftMargin);
                    this._speedSlider.CenterVerticallyInParent();

                    //text Background
                    this._slidervaluebackground.SetFrameWidth(90);
                    this._slidervaluebackground.SetFrameHeight(40);
                    this._slidervaluebackground.AlignToRightOfParent(RightMargin);
                    this._slidervaluebackground.CenterVerticallyInParent();

                    //slider value Label
                    this._slidervalue.SetFrameWidth(80);
                    this._slidervalue.SetFrameHeight(40);
                    this._slidervalue.TextAlignment = UITextAlignment.Center;
                    this._slidervalue.AlignToRightOfParent(RightMargin);
                    this._slidervalue.CenterVerticallyInParent();

                    //bottom divider
                    this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
                    this._dividerBottomView.SetSize();
                });
            }
        }

        private class Duration : UIView
        {
            private readonly AquamonixLabel _textLabel = new AquamonixLabel();
            //private readonly DividerLine _dividerTopView = new DividerLine();
            private readonly UIView _durationValueBackground = new UIView();
            private readonly DividerLine _dividerBottomView = new DividerLine();
            private readonly UITextField _durationvalue = new UITextField();


            public Duration() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    //label
                    /* this._textLabel.SetFontAndColor(TextFont)*/
                    ;
                    this._textLabel.SizeToFit();
                    this._textLabel.TextColor = Colors.StandardTextColor;
                    this._textLabel.Text = "Standby";

                    //slidervalue label background
                    this._durationValueBackground.BackgroundColor = UIColor.FromRGB(229, 227, 227);
                    this._durationValueBackground.Layer.CornerRadius = 15;
                    this._durationValueBackground.Layer.BorderColor = UIColor.FromRGB(229, 227, 227).CGColor;
                    this._durationValueBackground.Layer.BorderWidth = 1;

                    this._durationvalue.SizeToFit();
                    this._durationvalue.TextColor = Colors.StandardTextColor;
                    this._durationvalue.Text = "55 min";
                    this._durationvalue.ValueChanged += _durationvalue_ValueChanged;

                    this.AddSubviews(
                        this._textLabel,
                        this._durationValueBackground,
                        this._durationvalue,
                        //this._dividerTopView,
                        this._dividerBottomView
                    );
                });
            }

            private void _durationvalue_ValueChanged(object sender, EventArgs e)
            {

            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //label 
                    this._textLabel.SetFrameWidth(100);
                    this._textLabel.SetFrameHeight(20);
                    this._textLabel.SetFrameX(LeftMargin);
                    this._textLabel.CenterVerticallyInParent();

                    //switch
                    this._durationValueBackground.SetFrameWidth(90);
                    this._durationValueBackground.SetFrameHeight(40);
                    this._durationValueBackground.AlignToRightOfParent(RightMargin);
                    this._durationValueBackground.CenterVerticallyInParent();

                    this._durationvalue.SetFrameWidth(80);
                    this._durationvalue.SetFrameHeight(40);
                    this._durationvalue.TextAlignment = UITextAlignment.Center;
                    this._durationvalue.AlignToRightOfParent(RightMargin);
                    this._durationvalue.CenterVerticallyInParent();
                    //top divider
                    //this._dividerTopView.SetFrameLocation(0, 0);
                    //this._dividerTopView.SetSize();

                    //bottom divider
                    this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
                    this._dividerBottomView.SetSize();
                });
            }
        }

        private void OnSwitchChanged()
        {
            ExceptionUtility.Try(() =>
            {
                // goto mode //
                if (_UISegmentedControl.SelectedSegment == 0)
                {
                    if (this._standByModeSwitch.On)
                    {
                        //Switch if off
                        //this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 750);
                        //_duration.Hidden = true;
                        //_StandByLabelBackground.Hidden = true;
                        //_endGunSwitch.Hidden = false;
                        //_FwdRevSwitchView.Hidden = false;
                        //_wetdrySwitch.Hidden = false;
                        //_speedSliderView.Hidden = false;
                        //_speedwateringSwitch.Hidden = false;
                        //_UISegmentedControl.Hidden = false;
                        //_GoToAngleSliderView.Hidden = false;
                        //_UISegmentedControl.Hidden = false;
                        //_Aux1Switch.Hidden = false;
                        //_Aux2Switch.Hidden = false;
                        //_StartButton.Hidden = false;
                        //this._Aux1Switch.SetFrameLocation(0, _endGunSwitch.Frame.Bottom);
                        //this._Aux2Switch.SetFrameLocation(0, _Aux1Switch.Frame.Bottom);
                        //this._StartButton.SetFrameLocation(0, _Aux2Switch.Frame.Bottom + 15);
                        //this._StartButton.CenterHorizontallyInParent();
                    }
                    else
                    {
                        ////Switch is On
                        //this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 350);
                        //_endGunSwitch.Hidden = true;
                        //_FwdRevSwitchView.Hidden = true;
                        //_wetdrySwitch.Hidden = true;
                        //_speedSliderView.Hidden = true;
                        //_AutoStopRev.Hidden = true;
                        //_speedwateringSwitch.Hidden = true;
                        //_UISegmentedControl.Hidden = true;
                        //_GoToAngleSliderView.Hidden = true;
                        //_StandByLabelBackground.Hidden = false;
                        //_duration.Hidden = false;
                        //_Aux1Switch.Hidden = false;
                        //_Aux2Switch.Hidden = false;
                        //_StartButton.Hidden = false;
                        //this._duration.SetFrameLocation(0, _standByModeSwitch.Frame.Bottom);
                        //this._Aux1Switch.SetFrameLocation(0, _duration.Frame.Bottom);
                        //this._Aux2Switch.SetFrameLocation(0, _Aux1Switch.Frame.Bottom);
                        //this._StartButton.CenterHorizontallyInParent();
                    }
                }
                // countinuous mode
                if (_UISegmentedControl.SelectedSegment == 1)
                {
                    if (this._standByModeSwitch.On)
                    {
                        //Switch if off
                        this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 750);
                        _endGunSwitch.Hidden = false;
                        _fwdRevSwitchView.Hidden = false;
                        _wetDrySwitch.Hidden = false;
                        _speedSliderView.Hidden = false;
                        _autoStopRev.Hidden = false;
                        _speedwateringSwitch.Hidden = false;
                        _UISegmentedControl.Hidden = false;
                        _goToAngleSliderView.Hidden = true;
                        _duration.Hidden = true;
                        _standByLabelBackground.Hidden = true;
                        _UISegmentedControl.Hidden = false;
                        _aux1Switch.Hidden = false;
                        _aux2Switch.Hidden = false;
                        _startButton.Hidden = false;
                        this._aux1Switch.SetFrameLocation(0, _endGunSwitch.Frame.Bottom);
                        this._aux2Switch.SetFrameLocation(0, _aux1Switch.Frame.Bottom);
                        this._startButton.SetFrameLocation(0, _aux2Switch.Frame.Bottom + 15);
                        this._startButton.CenterHorizontallyInParent();
                    }
                    else
                    {
                        ////Switch is On
                        this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 350);
                        _endGunSwitch.Hidden = true;
                        _fwdRevSwitchView.Hidden = true;
                        _wetDrySwitch.Hidden = true;
                        _speedSliderView.Hidden = true;
                        _autoStopRev.Hidden = true;
                        _speedwateringSwitch.Hidden = true;
                        _UISegmentedControl.Hidden = true;
                        _goToAngleSliderView.Hidden = true;
                        _aux1Switch.Hidden = false;
                        _aux2Switch.Hidden = false;
                        _startButton.Hidden = false;
                        _standByLabelBackground.Hidden = false;
                        _duration.Hidden = false;
                        this._duration.SetFrameLocation(0, _standByModeSwitch.Frame.Bottom);
                        this._aux1Switch.SetFrameLocation(0, _duration.Frame.Bottom);
                        this._aux2Switch.SetFrameLocation(0, _aux1Switch.Frame.Bottom);
                        this._startButton.CenterHorizontallyInParent();
                    }
                }
            });
        }

        private void ChangeFeatureSettingValue(DeviceSetting newSetting)
        {
            if (this._standByModeSwitch.On)
                this._feature.SettingsValueDictionary = this._feature.EnableDictionary;

            if (this._device.ChangeFeatureSettingValue != null)
                this._device.ChangeFeatureSettingValue(this._device, this._feature, newSetting);

            this.NavigationController.PopViewController(true);
        }

        public void HandleControlValueChanged(object sender, EventArgs e)
        {
            int current = (int)_UISegmentedControl.SelectedSegment;
            if (current == 1)
            {
                this._device.Continuous = true;
                this.Continuous();
            }
            else if (current == 0)
            {
                this._device.Continuous = false;
                this.Goto();

            }
        }

        public void Continuous()
        {
            this.scrollView.SetFrameLocation(0, Sizes.NavigationHeaderHeight + 5);
            this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 750);
            this._goToAngleSliderView.Hidden = true;
            this._autoStopRev.Hidden = false;
            this._duration.Hidden = true;
            this._fwdRevSwitchView.SetFrameLocation(0, _autoStopRev.Frame.Bottom);
            this._wetDrySwitch.SetFrameLocation(0, _fwdRevSwitchView.Frame.Bottom);
            this._speedwateringSwitch.SetFrameLocation(0, _wetDrySwitch.Frame.Bottom);
            this._speedSliderView.SetFrameLocation(0, _speedwateringSwitch.Frame.Bottom + 10);
            this._endGunSwitch.SetFrameLocation(0, _speedSliderView.Frame.Bottom);
            this._aux1Switch.SetFrameLocation(0, _endGunSwitch.Frame.Bottom);
            this._aux2Switch.SetFrameLocation(0, _aux1Switch.Frame.Bottom);
            this._startButton.SetFrameLocation(0, _aux2Switch.Frame.Bottom + 15);
            this._startButton.CenterHorizontallyInParent();
        }

        public void Goto()
        {
            this.scrollView.SetFrameLocation(0, Sizes.NavigationHeaderHeight + 5);
            this.scrollView.ContentSize = new CGSize(this.PrimeView.Frame.Width, 750);
            this._autoStopRev.Hidden = true;
            this._endGunSwitch.Hidden = false;
            this._fwdRevSwitchView.Hidden = false;
            this._wetDrySwitch.Hidden = false;
            this._speedSliderView.Hidden = false;
            this._speedwateringSwitch.Hidden = false;
            this._UISegmentedControl.Hidden = false;
            this._goToAngleSliderView.Hidden = false;
            this._UISegmentedControl.Hidden = false;
            this._aux1Switch.Hidden = false;
            this._duration.Hidden = true;
            this._aux2Switch.Hidden = false;
            this._fwdRevSwitchView.SetFrameLocation(0, _goToAngleSliderView.Frame.Bottom);
            this._wetDrySwitch.SetFrameLocation(0, _fwdRevSwitchView.Frame.Bottom);
            this._speedwateringSwitch.SetFrameLocation(0, _wetDrySwitch.Frame.Bottom);
            this._speedSliderView.SetFrameLocation(0, _speedwateringSwitch.Frame.Bottom + 10);
            this._endGunSwitch.SetFrameLocation(0, _speedSliderView.Frame.Bottom);
            this._aux1Switch.SetFrameLocation(0, _endGunSwitch.Frame.Bottom);
            this._aux2Switch.SetFrameLocation(0, _aux1Switch.Frame.Bottom);
            this._startButton.SetFrameLocation(0, _aux2Switch.Frame.Bottom + 15);
            this._startButton.CenterHorizontallyInParent();
        }

        private class TextChangedEventArgs
        {
        }
    }
}




