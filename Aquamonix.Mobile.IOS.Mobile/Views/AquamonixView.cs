using System;

using UIKit;
using CoreGraphics;


namespace Aquamonix.Mobile.IOS.Views
{
    public class AquamonixView : UIView
    {
        private bool _isDisposed = false; 

        public bool IsDisposed { get { return _isDisposed; } }

        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
