using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Header that shows brief summary texts.
    /// </summary>
	public class SummaryHeader : AquamonixView
	{
		private const int LeftMargin = 17;
		private const int RightMargin = 10;
		private const int TopMargin = 15;
		private const int BottomMargin = 15;
		private const int LabelHeight = 25;

		private static readonly FontWithColor TextFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize5, Colors.StandardTextColor);

		private List<AquamonixLabel> _messageLabels = new List<AquamonixLabel>(); 

		public int ContentHeight
		{
			get
			{
				return (this._messageLabels.Any() ? TopMargin + BottomMargin + (_messageLabels.Count * LabelHeight) : 0); 
			}
		}

		public SummaryHeader() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = Colors.LightGrayBackground;
			});
		}

		public void SetMessages(IEnumerable<string> messages)
		{
			ExceptionUtility.Try(() =>
			{
				this.Clear();

				if (messages != null)
				{
					foreach (string msg in messages)
					{
						var label = new AquamonixLabel();
						label.Text = msg;
						label.SetFontAndColor(TextFont);

						this._messageLabels.Add(label);
						this.AddSubview(label);
					}
				}
			});
		}

		public void Clear()
		{
			ExceptionUtility.Try(() =>
			{
				foreach (var label in _messageLabels)
				{
					label.RemoveFromSuperview();
				}

				this._messageLabels.Clear();
			});
		}

		public override void SizeToFit()
		{
			ExceptionUtility.Try(() =>
			{
                if (this.Superview?.Frame != null) {
                    this.SetFrameSize(this.Superview.Frame.Width, this.ContentHeight);
                }
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				int currentY = TopMargin;
				foreach (var label in this._messageLabels)
				{
					label.SetFrameSize(this.Frame.Width - LeftMargin - RightMargin, LabelHeight);
					label.SetFrameLocation(LeftMargin, currentY);
					label.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
					currentY = (int)label.Frame.Bottom;
				}
			});
		}
	}
}
