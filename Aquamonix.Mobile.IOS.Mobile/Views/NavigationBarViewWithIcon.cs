using System;

using UIKit;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Navigation bar variation with icon.
    /// </summary>
	public class NavigationBarViewWithIcon : NavigationBarView
	{
		private static readonly FontWithColor NavHeaderFont = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.StandardTextColor);
		private static readonly FontWithColor NavHeaderFontSmall = new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize6, Colors.StandardTextColor);

		private readonly IconWithLabel _titleLabel = new IconWithLabel(NavHeaderFont);

		private int MaxTitleWidth
		{
			get { return (int)UIScreen.MainScreen.Bounds.Width - 90; }
		}

		public NavigationBarViewWithIcon(bool fullWidth = false) : base(fullWidth)
		{
			ExceptionUtility.Try(() =>
			{
				this.AddSubviews(_titleLabel);
			});
		}

		public void SetTitleAndImage(string title, UIImage image = null)
		{
			ExceptionUtility.Try(() =>
			{
				this._titleLabel.SetIconAndText(image, title);
				this.AdjustTitleSize();
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this._titleLabel.SetFrameHeight(Height);

				//enforce max width for the chumpy 
				this._titleLabel.EnforceMaxWidth(this.MaxTitleWidth);

				//this._titleLabel.SetFrameX(this.Frame.Width / 2 - _titleLabel.Frame.Width / 2);
				this._titleLabel.CenterHorizontallyInParent();
				this._titleLabel.SetFrameY(0);
			});
		}

		private void AdjustTitleSize()
		{
			//this._titleLabel.SetDebugBorder();
			//this._titleLabel.Label.SetDebugBorder();
			this._titleLabel.Label.TextAlignment = UITextAlignment.Center;
			    
			var size = this._titleLabel.CalculateSize();

			if (size.Width > this.MaxTitleWidth)
			{
				this._titleLabel.SetFontAndColor(NavHeaderFontSmall);
				size = this._titleLabel.CalculateSize();

				if (size.Width > this.MaxTitleWidth)
				{
					this._titleLabel.Label.LineBreakMode = UILineBreakMode.WordWrap;
					this._titleLabel.Label.Lines = 2;
					this._titleLabel.Label.Text = this._titleLabel.Label.Text;
					this._titleLabel.SetFontAndColor(NavHeaderFontSmall);

					size = this._titleLabel.CalculateSize();
					if (size.Width > MaxTitleWidth)
					{
						this._titleLabel.Label.LineBreakMode = UILineBreakMode.MiddleTruncation;
						this._titleLabel.Label.Lines = 2;
						this._titleLabel.Label.Text = this._titleLabel.Label.Text;
						this._titleLabel.SetFontAndColor(NavHeaderFontSmall);
					}
				}
			}

			this._titleLabel.SizeToFit();
		}
	}
}
