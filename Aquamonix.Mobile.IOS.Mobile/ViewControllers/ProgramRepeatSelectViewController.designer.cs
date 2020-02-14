// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using CoreGraphics;

using Aquamonix.Mobile.IOS.Views;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    [Register("ProgramRepeatSelectViewController")]
    partial class ProgramRepeatSelectViewController
    {
        private readonly NavBarView _navBarView = new NavBarView();
        private readonly SummaryHeader _summaryView = new SummaryHeader();
        private UIPickerView _numberPicker;
        private readonly UILabel _repeatText = new UILabel();
        private readonly UIButton _startButton = new UIButton();

        void ReleaseDesignerOutlets()
        {
        }

        protected override void InitializeViews()
        {
            base.InitializeViews();

            _numberPicker = new UIPickerView(
                new CGRect(
                    UIScreen.MainScreen.Bounds.X - UIScreen.MainScreen.Bounds.Width,
                    UIScreen.MainScreen.Bounds.Height - 230,
                    UIScreen.MainScreen.Bounds.Width,
                    180
                )
            );

            //10f, 244f, this.PrimeView.Frame.Width - 20, 240f));
            _numberPicker.BackgroundColor = UIColor.LightTextColor;
            var pickerModel = new StatusPickerViewModel();
            pickerModel.PickerChanged += PickerModel_PickerChanged;
            _numberPicker.Model = pickerModel;
            this._numberPicker.SetFrameLocation(5, 120);


            //Label
            this._repeatText.Frame = new CGRect((UIScreen.MainScreen.Bounds.Width / 2) - 155, (UIScreen.MainScreen.Bounds.Height / 2) - 230, 180, 50);
            this._repeatText.TextColor = UIColor.Black;
            this._repeatText.TextAlignment = UITextAlignment.Left;
            this._repeatText.Text = "Repeats";
            this._startButton.CenterHorizontallyInParent();

            //Button

            this._startButton.Frame = new CGRect((UIScreen.MainScreen.Bounds.Width / 2) - 90, (UIScreen.MainScreen.Bounds.Height / 2) + 230, 200, 50);
            this._startButton.BackgroundColor = UIColor.FromRGB(93, 163, 40);
            this._startButton.Layer.CornerRadius = 25;
            this._startButton.SetTitle("Start", UIControlState.Normal);
            this._startButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            this._startButton.CenterHorizontallyInParent();
            this._startButton.TouchUpInside += StartButton_TouchUpInside;


            this.PrimeView.AddSubviews(
                _navBarView,
                _summaryView,
                _numberPicker,
                _repeatText,
                _startButton
            );
        }
    }
}
