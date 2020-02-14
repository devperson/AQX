using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.ViewControllers;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionStateDebugView : AquamonixView
    {
        public const int Height = 50;
        private const int VerticalMargin = 11;
        private const int HorizontalMargin = 19;

        private readonly DividerLine _dividerView = new DividerLine();
        private readonly UILabel _infoLabel = new UILabel(); 

        private WeakReference<TopLevelViewControllerBase> _parent;

        public ConnectionStateDebugView(TopLevelViewControllerBase parent) : base()
        {
            ExceptionUtility.Try(() =>
            {
                this.BackgroundColor = UIColor.White;
                this._parent = new WeakReference<TopLevelViewControllerBase>(parent);

                this._infoLabel.SetFontAndColor(new UI.FontWithColor(Fonts.RegularFontName, Sizes.FontSize4, Colors.StandardTextColor));
                this.SetText(String.Empty);

                this.AddSubviews(_dividerView, _infoLabel);

                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.ConnectionStateChanged.ToString()), this.OnConnectionStateChanged);
            });
        }

        public void SetText(string text)
        {
            MainThreadUtility.InvokeOnMain(() => {
                this._infoLabel.Text = text;
                this._infoLabel.SizeToFit();
            });
        }

        public override void LayoutSubviews()
        {
            ExceptionUtility.Try(() =>
            {
                base.LayoutSubviews();

                this._dividerView.SetFrameLocation(0, 0);
                this._dividerView.SetSize();
            });
        }

        private void OnConnectionStateChanged(NSNotification notification)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                this.SetText(ConnectionManager.State.ToString());
            });
        }
    }
}
