using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.ViewControllers;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.Utilities;
using CoreGraphics;

namespace Aquamonix.Mobile.IOS.Views
{
    public class ReconnectingView : AquamonixView
    {
        public const int Height = 50;

        private WeakReference<TopLevelViewControllerBase> _parent;
        private readonly UILabel _messageLabel = new UILabel();

        public ReconnectingView(TopLevelViewControllerBase parent) : base()
        {
            ExceptionUtility.Try(() =>
            {
                this.BackgroundColor = GraphicsUtility.ColorForReconBar(DisplayMode.Reconnecting); 
                this._parent = new WeakReference<TopLevelViewControllerBase>(parent);
                this._messageLabel.SetFontAndColor(new UI.FontWithColor(Fonts.RegularFontName, Sizes.FontSize9, UIColor.White));
            });
        }

        public override void LayoutSubviews()
        {
            ExceptionUtility.Try(() =>
            {
                base.LayoutSubviews();

                this.AddSubview(this._messageLabel);
                this._messageLabel.SizeToFit();
                this._messageLabel.CenterInParent();
            });
        }

        public void SetText(string text)
        {
            MainThreadUtility.InvokeOnMain(() => {
                this._messageLabel.Text = text;
                this._messageLabel.SizeToFit();
                this._messageLabel.CenterInParent();
            });
        }

        public void SetTextAndMode(string text, DisplayMode displayMode)
        {
            MainThreadUtility.InvokeOnMain(() => {

                //if text null, get default text for mode 
                switch (displayMode)
                {
                    case DisplayMode.Reconnecting:
                        if (String.IsNullOrEmpty(text))
                            text = Aquamonix.Mobile.Lib.Domain.StringLiterals.Reconnecting;
                        break;
                    case DisplayMode.ServerDown:
                        if (String.IsNullOrEmpty(text))
                            text = Aquamonix.Mobile.Lib.Domain.StringLiterals.ServerNotAvailable;
                        break;
                    case DisplayMode.Connected:
                        if (String.IsNullOrEmpty(text))
                            text = Aquamonix.Mobile.Lib.Domain.StringLiterals.Connected;
                        break;
                }

                //set background color 
                this.BackgroundColor = GraphicsUtility.ColorForReconBar(displayMode); ;

                //and set text
                this.SetText(text); 
            });
        }


        public enum DisplayMode
        {
            Reconnecting, 
            ServerDown,
            Connected
        }
    }
}
