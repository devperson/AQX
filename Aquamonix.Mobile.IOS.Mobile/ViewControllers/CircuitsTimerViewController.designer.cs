using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("CircuitsTimerViewController")]
	partial class CircuitsTimerViewController
	{
		private const int TextHeaderHeight = 38;
		private const int LeftTextMargin = 13;
		private const int FooterHeight = 84;

		private static readonly FontWithColor BoldTextFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize8, Colors.StandardTextColor);
		//private static readonly FontWithColor TextFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);

		private readonly TimeIntervalPicker _intervalPickerView = new TimeIntervalPicker();
		private readonly NavBarView _navBarView = new NavBarView();
		private readonly AquamonixView _headerTextView = new AquamonixView();
		private readonly AquamonixView _footerView = new AquamonixView();
		private readonly AquamonixLabel _headerTextLabel = new AquamonixLabel();
		private readonly AquamonixLabel _tableHeaderTextLabel = new AquamonixLabel();
		private readonly AquamonixButton _startButton = new AquamonixButton(AquamonixButtonStyle.RoundedSolidColor);

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			//header text 
			this._headerTextLabel.Text = StringLiterals.SetTimer;
			this._headerTextLabel.SetFontAndColor(BoldTextFont);
			this._headerTextLabel.SizeToFit();

			//table header text 
			this._tableHeaderTextLabel.SetFontAndColor(BoldTextFont);
			this._tableHeaderTextLabel.Text = StringLiterals.SelectPumps;
			this._tableHeaderTextLabel.SizeToFit();

			//header view 
			this._headerTextView.AddSubview(this._headerTextLabel);
			this._headerTextView.BackgroundColor = Colors.LightGrayBackground;

			//footer button 
			this._startButton.BackgroundColor = Colors.AquamonixGreen;
			this._startButton.SetTitle(StringLiterals.StartCircuits.ToUpper(), UIControlState.Normal);
			this._startButton.SetTitle(StringLiterals.StartCircuits.ToUpper(), UIControlState.Disabled);

			//footer view
			this._footerView.SetFrameHeight(FooterHeight);
			this._footerView.AddSubview(this._startButton);

			this.PrimeView.AddSubviews(
				this._intervalPickerView,
				this._headerTextView,
				this._footerView
			);
		}

		protected override void HandleViewDidLayoutSubviews()
		{
			base.HandleViewDidLayoutSubviews();

			//header view 
			this._headerTextView.SetFrameSize(this.PrimeView.Frame.Width, TextHeaderHeight);
			this._headerTextView.SetFrameLocation(0, Sizes.NavigationHeaderHeight);

			//header label 
			this._headerTextLabel.SetFrameX(LeftTextMargin);
			this._headerTextLabel.CenterVerticallyInParent();

			//date picker 
			this._intervalPickerView.SetFrameLocation(0, this._headerTextView.Frame.Bottom);
			this._intervalPickerView.SetFrameSize(this.PrimeView.Frame.Width, 216);

			//table header label 
			this._tableHeaderTextLabel.SetFrameX(LeftTextMargin);
			this._tableHeaderTextLabel.CenterVerticallyInParent();

			//footer
			this._footerView.SetFrameSize(this.PrimeView.Frame.Width, FooterHeight);
			this._footerView.SetFrameLocation(0, this.PrimeView.Frame.Height - FooterHeight);

			//footer button 
			this._startButton.SetFrameHeight(Sizes.StandardButtonHeight);
			this._startButton.SetFrameWidth(260);
			this._startButton.CenterInParent();
		}
	}
}
