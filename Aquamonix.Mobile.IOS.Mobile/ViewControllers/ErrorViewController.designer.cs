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

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register("ErrorViewController")]
	partial class ErrorViewController
	{
		private const int TopMargin = 30;
		private const int SideMargin = 16;
		private const int InnerMargin = 10;
		private const int TextMargin = 5;

		private static readonly FontWithColor HeaderFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize9, Colors.StandardTextColor);
		private static readonly FontWithColor SubFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize4, Colors.StandardTextColor);
		private static readonly FontWithColor ButtonFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize7, Colors.BlueButtonText);
		//private readonly static FontWithColor ErrorLabelFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize7, Colors.StandardTextColor); 

		private readonly UIView _innerView = new UIView();
		private readonly AquamonixLabel _headerLabel = new AquamonixLabel();
		private readonly AquamonixLabel _subLabel1 = new AquamonixLabel();
		private readonly AquamonixLabel _subLabel2 = new AquamonixLabel();
		private readonly UIButton _retryButton = new UIButton();
		private readonly UIButton _settingsButton = new UIButton();
		private readonly UIImageView _logoImageView = new UIImageView();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			ExceptionUtility.Try(() =>
			{
				base.InitializeViews();

				this.PrimeView.BackgroundColor = UIColor.White;

				//inner view 
				this._innerView.BackgroundColor = UIColor.White;
				this._innerView.Layer.BorderWidth = (nfloat)0.5;
				this._innerView.Layer.BorderColor = UIColor.Red.CGColor;

				//big label 
				this._headerLabel.SetFontAndColor(HeaderFont);
				this._headerLabel.Text = StringLiterals.ConnectionUnavailable; //"Connection Unavailable";
				this._headerLabel.TextAlignment = UITextAlignment.Center;
				this._headerLabel.SizeToFit();

				//logo image 
				this._logoImageView.Image = Images.AquamonixLogo;
				this._logoImageView.SizeToFit();

				//sub label 
				this._subLabel1.SetFontAndColor(SubFont);
				this._subLabel1.Text = StringLiterals.ConnectionLost; //"Connection to the server has been lost.";
				this._subLabel1.TextAlignment = UITextAlignment.Center;
				this._subLabel1.SizeToFit();

				this._subLabel2.SetFontAndColor(SubFont);
				this._subLabel2.Text = StringLiterals.TryAgain; //"We can try again to reconnect, or go back to Settings.";
				this._subLabel2.TextAlignment = UITextAlignment.Center;
				this._subLabel2.SizeToFit();

				//cancel button 
				this._retryButton.SetTitle("Retry", UIControlState.Normal);
				this._retryButton.SetFontAndColor(ButtonFont);
				this._retryButton.SizeToFit();

				//settings button 
				this._settingsButton.SetTitle("Settings", UIControlState.Normal);
				this._settingsButton.SetFontAndColor(ButtonFont);
				this._settingsButton.SizeToFit();

				this._innerView.AddSubviews(
					this._headerLabel,
					this._logoImageView,
					this._subLabel1,
					this._subLabel2,
					this._retryButton,
					this._settingsButton
				);

				this.PrimeView.AddSubviews(_innerView);
			});
		}

		protected override void HandleViewDidLayoutSubviews()
		{
			//inner view 
			this._innerView.SetFrameLocation(InnerMargin, TopMargin);
			this._innerView.SetFrameSize(this.PrimeView.Frame.Width - (InnerMargin * 2), this.PrimeView.Frame.Height - (InnerMargin + TopMargin));

			//header label 
			this._headerLabel.SetFrameLocation(TextMargin, 80);
			this._headerLabel.SetFrameWidth(this._innerView.Frame.Width - (TextMargin * 2));

			//logo image 
			this._logoImageView.SetFrameY(this._headerLabel.Frame.Bottom + 20);
			this._logoImageView.CenterHorizontallyInParent();

			//sublabel 
			this._subLabel1.SetFrameWidth(this._innerView.Frame.Width - (TextMargin * 2));
			this._subLabel1.SetFrameLocation(TextMargin, this._logoImageView.Frame.Bottom + 20);

			this._subLabel2.SetFrameWidth(this._innerView.Frame.Width - (TextMargin * 2));
			this._subLabel2.SetFrameLocation(TextMargin, this._subLabel1.Frame.Bottom);

			//cancel button 
			this._retryButton.CenterHorizontallyInParent();
			this._retryButton.SetFrameY(this._subLabel2.Frame.Bottom + 20);

			//cancel button 
			this._settingsButton.CenterHorizontallyInParent();
			this._settingsButton.SetFrameY(this._retryButton.Frame.Bottom + 20);

        }
	}
}
