using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.ViewControllers;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Application's bottom tab bar.
    /// </summary>
	public class TabBarView : AquamonixView
    {
        public static int Height
        {
            get { return 60 + Sizes.NotchOffset; }
        }
		private const int VerticalMargin = 11;
		private const int HorizontalMargin = 14;
        private const int ButtonHeight = 33;
        private const int ButtonWidth = 38;
        private const int ImageSize = 28;

		private static readonly UIImage _alertsTabImage = UIImage.FromFile("Images/TabBar/bell.png");
		private static readonly UIImage _alertsTabActiveImage = UIImage.FromFile("Images/TabBar/bell_active.png");
		private static readonly UIImage _alertsRedTabImage = UIImage.FromFile("Images/TabBar/bellred.png");
		private static readonly UIImage _alertsRedTabActiveImage = UIImage.FromFile("Images/TabBar/bellred_active.png");
		private static readonly UIImage _homeTabImage = UIImage.FromFile("Images/TabBar/home.png");
		private static readonly UIImage _homeTabActiveImage = UIImage.FromFile("Images/TabBar/home_active.png");
		private static readonly UIImage _settingsTabImage = UIImage.FromFile("Images/TabBar/gear.png");
		private static readonly UIImage _settingsTabActiveImage = UIImage.FromFile("Images/TabBar/gear_active.png");

		private readonly UIButton _alertsButton = new UIButton(UIButtonType.Custom);
		private readonly UIButton _homeButton = new UIButton(UIButtonType.Custom);
		private readonly UIButton _settingsButton = new UIButton(UIButtonType.Custom);
		private readonly DividerLine _dividerView = new DividerLine();

		private WeakReference<TopLevelViewControllerBase> _parent; 

		public TabBarView(TopLevelViewControllerBase parent) : base() 
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = UIColor.White;
				this._parent = new WeakReference<TopLevelViewControllerBase>(parent);

				this.SetAlertsButtonImage();
				_alertsButton.TouchUpInside += (o, e) =>
				{
					ExceptionUtility.Try(() =>
					{
						TopLevelViewControllerBase target;
						if (this._parent != null && this._parent.TryGetTarget(out target))
							target.NavigateAlerts();
					});
				};

				_homeButton.SetImage(_homeTabImage, UIControlState.Normal);
				_homeButton.SetImage(_homeTabActiveImage, UIControlState.Disabled);
				_homeButton.TouchUpInside += (o, e) =>
				{
					ExceptionUtility.Try(() =>
					{
						TopLevelViewControllerBase target;
						if (this._parent != null && this._parent.TryGetTarget(out target))
							target.NavigateHome();
					});
				};

				_settingsButton.ImageView.Image = UIImage.FromFile("Images/TabBar/gear.png");
				_settingsButton.SetImage(_settingsTabImage, UIControlState.Normal);
				_settingsButton.SetImage(_settingsTabActiveImage, UIControlState.Disabled);
				_settingsButton.TouchUpInside += (o, e) =>
				{
					ExceptionUtility.Try(() =>
					{
						TopLevelViewControllerBase target;
						if (this._parent != null && this._parent.TryGetTarget(out target))
							target.NavigateUserConfig();
					});
				};

				this.AddSubviews(_alertsButton, _homeButton, _settingsButton, _dividerView);

				NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.AlertsCountChanged.ToString()), this.OnAlertsCountChanged);
			});
   		}

		public void SelectTab(int tabIndex)
		{
			ExceptionUtility.Try(() =>
			{
				switch (tabIndex)
				{
					case 0:
						this._alertsButton.Enabled = false;
						break;
					case 1:
						this._homeButton.Enabled = false;
						break;
					case 2:
						this._settingsButton.Enabled = false;
						break;
				}
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				var buttonWidth = ButtonWidth;

				this._alertsButton.SetFrameSize(buttonWidth, ButtonHeight);
				this._alertsButton.SetFrameLocation(HorizontalMargin, VerticalMargin);
				this._alertsButton.ImageView.SetFrameSize(ImageSize, ImageSize);
                this._alertsButton.ImageEdgeInsets = new UIEdgeInsets((ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2, (ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2);

				this._homeButton.SetFrameSize(buttonWidth, ButtonHeight);
				this._homeButton.SetFrameLocation((this.Frame.Width / 2) - (buttonWidth / 2), VerticalMargin);
				this._homeButton.ImageView.SetFrameSize(ImageSize, ImageSize);
                this._homeButton.ImageEdgeInsets = new UIEdgeInsets((ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2, (ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2);

                this._settingsButton.SetFrameSize(buttonWidth, ButtonHeight);
				this._settingsButton.SetFrameLocation(this.Frame.Width - buttonWidth - HorizontalMargin, VerticalMargin);
				this._settingsButton.ImageView.SetFrameSize(ImageSize, ImageSize);
                this._settingsButton.ImageEdgeInsets = new UIEdgeInsets((ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2, (ButtonHeight - ImageSize) / 2, (buttonWidth - ImageSize) / 2);

                this._dividerView.SetFrameLocation(0, 0);
				this._dividerView.SetSize();
			});
		}

		public void SetAlertsButtonImage()
		{
			this.SetAlertsButtonImage(DataCache.ActiveAlertsCount > 0); 
		}

		private void SetAlertsButtonImage(bool hasNewAlerts)
		{
			ExceptionUtility.Try(() =>
			{
				if (hasNewAlerts)
				{
					_alertsButton.SetImage(_alertsRedTabImage, UIControlState.Normal);
					_alertsButton.SetImage(_alertsRedTabActiveImage, UIControlState.Disabled);
				}
				else
				{
					_alertsButton.SetImage(_alertsTabImage, UIControlState.Normal);
					_alertsButton.SetImage(_alertsTabActiveImage, UIControlState.Disabled);
				}
			});
		}

		private void OnAlertsCountChanged(NSNotification notification)
		{
			MainThreadUtility.InvokeOnMain(() =>
			{
				this.SetAlertsButtonImage();
			}); 
		}
	}
}
