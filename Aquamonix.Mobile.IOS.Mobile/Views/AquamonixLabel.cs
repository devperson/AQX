using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
	public class AquamonixLabel : UILabel
	{
		public AquamonixLabel() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.LineBreakMode = UILineBreakMode.TailTruncation;
			});
		}

		public AquamonixLabel(FontWithColor fontWithColor) : this()
		{
			ExceptionUtility.Try(() =>
			{
				this.SetFontAndColor(fontWithColor);
			});
		}

		public void EnforceMaxWidth(int maxWidth)
		{
			ExceptionUtility.Try(() =>
			{
				this.EnforceMaxWidth((nfloat)maxWidth);
			});
		}

		public void EnforceMaxWidth(nfloat maxWidth)
		{
			ExceptionUtility.Try(() =>
			{
				if (this.Frame.Width > maxWidth)
					this.SetFrameWidth(maxWidth);
			});
		}

		public void EnforceMaxXCoordinate(int maxX)
		{
			this.EnforceMaxXCoordinate((nfloat)maxX);
		}

		public void EnforceMaxXCoordinate(nfloat maxX)
		{
			this.EnforceMaxWidth(maxX - this.Frame.X);
		}
	}
}
