using System;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.Views
{

    public class RangeSliderKnobLayer : CALayer
    {
        public bool Highlighted
        {
            get;
            set;
        }
        public override void DrawInContext(CGContext ctx)
        {
            base.DrawInContext(ctx);
            if(Highlighted)
            ctx.SetFillColor(UIColor.Purple.CGColor);
            else
                 ctx.SetFillColor(UIColor.Yellow.CGColor);
            ctx.FillRect(Bounds);
        }

        public static implicit operator RangeSliderKnobLayer(CGPoint v)
        {
            throw new NotImplementedException();
        }
    }
}

        

      