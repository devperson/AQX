using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Interactive search box e.g. for Alerts & Devices.
    /// </summary>
	public class SearchBox : AquamonixView
	{
		public const int Height = 54;
		private const int HorizontalMargin = 7;
		private const int VerticalMargin = 7;
		private const int CancelButtonWidth = 60;
		private static readonly bool AnimateCancelButton = false;

		private readonly AquamonixTextField _searchField = new AquamonixTextField();
		private readonly UIButton _cancelButton = new UIButton();

		private static readonly UIImage _searchIcon = UIImage.FromFile("Images/searchBoxIcon.png");
		private static readonly FontWithColor TextboxFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize5, Colors.StandardTextColor);
		private static readonly FontWithColor CancelButtonFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize6, UIColor.White);

		//private static readonly UIImage _xButtonIcon = UIImage.FromFile("Images/xButton.png");

		public Action<string> OnTextChanged
		{
			get { return this._searchField.OnTextChanged; }
			set { this._searchField.OnTextChanged = value; }
		}

		public string Text
		{
			get { return this._searchField.Text; }
			set { this._searchField.Text = value; }
		}

		public SearchBox() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = Colors.FromHex(0xc8c5ca);

				//search textfield
				this._searchField.BackgroundColor = UIColor.White;
				this._searchField.Placeholder = StringLiterals.SearchTextPlaceholder;
				this._searchField.KeyboardType = UIKeyboardType.WebSearch;
				this._searchField.SetFontAndColor(TextboxFont);
				this._searchField.DismissOnButtonClick = true;

				//search icon in textfield 
				var searchIconView = new UIImageView(_searchIcon);
				searchIconView.SizeToFit();

				//view to house search icon
				UIView leftView = new UIView(new CoreGraphics.CGRect(0, 0, searchIconView.Frame.Width + 10, Height - (VerticalMargin * 2)));
				leftView.AddSubview(searchIconView);
				searchIconView.CenterInParent();

				this._searchField.LeftView = leftView;
				this._searchField.LeftViewMode = UITextFieldViewMode.Always;
				this._searchField.LeftIndent = (int)leftView.Frame.Right;

				this._cancelButton.BackgroundColor = this.BackgroundColor;
				this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Normal);
				this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Disabled);
				this._cancelButton.SetFontAndColor(CancelButtonFont);
				this._cancelButton.SetFrameSize(CancelButtonWidth, Height); 
				this._cancelButton.TouchUpInside += (o, e) =>
				{
					this.OnCancel();
				};

				this._searchField.OnGotFocus = () =>
				{
					this.ShowCancelButton(); 
				}; 

				this._searchField.OnLostFocus = () =>
				{
					this.HideCancelButton();
				};

				this.HideCancelButton(); 
				this.AddSubviews(_searchField, this._cancelButton);
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this._searchField.SetFrameSize(this.GetSearchFieldWidth(!this._cancelButton.Hidden), Height - (VerticalMargin * 2));
				this._searchField.SetFrameLocation(HorizontalMargin, VerticalMargin);
				this._searchField.TextRect(new CoreGraphics.CGRect(20, 0, this._searchField.Frame.Width - 20, this._searchField.Frame.Height));

				this._cancelButton.SetFrameLocation(this._searchField.Frame.Right + HorizontalMargin, 0); 
			});
		}

		public void DismissKeyboard()
		{
			ExceptionUtility.Try(() =>
			{
				this._searchField.ResignFirstResponder();
				this.HideCancelButton();
			}); 
		}

		private void OnCancel()
		{
			if (String.IsNullOrEmpty(this._searchField.Text))
			{
				this.DismissKeyboard();
			}

			this._searchField.Text = String.Empty;
			if (this.OnTextChanged != null)
				this.OnTextChanged(String.Empty);
		}

		private nfloat GetSearchFieldWidth(bool cancelButtonShowing)
		{
			return this.Frame.Width - (HorizontalMargin * 2) - (cancelButtonShowing ? CancelButtonWidth : 0);
		}

		private void ShowCancelButton(bool show = true)
		{
			//TODO: animate this 
			MainThreadUtility.InvokeOnMain(() =>
			{
				if (AnimateCancelButton)
				{
					UIView.BeginAnimations("animateDot");
					UIView.SetAnimationDuration(1);
					UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);

					this._searchField.SetFrameWidth(this.GetSearchFieldWidth(show));

					UIView.SetAnimationDelegate(this);
					UIView.CommitAnimations();
				}
				else 
				{
					this._cancelButton.Hidden = (!show);
					this._searchField.SetFrameWidth(this.GetSearchFieldWidth(show));
				}
			});
		}

		private void HideCancelButton()
		{
			this.ShowCancelButton(show: false);
		}
	}
}
