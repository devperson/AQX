using System;

using UIKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.Views
{
	public class ProgressWidget : AquamonixView
    {
		private const int DotSize = 4;
		private const int LargeDotSize = 7;

		private readonly System.Timers.Timer _animationTimer = new System.Timers.Timer();
		private readonly UIImageView[] _imageViews;
		private int _currentDotIndex = 0;
		private bool _animationRunning = false;

		private static UIImage normalDotImage = GraphicsUtility.CreateColoredRect(Colors.StandardTextColor, new CGSize(DotSize, DotSize));
		private static UIImage largeDotImage = GraphicsUtility.CreateColoredCircle(UIColor.DarkGray, LargeDotSize);

		public ProgressWidget(int dotCount) : base()
		{
			this._imageViews = new UIImageView[dotCount];

			ExceptionUtility.Try(() =>
			{
				for (int n = 0; n < this._imageViews.Length; n++)
				{
					this._imageViews[n] = new UIImageView(GraphicsUtility.CreateColoredCircle(Colors.StandardTextColor, DotSize));
					this._imageViews[n].ContentMode = UIViewContentMode.Center;
					this._imageViews[n].MakeRoundedCorners(UIRectCorner.AllCorners, DotSize/2);
					this.SetNormalDotState(this._imageViews[n]);
					this.AddSubview(this._imageViews[n]);
					this.SetNormalDotState(this._imageViews[n]);
				}

				this._animationTimer.Elapsed += (sender, e) =>
				{
					if (this._animationRunning)
					{
						this._animationTimer.Enabled = false;
						MainThreadUtility.InvokeOnMain(() =>
						{
							this._currentDotIndex = 0;
							this.AnimateDotLarge();
						});
					}
				};

				this._animationTimer.Interval = 2000; 
			});
		}

		public void StartAnimation()
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				if (!this._animationRunning)
				{
					this._animationRunning = true;
					this.CenterDots();
					this._animationTimer.Enabled = true;
					this._animationTimer.Start();
				}
			});
		}

		public void StopAnimation()
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				this._animationTimer.Stop();
				this._animationTimer.Enabled = false;
				this._animationRunning = false;
			});
		}

		private void SetNormalDotState(UIImageView dotImage)
		{
			dotImage.Image = normalDotImage;
			dotImage.SizeToFit();
			dotImage.SetFrameLocation(dotImage.Frame.X + 1, dotImage.Frame.Y + 1);
		}

		private void SetHighlightedDotState(UIImageView dotImage)
		{
			dotImage.Image = largeDotImage;
			dotImage.SizeToFit();
			dotImage.SetFrameLocation(dotImage.Frame.X -1, dotImage.Frame.Y - 1);
		}

		private void MoveDot(UIImageView dotImage, bool direction)
		{
			dotImage.SetFrameY(dotImage.Frame.Y + (direction ? -3 : 3));
		}

		[Export("AnimateDotLarge")]
        private void AnimateDotLarge()
		{
			if (this._animationRunning)
			{
				MainThreadUtility.InvokeOnMain(() =>
				{
					UIView.BeginAnimations("animateDot");
					UIView.SetAnimationDuration(.2);
					UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);

					//this.SetFrameLocation(0, 0);
					this.MoveDot(this._imageViews[this._currentDotIndex], true); 
					//this.SetHighlightedDotState(this._imageViews[this._currentDotIndex]);

					UIView.SetAnimationDelegate(this);
					UIView.SetAnimationDidStopSelector(new ObjCRuntime.Selector("AnimateDotNormal"));

					UIView.CommitAnimations();
				});
			}
		}

		[Export("AnimateDotNormal")]
		private void AnimateDotNormal()
		{
			if (this._animationRunning)
			{
				MainThreadUtility.InvokeOnMain(() =>
				{
					UIView.BeginAnimations("animateDot");
					UIView.SetAnimationDuration(.2);
					UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);

					//this.SetFrameLocation(10, 10);
					this.MoveDot(this._imageViews[this._currentDotIndex], false);
					//this.SetNormalDotState(this._imageViews[this._currentDotIndex]);

					UIView.SetAnimationDelegate(this);
					UIView.SetAnimationDidStopSelector(new ObjCRuntime.Selector("AnimateNext"));

					UIView.CommitAnimations();
				});
			}
		}

		[Export("AnimateNext")]
		private void AnimateNext()
		{
			this._currentDotIndex++;
			if (this._currentDotIndex < this._imageViews.Length)
				this.AnimateDotLarge();
			else
				this._animationTimer.Enabled = true;
		}

		private void CenterDots()
		{
			foreach (var imageView in this._imageViews)
				imageView.CenterVerticallyInParent();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			ExceptionUtility.Try(() =>
			{
				int totalWidth = (int)this.Frame.Width;
				int minWidth = this._imageViews.Length * DotSize;
				int padding = 0;

				if (minWidth < totalWidth)
					padding = (totalWidth - minWidth) / (_imageViews.Length + 2); 

				int x = padding;

				for (int n =0; n < this._imageViews.Length; n++)
				{
					this._imageViews[n].CenterVerticallyInParent();
					this._imageViews[n].SetFrameX(x);

					x += padding + DotSize;
				}
			});
		}
	}
}
