using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquamonix.Mobile.IOS.Views
{
    public class NumberedTextTableViewCells : TableViewCellBase
    {
        protected const int LeftMargin = 4;
        protected const int LeftTextMargin = 42;
        protected const int RightMargin = 10;

        private static readonly FontWithColor FirstLineFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
        private static readonly FontWithColor SecondLineFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize4, Colors.StandardTextColor);

        private readonly UILabel _numberLabel = new UILabel();
        private readonly UILabel _firstLineLabel = new UILabel();
        private readonly UILabel _secondLineLabel = new UILabel();
        private readonly UILabel _forwordlable = new UILabel();
        private readonly UILabel _wetlable = new UILabel();
        private readonly UILabel _smmlable = new UILabel();

        private readonly UIImageView _mainImage = new UIImageView();
        private readonly UIImageView _forImage = new UIImageView();
        private readonly UIImageView _Wetimage = new UIImageView();

        protected UILabel NumberLabel
        {
            get { return _numberLabel; }
        }
        protected UILabel FirstLineLabel
        {
            get { return _firstLineLabel; }
        }
        protected UILabel SecondLineLabel
        {
            get { return _secondLineLabel; }
        }
        protected UILabel ForrwordLable
        {
            get { return _forwordlable; }
        }
        protected UILabel Wetlable
        {
            get { return _wetlable; }
        }
        protected UILabel SMMlable
        {
            get { return _smmlable; }
        }

        public NumberedTextTableViewCells(IntPtr handle) : base(handle)
        {
            ExceptionUtility.Try(() =>
            {
                this.ContentView.AddSubviews(_mainImage, _numberLabel, _firstLineLabel, _forwordlable, _wetlable, _smmlable, _forImage, _Wetimage);
            });
        }

        public void LoadCellValues(string number, string programNumber, string secondLine)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.Text = programNumber;// number;
                this._numberLabel.SizeToFit();
                this._firstLineLabel.Text = programNumber;
                this._firstLineLabel.SizeToFit();
                this._forwordlable.Text = "fwd";
                this._forwordlable.SizeToFit();
                this._wetlable.Text = "Wet";
                this._wetlable.SizeToFit();
                this._smmlable.Text = "mm E A1 A2";
                this._smmlable.SizeToFit();
            });
        }

        public void LoadCellValues(string number, string programnumber, string secondLine, ItemsDictionary<StepsItems> items)
        {
            RemoveSubviews();
            this.ContentView.AddSubview(_numberLabel);
            int Ypos = 80;
            ExceptionUtility.Try(() =>
            {
                if (items != null)
                {
                    foreach (var step in items)
                    {
                        UIView Circle = new UIView();
                        Circle.BackgroundColor = UIColor.Black;
                        Circle.Layer.CornerRadius = 35;
                        Circle.Frame = new CoreGraphics.CGRect(15, Ypos, 70, 70);

                        UIImageView FromAngleImageView = new UIImageView();
                        FromAngleImageView.Image = UIImage.FromFile("Images/CurrentAngleFwd.png");

                        double FromAngle = step.Value.FromAngle != null ? step.Value.FromAngle.Visible == true ? double.Parse(step.Value.FromAngle.Value) : 0 : 0;
                        //double FromAngle = double.Parse("0");
                        var CurrentAngle = Math.PI * (FromAngle) / 180.0;
                        FromAngleImageView.Frame = new CoreGraphics.CGRect(17, 25, 40, 35);
                        FromAngleImageView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)CurrentAngle), 6, -25);
                        UIImageView NextAngleImageView = new UIImageView();
                        NextAngleImageView.Image = UIImage.FromFile("Images/NextAngle.png");
                        NextAngleImageView.Frame = new CoreGraphics.CGRect(17, 20, 40, 35);
                        double ToAngle = step.Value.ToAngle != null ? step.Value.ToAngle.Visible == true ? double.Parse(step.Value.ToAngle.Value) : 0 : 0;
                        //double ToAngle = double.Parse("0");
                        var NextAngle = Math.PI * ToAngle / 180.0;
                        NextAngleImageView.Transform = CGAffineTransform.Translate(CGAffineTransform.MakeRotation((float)NextAngle), 0, -23);
                        UILabel StepLbl = new UILabel();
                        StepLbl.Text = step.Value.Name;
                        StepLbl.Frame = new CoreGraphics.CGRect(Circle.Frame.Right + 30, Ypos, 100, 70);
                        StepLbl.SizeToFit();
                        UILabel Angle = new UILabel();
                        Angle.Text = FromAngle.ToString() + " deg";
                        Angle.Frame = new CoreGraphics.CGRect(Circle.Frame.Right + 30, StepLbl.Frame.Top + 30, 100, 70);
                        Angle.SizeToFit();

                        var Direction = step.Value.Direction != null ? step.Value.Direction.Value != null ? step.Value.Direction.Value : "Forward" : "Forward";

                        UILabel DirectionLbl = new UILabel();
                        DirectionLbl.Text = Direction.ToString();
                        DirectionLbl.Frame = new CoreGraphics.CGRect(Angle.Frame.Right + 5, StepLbl.Frame.Top + 30, 100, 70);
                        DirectionLbl.SizeToFit();

                        ContentView.AddSubview(DirectionLbl);
                        ContentView.AddSubview(Angle);
                        ContentView.AddSubview(_numberLabel);
                        ContentView.AddSubview(StepLbl);
                        Ypos = Ypos + 80;
                        Circle.AddSubview(FromAngleImageView);
                        Circle.AddSubview(NextAngleImageView);
                        ContentView.AddSubviews(Circle);


                    }
                }

                this._numberLabel.Text = programnumber;// number;
                this._numberLabel.SizeToFit();
                //this._firstLineLabel.Text = programnumber;
                //this._firstLineLabel.SizeToFit();
                //this._forwordlable.Text = "fwd";
                //this._forwordlable.SizeToFit();
                //this._wetlable.Text = "Wet";
                //this._wetlable.SizeToFit();
                //this._smmlable.Text = "mm E A1 A2";
                //this._smmlable.SizeToFit();


            });
        }

        public void RemoveSubviews()
        {
            foreach (UIView view in this.ContentView.Subviews)
            {
                view.RemoveFromSuperview();
            }
        }

        public void SetTextColor(UIColor color)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.TextColor = color;
                this._firstLineLabel.TextColor = color;
            });
        }

        public void SetLine2TextColor(UIColor color)
        {
            ExceptionUtility.Try(() =>
            {
                this._secondLineLabel.TextColor = color;
            });
        }

        public void SetLine1Font(FontWithColor font)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.SetFontAndColor(font);
                this._firstLineLabel.SetFontAndColor(font);
                this._firstLineLabel.SizeToFit();
            });
        }

        public void SetLine2Font(FontWithColor font)
        {
            ExceptionUtility.Try(() =>
            {
                this._secondLineLabel.SetFontAndColor(font);
                this._secondLineLabel.SizeToFit();
            });
        }

        protected override void HandleLayoutSubviews()
        {
            base.HandleLayoutSubviews();
            //this._mainImage.SetFrameHeight(30);
            //this._mainImage.SetFrameWidth(33);
            //this._mainImage.Frame = new CoreGraphics.CGRect(15, 10, 80, 80);
            //this._mainImage.Image = Images.MainImage;

            this._numberLabel.Frame = new CoreGraphics.CGRect(this._mainImage.Frame.Right + 30, 10, 100, 20);
            this._numberLabel.Font = UIFont.PreferredHeadline;
            this._numberLabel.TextColor = UIColor.Black;

            //this._firstLineLabel.Frame = new CoreGraphics.CGRect(this._mainImage.Frame.Right + 30, this._numberLabel.Frame.Bottom + 10, 100, 20);
            //this._firstLineLabel.Font = UIFont.PreferredBody;
            //this._firstLineLabel.TextColor = UIColor.Black;


            //this._forwordlable.Frame = new CoreGraphics.CGRect(this._mainImage.Frame.Right + 30, this._firstLineLabel.Frame.Bottom + 5, 30, 30);
            //this._forwordlable.Font = UIFont.PreferredCaption1;
            //this._forwordlable.TextColor = UIColor.Gray;

            //this._forImage.Frame = new CoreGraphics.CGRect(this._forwordlable.Frame.Right, this._firstLineLabel.Frame.Bottom, 30, 30);
            //this._forImage.Image = Images.Farrow;

            //this._wetlable.Frame = new CoreGraphics.CGRect(this._forImage.Frame.Right, this._firstLineLabel.Frame.Bottom + 5, 30, 30);
            //this._wetlable.Font = UIFont.PreferredCaption2;
            //this._wetlable.TextColor = UIColor.Gray;

            //this._Wetimage.Frame = new CoreGraphics.CGRect(this._wetlable.Frame.Right, this._firstLineLabel.Frame.Bottom, 30, 30);
            //this._Wetimage.Image = Images.Wet;

            //this._smmlable.Frame = new CoreGraphics.CGRect(this._Wetimage.Frame.Right, this._firstLineLabel.Frame.Bottom + 5, 100, 30);
            //this._smmlable.Font = UIFont.PreferredCaption2;
            //this._smmlable.TextColor = UIColor.Gray;
        }
    }

    public class StatusPickerViewModel : UIPickerViewModel
    {
        public event EventHandler<int > PickerChanged;
        // UILabel label;
        private IList<string> values = new List<string>
        {
            "1","2","3","4","5","6","7","8","9","10"
        };

         // StatusPickerViewModel lbl;
         //  private UILabel personLabel;

        //public StatusPickerViewModel(UILabel personLabel)
        //{
        //    this.personLabel = personLabel;
        //}
        //public StatusPickerViewModel(IList<string> values)
        //{
        //    this.values = values;
        //}

        public string SelectedItem
        {
            get { return values[selectedIndex]; }
        }
        //UIPickerView samplePicker;
        protected int selectedIndex;

        public void setSelectedIndex(int index)
        {
            this.selectedIndex = index;
        }

        public StatusPickerViewModel()
        {
            //   getList();
            //this.values = values;
        }
        //private void getList()
        //{
        //    for (int i = 0; i <= 10; i++)
        //    {
        //        string listValue = i.ToString();
        //        values.Add(listValue);
        //    }


        //}

        public StatusPickerViewModel(NSArray itemSourceArray, nint selectedItemIndex)
        {
            
            this.selectedIndex = (int)selectedItemIndex;

        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return values.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            // return values[(int)row];
            if (component == 0)
                return values[(int)row];
            else
                return row.ToString();
        }

       

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            selectedIndex = (int)row;

            PickerChanged.Invoke(this, Int32.Parse(this.SelectedItem));

        }

		public override nfloat GetComponentWidth(UIPickerView pickerView, nint component)
		{
            if (component == 0)
                return 240f;
            else
                return 40f;
		}
        public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
        {
            return 40f;
        }
      

		private class Picker
        {
            public object SelectedItem { get; internal set; }
        }
    }

    public class PickerChangedEventArgs : EventArgs
    {
        public string SelectedValue { get; set; }
    }
}
