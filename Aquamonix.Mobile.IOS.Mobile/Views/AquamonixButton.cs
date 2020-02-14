using System;

using UIKit;
using CoreGraphics;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
	public class AquamonixButton : UIButton
	{
		private static UIImage _prevButtonImage = UIImage.FromFile("Images/back.png");
		private static UIImage _nextButtonImage = UIImage.FromFile("Images/forward.png");
		private static FontWithColor ButtonTextFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize9, UIColor.White);

		private UIImage _normalImage;
		private UIImage _disabledImage; 

		private bool _setStyleFlag;

		public AquamonixButtonStyle AquamonixStyle { get; private set; }

		public AquamonixButton(AquamonixButtonStyle style, UIColor backgroundColor = null) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.AquamonixStyle = style;

				this.BackgroundColor = backgroundColor;
				//this.AdjustsImageWhenHighlighted = false;
				this.SetFontAndColor(ButtonTextFont);
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				if (!_setStyleFlag)
				{
					_setStyleFlag = true;

					if (this.AquamonixStyle == AquamonixButtonStyle.RoundedSolidColor)
					{
						if (this._disabledImage == null)
						{
							this._disabledImage = GraphicsUtility.CreateColoredRect(UIColor.Gray, this.Frame.Size);

							if (this._disabledImage != null)
								this.SetBackgroundImage(this._disabledImage, UIControlState.Disabled);
						}

						if (this._normalImage == null)
						{
							this._normalImage = GraphicsUtility.CreateColoredRect(this.BackgroundColor, this.Frame.Size);

							if (this._normalImage != null)
								this.SetBackgroundImage(this._normalImage, UIControlState.Normal);
						}

						this.MakeRoundedCorners(UIRectCorner.AllCorners, 20);
						this.SetTitleColor(UIColor.White, UIControlState.Normal);
						this.SetTitleColor(UIColor.White, UIControlState.Disabled);
					}
					else if (this.AquamonixStyle == AquamonixButtonStyle.Prev)
					{
						this.SetBackgroundImage(_prevButtonImage, UIControlState.Normal);
						this.SizeToFit();
						this.SetTitleColor(UIColor.White, UIControlState.Normal);
					}
					else if (this.AquamonixStyle == AquamonixButtonStyle.Next)
					{
						this.SetBackgroundImage(_nextButtonImage, UIControlState.Normal);
						this.SizeToFit();
						this.SetTitleColor(UIColor.White, UIControlState.Normal);
					}
				}
			});
		}
	}

	public enum AquamonixButtonStyle
	{
		RoundedSolidColor, 
		Prev, 
		Next
	}
}
