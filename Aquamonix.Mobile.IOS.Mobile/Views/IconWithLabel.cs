using System;

using UIKit;
using CoreGraphics;
using Foundation;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Combination of icon & label.
    /// </summary>
	public class IconWithLabel : AquamonixView
    {
		public const int Height = 30;
		private const int Padding = 5;

		private readonly AquamonixLabel _label = new AquamonixLabel();
		private readonly UIImageView _icon = new UIImageView();

		public AquamonixLabel Label
		{
			get
			{
				return this._label;
			}
		}

		public IconWithLabel(FontWithColor font) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this._label.SetFontAndColor(font);

				this.AddSubviews(_icon, _label);
			});
		}

		public void SetFontAndColor(FontWithColor font)
		{
			this._label.SetFontAndColor(font); 
		}

		public void SetIconAndText(UIImage icon, string text)
		{
			ExceptionUtility.Try(() =>
			{
				_icon.Image = icon;
				if (_icon.Image != null)
					_icon.SizeToFit();
				else
					_icon.SetFrameSize(0, 0);

				_label.Text = text;
				_label.SizeToFit();
			});
		}

		public void SetTextColor(UIColor color)
		{
			ExceptionUtility.Try(() =>
			{
				this._label.TextColor = color;
			});
		}

		public void EnforceMaxWidth(int maxWidth)
		{
			if (this.Frame.Width > maxWidth)
				this.SetFrameWidth(maxWidth); 
			
			this._label.EnforceMaxWidth(maxWidth - this._icon.Frame.Right); 
		}

		public CGSize CalculateSize()
		{
			// Get size of target string using the label's font.
			var nsString = new NSString(this._label.Text);
			UIStringAttributes attribs = new UIStringAttributes { Font = this._label.Font };
			var size = nsString.GetSizeUsingAttributes(attribs);

			return size;
		}

		public override void SizeToFit()
		{
			ExceptionUtility.Try(() =>
			{
				this.SetFrameWidth(_icon.Frame.Width + Padding + _label.Frame.Width);
				this.SetFrameHeight((nfloat)Math.Max(this._label.Frame.Height, this._icon.Frame.Height));
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this._icon.SetFrameLocation(0, this.Frame.Height / 2 - _icon.Frame.Height / 2);

				this._label.SetFrameX(_icon.Frame.Right + Padding);
				this._label.SetFrameHeight(this.Frame.Size.Height);
				this._label.CenterVerticallyInParent(); 
				//this._label.EnforceMaxXCoordinate(this.Frame.Width - Padding); 
				//this._label.SetFrameHeight(20);
			});
		}
	}
}
