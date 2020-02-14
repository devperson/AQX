using System;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.Views
{

    public class RangeSliderTrackLayer : CALayer
    {
        public override void DrawInContext(CGContext ctx)
        {
            base.DrawInContext(ctx);
            ctx.SetFillColor(UIColor.Blue.CGColor);
            ctx.FillRect(Bounds);
        }
    }
       
}