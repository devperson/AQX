using System;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.Views
{
   public class RangeSliderView : UIControl
    {
        RangeSliderTrackLayer _trackLayer;
        RangeSliderKnobLayer _leftKnobLayer;
        private CGPoint _leftTouchPoint;

        public RangeSliderView ()
        {
            this.Initialize();
        }
        public RangeSliderView (CGRect frame): base(frame)
        {
            this.Initialize();
        }
        private void Initialize()
        {
            _trackLayer = new RangeSliderTrackLayer();
            Layer.AddSublayer(_trackLayer);

            _leftKnobLayer = new RangeSliderKnobLayer();
            Layer.AddSublayer(_leftKnobLayer);
            SetLayerFrame();
        }
        private void SetLayerFrame() 
        {
            _trackLayer.Frame = new CGRect(0, (Bounds.Height * 0.25), Bounds.Width, Bounds.Height / 2);
            _trackLayer.SetNeedsDisplay();

            var leftX = _leftTouchPoint == CGPoint.Empty ? 50 : _leftTouchPoint.X;
             _leftKnobLayer.Frame = new CGRect(leftX, 0, Bounds.Height, Bounds.Height);
            _leftKnobLayer.SetNeedsDisplay();
        }
        
        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            var TouchPoint = uitouch.LocationInView(this);
            if(_leftKnobLayer.Frame.Contains(TouchPoint))
            {
                _leftKnobLayer = TouchPoint;
                _leftKnobLayer.Highlighted = true;
                _leftKnobLayer.SetNeedsDisplay();

            }
            return _leftKnobLayer.Highlighted;
        }
        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            var TouchPoint = uitouch.LocationInView(this);
            if(_leftKnobLayer.Highlighted)
            {
                _leftKnobLayer = TouchPoint;
            }
            CATransaction.Begin();
            CATransaction.DisableActions = true;

             SetLayerFrame();
            CATransaction.Commit();
            return _leftKnobLayer.Highlighted;
        }
        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            _leftKnobLayer.Highlighted = false;
            _leftKnobLayer.SetNeedsDisplay(); 
        }

    }
}