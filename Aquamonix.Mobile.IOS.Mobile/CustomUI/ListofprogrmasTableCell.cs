using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.CustomUI
{
    public partial class ListofprogrmasTableCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ListofprogrmasTableCell");
        public static readonly UINib Nib;

        public UIImageView Imgfwd
        {
            get
            {
                return imgfwd;
            }
            set
            {
                imgfwd = value;
            }
        }

        public UIImageView Imgwet
        {
            get
            {
                return imgwet;
            }
            set
            {
                imgwet = value;
            }
        }

        public UIImageView ImgProgrm
        {
            get
            {
                return imgProgram;
            }
            set
            {
                imgProgram = value;
            }
        }

        public UILabel Lblname
        {
            get
            {
                return lblname;
            }
            set
            {
                lblname = value;
            }
        }

        public UILabel LblAngle
        {
            get
            {
                return lblangle;
            }
            set
            {
                lblangle = value;
            }
        }

        public UILabel Lblforword
        {
            get
            {
                return lblforword;
            }
            set
            {
                lblforword = value;
            }
        }

        public UILabel LblWet
        {
            get
            {
                return lblwet;
            }
            set
            {
                lblwet = value;
            }
        }


        static ListofprogrmasTableCell()
        {
            Nib = UINib.FromName("ListofprogrmasTableCell", NSBundle.MainBundle);
        }

        protected ListofprogrmasTableCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public ListofprogrmasTableCell(UITableViewCellStyle style, string cellIndentifier) : base(style, cellIndentifier)
        {

            var width = UIScreen.MainScreen.Bounds.Width;
            imgProgram = new UIImageView(new CGRect(10, 10, 50, 50));
            imgProgram.Layer.CornerRadius = 25;
            lblname = new UILabel(new CGRect(ImgProgrm.Frame.Right + 10, 10, 100, 30));
            lblangle = new UILabel(new CGRect(ImgProgrm.Frame.Right + 10, lblname.Frame.Bottom, 100, 30));
            lblforword = new UILabel(new CGRect(ImgProgrm.Frame.Right + 10, lblangle.Frame.Bottom, 50, 30));
            imgfwd = new UIImageView(new CGRect(lblforword.Frame.Right, lblangle.Frame.Bottom, 30, 30));
            lblwet = new UILabel(new CGRect(imgfwd.Frame.Right , lblangle.Frame.Bottom, 50, 30));
            imgwet = new UIImageView(new CGRect(imgfwd.Frame.Right, lblangle.Frame.Bottom, 30, 30));
            lblmm = new UILabel(new CGRect(imgwet.Frame.Right, lblangle.Frame.Bottom, 50, 30));

            this.ContentView.AddSubviews(new UIView[] { imgProgram,lblname,lblangle,lblforword,imgfwd,lblwet,imgwet,lblmm});


        }
    }
}
