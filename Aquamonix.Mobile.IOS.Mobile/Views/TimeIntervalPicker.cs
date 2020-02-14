using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
	public class TimeIntervalPicker : AquamonixView
    {
		private UIPickerView _hoursPicker = new UIPickerView();
		private UIPickerView _minutesPicker = new UIPickerView();

		public TimeSpan Value
		{
			get
			{
				int hours = (int)this._hoursPicker.SelectedRowInComponent(0);
				int minutes = (int)this._minutesPicker.SelectedRowInComponent(0) * this.MinutesInterval;

				return TimeSpan.FromMinutes(minutes + (hours * 60)); 
			}
			set
			{
				this.SetValue(value);
			}
		}

		public int MinutesInterval { get;  private set; }

		public TimeIntervalPicker(int interval = 1) : base()
		{
			this.MinutesInterval = interval; 

			ExceptionUtility.Try(() =>
			{
				this._hoursPicker.Model = new HoursPickerViewModel();
				this._minutesPicker.Model = new MinutesPickerViewModel(this.MinutesInterval);
			});

			this.AddSubviews(this._hoursPicker, this._minutesPicker);
		}

		public void SetValue(TimeSpan value)
		{
			var minutes = value.TotalMinutes;
			var hours = 0;
			while (minutes >= 60)
			{
				hours++;
				minutes -= 60; 
			}

			if (hours < 0)
				hours = 0;
			if (minutes < 0)
				minutes = 0; 

			this._hoursPicker.Select((hours), 0, false);
			this._minutesPicker.Select(((int)minutes / this.MinutesInterval), 0, false);
		}

		public string ValueAsString()
		{
			int hours = (int)this._hoursPicker.SelectedRowInComponent(0);
			int minutes = (int)this._minutesPicker.SelectedRowInComponent(0) * this.MinutesInterval;

			string output = String.Empty;
			string minutesString = String.Format("{0} {1}", minutes, (minutes == 1 ? "minute" : "minutes"));
			if (hours == 0)
			{
				output = minutesString;
			}
			else {
				string hoursString = String.Format("{0} {1}", hours, (hours == 1 ? "hour" : "hours"));
				output = hoursString;

				if (minutes > 0)
					output += " and " + minutesString;
			}

			return output;
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				this._hoursPicker.SetFrameLocation(0, 0);
				this._hoursPicker.SetFrameSize((nfloat)(120), this.Frame.Height);

				this._minutesPicker.SetFrameLocation(this._hoursPicker.Frame.Right, this._hoursPicker.Frame.Y);
				this._minutesPicker.SetFrameSize(this._hoursPicker.Frame.Width, this._hoursPicker.Frame.Height);

				//center everything
				var totalWidth = this.Frame.Width;
				var contentWidth = this._minutesPicker.Frame.Right - this._hoursPicker.Frame.X;
				var leftMargin = (totalWidth - contentWidth) / 2;

				this._hoursPicker.SetFrameX(leftMargin);
				this._minutesPicker.SetFrameX(this._hoursPicker.Frame.Right);

				base.LayoutSubviews();
			}); 
		}

		public class HoursPickerViewModel : UIPickerViewModel
		{
			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 100;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return String.Format("{0} {1}", (row), (row == 1) ? "hour" : "hours");
			}
		}

		public class MinutesPickerViewModel : UIPickerViewModel
		{
			public int Interval { get; private set;}

			public MinutesPickerViewModel(int interval = 1)
			{
				this.Interval = interval;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return (nint)System.Math.Floor((double)(60 / this.Interval));
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return String.Format("{0} min", (row * Interval));
			}
		}
	}
}
