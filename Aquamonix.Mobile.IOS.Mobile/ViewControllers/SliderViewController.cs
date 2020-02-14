using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    [Register("UniversalView")]
    public class UniversalView : UIView
    {
        public UniversalView()
        {
            Initialize();
        }

        public UniversalView(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.Red;
        }
    }

    [Register("SliderViewController")]
    public class SliderViewController : UIViewController
    {
      //  private static SliderViewController _instance;
      //  private string DeviceId;

        private SliderViewController()
        {
            View.AddSubview(new RangeSliderView(new CoreGraphics.CGRect(50, 100, 300, 50)));
        }
        //public static SliderViewController CreateInstance(string deviceId)
        //{
        //    //ExceptionUtility.Try(() =>
        //    //{
        //    //    if (_instance != null && _instance.DeviceId != deviceId)
        //    //    {
        //    //        _instance.Dispose();
        //    //        _instance = null;
        //    //    }
        //    //});

        //    //if (_instance == null)
        //    //    _instance = new SliderViewController();

        //    //return _instance;
        //}

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = new UniversalView();
            this.SetCustomBackButton();
            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }

        public void SetCustomBackButton()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.NavigationItem != null)
                {
                    this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.BackArrow, UIBarButtonItemStyle.Plain, (o, e) =>
                    {
                        if (this.NavigationController != null)
                            this.NavigationController.PopViewController(true);
                    });
                }
            });
        }
    }
}