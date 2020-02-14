// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    [Register("PivotControlViewController")]
    partial class PivotControlViewController
    {
        private readonly UISegmentedControl _UISegmentedControl = new UISegmentedControl();
        private readonly UIScrollView scrollView = new UIScrollView();
        private readonly SwitchView _standByModeSwitch = new SwitchView();
        private readonly SwitchView _endGunSwitch = new SwitchView();
        private readonly SwitchView _aux1Switch = new SwitchView();
        private readonly SwitchView _aux2Switch = new SwitchView();
        private readonly DualLabelSwitchView _autoStopRev = new DualLabelSwitchView();
        private readonly DualLabelSwitchView _wetDrySwitch = new DualLabelSwitchView();
        private readonly DualLabelSwitchView _speedwateringSwitch = new DualLabelSwitchView();
        private readonly Duration _duration = new Duration();
        private readonly AquamonixLabel _standByLabelText = new AquamonixLabel();
        private readonly UIView _standByLabelBackground = new UIView();
        private readonly UIButton _startButton = new UIButton();
        private readonly DualLabelSwitchView _fwdRevSwitchView = new DualLabelSwitchView();

        //Goto Slider View
        private readonly UIView _goToAngleSliderView = new UIView();
        private readonly UISlider _goToAngleSlider = new UISlider();
        private readonly DividerLine _dividerBottomView = new DividerLine();
        private readonly UIView _goToAngleSliderValueBackground = new UIView();
        private readonly AquamonixLabel _goToAngleSliderValue = new AquamonixLabel();
        private readonly UIImageView _degImageView = new UIImageView();
        private readonly AquamonixTextField _degreesTextField = new AquamonixTextField();


        //Speed Slider View
        private readonly UIView _speedSliderView = new UIView();
        private readonly UISlider _speedslider = new UISlider();
        private readonly DividerLine _speeddividerBottomView = new DividerLine();
        private readonly UIView _speedValueBackground = new UIView();
        private readonly AquamonixLabel _speedValue = new AquamonixLabel();
        private readonly AquamonixLabel _unitValue = new AquamonixLabel();
        private readonly AquamonixTextField _percentTextField = new AquamonixTextField();
        //  private readonly UITextField _degreesTextFieldView1 = new UITextField();


        protected override void InitializeViews()
        {
            base.InitializeViews();
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();
        }
    }
}