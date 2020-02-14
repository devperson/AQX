// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("EnableTimerViewController")]
	partial class EnableTimerViewController
	{
		private static readonly FontWithColor TextFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize9, Colors.StandardTextColor);

		//private readonly UIDatePicker _datePickerView = new UIDatePicker();
		private readonly TimeIntervalPicker _intervalPickerView = new TimeIntervalPicker(30);
		private readonly SwitchView _enabledSwitch = new SwitchView();
		private readonly CheckboxView _disableIndefCheckbox = new CheckboxView();
		private readonly CheckboxView _disableTimeCheckbox = new CheckboxView();
		private readonly NavBarView _navBarView = new NavBarView();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this.PrimeView.AddSubviews(
				//_datePickerView,
				_intervalPickerView,
				_enabledSwitch, 
				_disableIndefCheckbox, 
				_disableTimeCheckbox
			);
		}

		protected override void HandleViewDidLayoutSubviews()
		{
			base.HandleViewDidLayoutSubviews();

			//switch view
			this._enabledSwitch.SetFrameSize(this.PrimeView.Frame.Width, SwitchView.Height);
			this._enabledSwitch.SetFrameLocation(0, Sizes.NavigationHeaderHeight);

			//checkbox 
			this._disableIndefCheckbox.SetFrameSize(this.PrimeView.Frame.Width, CheckboxView.Height);
			this._disableIndefCheckbox.SetFrameLocation(0, this._enabledSwitch.Frame.Bottom);

			//checkbox 
			this._disableTimeCheckbox.SetFrameSize(this.PrimeView.Frame.Width, CheckboxView.Height);
			this._disableTimeCheckbox.SetFrameLocation(0, this._disableIndefCheckbox.Frame.Bottom);

			//date picker 
			//this._datePickerView.SetFrameLocation(0, this._disableTimeCheckbox.Frame.Bottom);
			this._intervalPickerView.SetFrameLocation(0, this._disableTimeCheckbox.Frame.Bottom);
			this._intervalPickerView.SetFrameSize(this.PrimeView.Frame.Width, 216);
		}
	}
}
