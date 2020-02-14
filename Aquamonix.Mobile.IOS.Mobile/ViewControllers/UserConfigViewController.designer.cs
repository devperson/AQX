// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

using UIKit;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	[Register ("UserConfigViewController")]
	partial class UserConfigViewController
	{
		private const int SideMargin = 0;
		private const int TextMargin = 5;

		private static readonly FontWithColor LabelFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize7, Colors.StandardTextColor);
		private static readonly FontWithColor VersionLabelFont = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize6, Colors.StandardTextColor);
		private static readonly FontWithColor TextBoxFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize7, Colors.StandardTextColor);
		private static readonly FontWithColor NavBarButtonsFont = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.BlueButtonText);
		private static readonly FontWithColor LogoutButtonFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize10, Colors.ErrorTextColor);
		private static readonly FontWithColor SupportButtonFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize10, Colors.BlueButtonText);
		                                                              
		private readonly NavBarView _navBarView = new NavBarView();
		private readonly UserConfigTableViewController _tableViewController = new UserConfigTableViewController();
		private readonly UIButton _logoutButton = new UIButton();
		private readonly UIButton _supportButton = new UIButton();
		private readonly AquamonixLabel _versionLabel = new AquamonixLabel();

		void ReleaseDesignerOutlets()
		{
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

			this.ShowTabBar = true;
            this.ShowConnectionBar = false;

			this.PrimeView.BackgroundColor = Colors.LightGrayBackground;
			this._tableViewController.TableView.BackgroundColor = UIColor.White;

			//logout button 
			this._logoutButton.BackgroundColor = UIColor.White;
			this._logoutButton.SetTitle(StringLiterals.LogoutButtonText, UIControlState.Normal);
			this._logoutButton.SetTitle(StringLiterals.LogoutButtonText, UIControlState.Disabled);
			this._logoutButton.SetTitleColor(LogoutButtonFont.Color, UIControlState.Normal);
			this._logoutButton.Font = LogoutButtonFont.Font;

			//support button 
			this._supportButton.BackgroundColor = UIColor.White;
			this._supportButton.SetTitle(StringLiterals.SupportButtonText, UIControlState.Normal);
			this._supportButton.SetTitle(StringLiterals.SupportButtonText, UIControlState.Disabled);
			this._supportButton.SetTitleColor(SupportButtonFont.Color, UIControlState.Normal);
			this._supportButton.Font = SupportButtonFont.Font;

			//version label 
			this._versionLabel.SetFontAndColor(VersionLabelFont);
			this._versionLabel.Text = VersionUtility.GetVersionString();
			this._versionLabel.SizeToFit();

			this.PrimeView.AddSubviews(
				this._tableViewController.TableView, 
				this._logoutButton, 
				this._supportButton, 
				this._versionLabel
			);
		}

		protected override void HandleViewDidLayoutSubviews()
		{
			base.HandleViewDidLayoutSubviews();

			//logout button 
			this._logoutButton.SetFrameLocation(SideMargin, this._tableViewController.TableView.Frame.Bottom + 50);
			this._logoutButton.SetFrameSize(this.PrimeView.Frame.Width - (SideMargin * 2), 60);

			//support button 
			this._supportButton.SetFrameLocation(SideMargin, this._logoutButton.Frame.Bottom + 20);
			this._supportButton.SetFrameSize(this._logoutButton.Frame.Size);

			//version label 
			this._versionLabel.SetFrameY(this.TabBar.Frame.Top - this._versionLabel.Frame.Height - 10);
			this._versionLabel.AlignToRightOfParent(TextMargin);
		}
	}
}
