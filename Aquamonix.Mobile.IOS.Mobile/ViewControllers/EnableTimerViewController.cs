using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    public partial class EnableTimerViewController : TopLevelViewControllerBase
	{
		private static EnableTimerViewController _instance;

		private DeviceDetailViewModel _device;
		private ProgramDisableFeatureViewModel _feature;

        protected override nfloat ReconBarVerticalLocation
        {
            get
            {
                return base.ReconBarVerticalLocation + 6;
            }
        }

        private EnableTimerViewController(DeviceDetailViewModel device, ProgramDisableFeatureViewModel feature) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this._feature = feature;
				this._device = device;
				this.Initialize();
				this._navBarView.TitleText = feature.SettingName; 
			});
		}

		public static EnableTimerViewController CreateInstance(DeviceDetailViewModel device, ProgramDisableFeatureViewModel feature)
		{
			ExceptionUtility.Try(() =>
			{
				if (_instance != null)
				{
					_instance.Dispose();
					_instance = null;
				}

				_instance = new EnableTimerViewController(device, feature);
			});

			return _instance;
		}

		protected override void InitializeViewController()
		{
			ExceptionUtility.Try(() =>
			{
				base.InitializeViewController();

				this._intervalPickerView.Value = TimeSpan.FromMinutes(30);

				//nav bar
				this._navBarView.OnCancel = () =>
				{
					this.NavigationController.PopViewController(true);
				};

				this._navBarView.OnSet = () =>
				{
					this.SetTime();
				};

				//enabled switch 
				this._enabledSwitch.SetText(StringLiterals.Disable);
				this._enabledSwitch.OnSwitchChanged = () =>
				{
					this.OnSwitchChanged();
				};

				//disable indefinitely 
				this._disableIndefCheckbox.SetText(this._feature.DisableDictionary?.ButtonText?.LastOrDefault());
				this._disableIndefCheckbox.Hidden = true;
				this._disableIndefCheckbox.OnCheckedChanged = () =>
				{
					this.OnCheckboxChanged(_disableIndefCheckbox);
				};

				//disable on time 
				this._disableTimeCheckbox.SetText(this._feature.DisableTimerDictionary?.ButtonText?.LastOrDefault());
				this._disableTimeCheckbox.Hidden = true;
				this._disableTimeCheckbox.OnCheckedChanged = () =>
				{
					this.OnCheckboxChanged(_disableTimeCheckbox);
				};

				//datepicker
				//this._datePickerView.Hidden = true;
				this._intervalPickerView.Hidden = true;

				this.NavigationBarView = this._navBarView;
				this.NavigationItem.HidesBackButton = true;

				int interval;
				if (!Int32.TryParse(this._feature.Feature.Setting.Values.Items.FirstOrDefault().Value.EditPrecision, out interval))
					interval = 30;

				//this._datePickerView.MinuteInterval = interval;

				this.SetCurrentValue();
			});
   		}

		private void SetTime()
		{
			DateTime? dateTime = null;

			if (!this._enabledSwitch.On)
			{
				//dateTime = NSDateToDateTime(this._datePickerView.Date);
				dateTime = DateTime.Now.AddMinutes(this._intervalPickerView.Value.TotalMinutes); 
			}

			this._feature.PromptValue = null;
			//if (!this._datePickerView.Hidden && dateTime != null)
			//	this._feature.PromptValue = DateTimeUtil.DateTimeInEnglish(dateTime.Value, allCaps:false);
			if (!this._intervalPickerView.Hidden && dateTime != null)
				this._feature.PromptValue = this._intervalPickerView.ValueAsString(); //DateTimeUtil.DateTimeInEnglish(dateTime.Value, allCaps: false);

			var originalSetting = this._feature?.Feature?.Setting;
			if (originalSetting != null)
			{
				var setting = new DeviceSetting() { Id = originalSetting.Id, Values = new ItemsDictionary<DeviceSettingValue>() }; 

				//HARDCODED
				setting.Values.Add("ProgramDisable", new DeviceSettingValue() { 
					Value = this._enabledSwitch.On ? 
					            this._feature.EnableValue : 
					            (this._disableIndefCheckbox.Checked ? this._feature.DisableValue : this._feature.DisableTimerValue)
				});

				if (!this._enabledSwitch.On && !this._disableIndefCheckbox.Checked && dateTime != null)
				{
					//TODO: round to nearest 1/2 hour? 
					var durationEnd = dateTime.Value;
					setting.Values.Add("EndDateTimeUtc", new DeviceSettingValue() { 
						Value = DateTimeUtil.ToUtcDateString(durationEnd) 
					});
				}

				//set the values for the setting 
				this.ChangeFeatureSettingValue(setting);
			}
		}

		private void SetCurrentValue()
		{
			this._disableIndefCheckbox.SetChecked(this._feature.IsDisabledIndefinitely);
			this._disableTimeCheckbox.SetChecked(!this._feature.IsDisabledIndefinitely);
			this._enabledSwitch.SetValue(this._feature.IsEnabled);
			this.OnSwitchChanged();

			if (this._feature.IsDisabledOnTimer)
			{
				//this.SetDurationInMinutes(this._feature.DurationInMinutes);
				this.SetDurationEndTime(this._feature.EndDateTimeUtc);
			}
			else {
				this.SetDurationEndTime(DateTime.Now.AddMinutes(24 * 60 + 10));
			}
		}

		private void SetDurationInMinutes(int durationMinutes)
		{
			if (durationMinutes > 0)
			{
				//DateTime target = DateTime.Now.AddMinutes(durationMinutes);
				//round to nearest 10 min 

				//this._datePickerView.SetDate(ConvertDateTimeToNSDate(target), false);
				this._intervalPickerView.Value = TimeSpan.FromMinutes(durationMinutes);
			}
		}

		private void SetDurationEndTime(DateTime? value)
		{
			var target = DateTime.Now.AddMinutes(60); 

			if (value != null)
			{
				target = value.Value;
			}
			//TODO: round to nearest 30 min 

			//this._datePickerView.SetDate(ConvertDateTimeToNSDate(target), false);
			this._intervalPickerView.SetValue(target.Subtract(DateTime.Now));
		}

		private NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return Foundation.NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}

		private void ChangeFeatureSettingValue(DeviceSetting newSetting)
		{
			if (this._enabledSwitch.On)
				this._feature.SettingsValueDictionary = this._feature.EnableDictionary;
			else
			{					
				//if (!this._datePickerView.Hidden)
				if (!this._intervalPickerView.Hidden)
					this._feature.SettingsValueDictionary = this._feature.DisableTimerDictionary;
				else
				{
					this._feature.SettingsValueDictionary = this._feature.DisableDictionary;
				}
			}

			if (this._device.ChangeFeatureSettingValue != null)
				this._device.ChangeFeatureSettingValue(this._device, this._feature, newSetting);

			this.NavigationController.PopViewController(true);
		}

		private DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		private void OnSwitchChanged()
		{
			ExceptionUtility.Try(() =>
			{
				if (this._enabledSwitch.On)
				{
					//this._datePickerView.Hidden = true;
					this._intervalPickerView.Hidden = true;
					this._disableIndefCheckbox.Hidden = true;
					this._disableTimeCheckbox.Hidden = true;
				}
				else {
					this._intervalPickerView.Hidden = (this._disableIndefCheckbox.Checked);
					this._disableIndefCheckbox.Hidden = false;
					this._disableTimeCheckbox.Hidden = false;
				}
			});
		}

		private void OnCheckboxChanged(CheckboxView checkbox)
		{
			ExceptionUtility.Try(() =>
			{
				if (checkbox == this._disableIndefCheckbox)
				{
					this._disableTimeCheckbox.SetChecked(!checkbox.Checked);
					this._intervalPickerView.Hidden = checkbox.Checked;
				}
				else if (checkbox == this._disableTimeCheckbox)
				{
					this._disableIndefCheckbox.SetChecked(!checkbox.Checked);
					this._intervalPickerView.Hidden = !checkbox.Checked;
				}
			});
		}


		private class NavBarView : NavigationBarView
		{
			public const int Margin = 16;

			private readonly UIButton _cancelButton = new UIButton();
			private readonly UIButton _setButton = new UIButton();
			private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
			private Action _onCancel;
			private Action _onSet;

			public Action OnCancel
			{
				get { return this._onCancel;}
				set { this._onCancel = WeakReferenceUtility.MakeWeakAction(value); }
			}

			public Action OnSet 
			{
				get { return this._onSet; }
				set { this._onSet = WeakReferenceUtility.MakeWeakAction(value); }
			}

			public string TitleText
			{
				get { return this._titleLabel.Text; }
				set { this._titleLabel.Text = value;}
			}

			public NavBarView() : base(fullWidth: true)
			{
				ExceptionUtility.Try(() =>
				{
					//cancel button 
					this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Normal);
					this._cancelButton.SetFontAndColor(new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.BlueButtonText));
					this._cancelButton.TouchUpInside += (o, e) =>
					{
						LogUtility.LogMessage("User clicked cancel button (enable timer).");
						if (this.OnCancel != null)
							this.OnCancel();
					};

					//selected label 
					this._titleLabel.SetFontAndColor(new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.StandardTextColor));
					this._titleLabel.Text = String.Empty;

					//select all button 
					this._setButton.SetTitle(StringLiterals.TimerSetButtonText, UIControlState.Normal);
					this._setButton.SetFontAndColor(new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.BlueButtonText));
					this._setButton.TouchUpInside += (o, e) =>
					{
						LogUtility.LogMessage("User clicked set button (enable timer).");
						if (this.OnSet != null)
							this.OnSet();
					};

					this.BackgroundColor = UIColor.White;

					this.AddSubviews(_titleLabel, _cancelButton, _setButton);
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
					this._setButton.SizeToFit();
					this._setButton.SetFrameHeight(Height);
					this._setButton.SetFrameLocation(this.Frame.Width - _setButton.Frame.Width - Margin, 0);

					//selection label 
					this._titleLabel.SizeToFit();
					this._titleLabel.SetFrameHeight(Height);
					this._titleLabel.SetFrameLocation(this.Frame.Width / 2 - this._titleLabel.Frame.Width / 2, 0);
					//this._titleLabel.EnforceMaxXCoordinate(this.Frame.Width - this._setButton.Frame.Left);
				});
			}
		}

		private class SwitchView : UIView
		{
			public const int Height = 45;
			private const int LeftMargin = 16;
			private const int RightMargin = 10;

			private readonly AquamonixLabel _textLabel = new AquamonixLabel();
			private readonly UISwitch _onImmediateSwitch = new UISwitch();
			private readonly DividerLine _dividerTopView = new DividerLine();
			private readonly DividerLine _dividerBottomView = new DividerLine();

			private Action _onSwitchChanged;

			public Action OnSwitchChanged
			{
				get { return this._onSwitchChanged;}
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
					this._textLabel.SetFontAndColor(TextFont);
					this._textLabel.SizeToFit();
					this._textLabel.TextColor = Colors.StandardTextColor;

					//switch
					this._onImmediateSwitch.On = false;
					this._onImmediateSwitch.ValueChanged += (sender, e) =>
					{
						if (this.OnSwitchChanged != null)
							this.OnSwitchChanged();
					};

					this.AddSubviews(
						this._textLabel,
						this._onImmediateSwitch,
						this._dividerTopView,
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
					this._dividerTopView.SetFrameLocation(0, 0);
					this._dividerTopView.SetSize();

					//bottom divider
					this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
					this._dividerBottomView.SetSize();
				});
			}
		}

		class CheckboxView : UIView
		{
			public const int Height = 45;
			private const int LeftMargin = 16;
			private const int RightMargin = 10;

			private readonly AquamonixLabel _textLabel = new AquamonixLabel();
			private readonly RoundCheckBox _checkbox = new RoundCheckBox(Images.GreenCheckboxChecked, Images.GreenCheckboxUnchecked);
			private readonly DividerLine _dividerTopView = new DividerLine();
			private readonly DividerLine _dividerBottomView = new DividerLine();

			private Action _onCheckedChanged;

			public Action OnCheckedChanged
			{
				get { return this._onCheckedChanged; }
				set { this._onCheckedChanged = WeakReferenceUtility.MakeWeakAction(value); }
			}

			public bool Checked
			{
				get { return this._checkbox.Checked; }
			}

			public bool Enabled
			{
				get { return this._checkbox.Enabled; }
				set
				{
					this._checkbox.Enabled = value; 
				}
			}

			public CheckboxView() : base()
			{
				ExceptionUtility.Try(() =>
				{
					//label
					this._textLabel.SetFontAndColor(TextFont);
					this._textLabel.SizeToFit();
					this._textLabel.TextColor = Colors.StandardTextColor;

					//checkbox
					this._checkbox.SetChecked(false);
					this._checkbox.OnCheckedChanged = () =>
					{
						if (this.OnCheckedChanged != null)
							this.OnCheckedChanged();
					};

					this.AddSubviews(
						this._textLabel,
						this._checkbox,
						this._dividerTopView,
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

			public void SetChecked(bool value)
			{
				ExceptionUtility.Try(() =>
				{
					if (this._checkbox.Checked != value)
						this._checkbox.SetChecked(value);
				});
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

					//checkbox
					this._checkbox.SizeToFit();
					this._checkbox.AlignToRightOfParent(RightMargin);
					this._checkbox.CenterVerticallyInParent();
					this._textLabel.EnforceMaxXCoordinate(this._checkbox.Frame.X);

					//top divider
					this._dividerTopView.SetFrameLocation(0, 0);
					this._dividerTopView.SetSize();

					//bottom divider
					this._dividerBottomView.SetFrameLocation(0, this.Frame.Height - 1);
					this._dividerBottomView.SetSize();
				});
			}
		}
	}
}

