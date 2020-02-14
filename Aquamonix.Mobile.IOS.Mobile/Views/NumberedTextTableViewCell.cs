using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;
using System.Collections.Generic;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.Views
{
    public class NumberedTextTableViewCell : TableViewCellBase
    {
        protected const int LeftMargin = 4;
        protected const int LeftTextMargin = 42;
        protected const int RightMargin = 10;

        private static readonly FontWithColor FirstLineFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);
        private static readonly FontWithColor SecondLineFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize4, Colors.StandardTextColor);

        private readonly AquamonixLabel _numberLabel = new AquamonixLabel();
        private readonly AquamonixLabel _firstLineLabel = new AquamonixLabel();
        private readonly AquamonixLabel _secondLineLabel = new AquamonixLabel();

        protected AquamonixLabel NumberLabel
        {
            get { return _numberLabel; }
        }

        protected AquamonixLabel FirstLineLabel
        {
            get { return _firstLineLabel; }
        }

        protected AquamonixLabel SecondLineLabel
        {
            get { return _secondLineLabel; }
        }

        public NumberedTextTableViewCell(IntPtr handle) : base(handle)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.SetFontAndColor(FirstLineFont);
                this._numberLabel.TextAlignment = UITextAlignment.Right;

                this._firstLineLabel.SetFontAndColor(FirstLineFont);

                this._secondLineLabel.SetFontAndColor(SecondLineFont);

                this.ContentView.AddSubviews(_numberLabel, _secondLineLabel, _firstLineLabel);
            });
        }

        public void LoadCellValues(string number, string firstLine, string secondLine)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.Text = number;
                this._numberLabel.SizeToFit();

                this._firstLineLabel.Text = firstLine;
                this._firstLineLabel.SizeToFit();

                this._secondLineLabel.Text = secondLine;
                this._secondLineLabel.SizeToFit();
            });
        }

        public void LoadCellValues(string number, string firstLine, string secondLine, ItemsDictionary<StepsItems> stepsItems)
        {
            ExceptionUtility.Try(() =>
            {
                this._numberLabel.Text = number;
                this._numberLabel.SizeToFit();

                this._firstLineLabel.Text = firstLine;
                this._firstLineLabel.SizeToFit();

                this._secondLineLabel.Text = secondLine;
                this._secondLineLabel.SizeToFit();
                //int Ypos = 30;
                //foreach(var item in stepsItems)
                //{
                  
                //    AquamonixLabel StepLabel = new AquamonixLabel();
                //    StepLabel.Text = item.Key.ToString();
                //    StepLabel.SizeToFit();
                //    StepLabel.SetFrameLocation(this._firstLineLabel.Frame.X, Ypos);
                //    this.ContentView.AddSubviews(StepLabel);
                //    Ypos = Ypos + 30;
                   
                //}

            });
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

            this._numberLabel.SetFrameHeight(30);
            this._numberLabel.SetFrameWidth(33);
            this._numberLabel.SetFrameX(LeftMargin);
            this._firstLineLabel.SetFrameX(LeftTextMargin);
            this._firstLineLabel.SetFrameHeight(30);

            if (!String.IsNullOrEmpty(this._secondLineLabel.Text))
            {
                this._numberLabel.SetFrameY(5);

                this._firstLineLabel.SetFrameY(this._numberLabel.Frame.Y);

                this._secondLineLabel.SetFrameHeight(25);
                this._secondLineLabel.SetFrameLocation(this._firstLineLabel.Frame.X, 25);
                this._secondLineLabel.Hidden = false;
            }
            else
            {
                this._firstLineLabel.CenterVerticallyInParent();
                this._numberLabel.SetFrameY(this._firstLineLabel.Frame.Y);
                this._secondLineLabel.Hidden = true;
            }

            this._numberLabel.EnforceMaxXCoordinate(this._firstLineLabel.Frame.X);
            this._firstLineLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
            this._secondLineLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
        }
       
    }
}
