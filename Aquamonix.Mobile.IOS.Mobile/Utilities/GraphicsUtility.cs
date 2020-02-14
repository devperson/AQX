using System;

using UIKit;
using CoreGraphics;

using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;

namespace Aquamonix.Mobile.IOS.Utilities
{
	public static class GraphicsUtility
	{
		public static UIImage ImageForColorBar(SeverityLevel severityLevel)
		{
			switch (severityLevel)
			{
				case SeverityLevel.Missing:
					return Images.StatusGray;
					
				case SeverityLevel.None:
					return Images.StatusGray;
					
				case SeverityLevel.Normal:
					return Images.StatusGreen;
					
				case SeverityLevel.Low:
					return Images.StatusOrange;
					
				case SeverityLevel.Medium:
					return Images.StatusOrange;
					
				case SeverityLevel.High:
					return Images.StatusRed;
					
				case SeverityLevel.Extreme:
					return Images.StatusRed;
			}

			return Images.StatusGray;
		}

		public static UIImage IconForDeviceBadge(DeviceBadgeViewModel value)
		{
			//bool showRedIcon = (int)(value.SeverityLevel) >= (int)SeverityLevel.High;
			//var color = TextColorForSeverity(value.SeverityLevel);

			switch (value.Type)
			{
				case "Alerts": 
					return Images.AlertsIcons[IconColorForSeverity(value.SeverityLevel)];
					/*
					if (showRedIcon)
					{
						//TODO: red or gray based on status ?
					}
					return Images.AlertsIcon;
					*/

				case "Programs":
					return Images.ProgramsIcons[IconColorForSeverity(value.SeverityLevel)];
					/*
					if (showRedIcon)
						return Images.ProgramsIconRed;
					else
						return Images.ProgramsIcon;
					*/

				case "ProgramsSetToRun":
					return Images.ProgramsIcons[IconColorForSeverity(value.SeverityLevel)];
					/*
					if (showRedIcon)
						return Images.ProgramsIconRed;
					else
						return Images.ProgramsIcon;
					*/

				case "Droplet":
					return Images.ProgramsPercentIcons[IconColorForSeverity(value.SeverityLevel)];
					/*
					if (showRedIcon)
						return Images.ProgramPercentIconRed;
					else
						return Images.ProgramPercentIcon;
						*/
			}

			return CreateColoredCircle(TextColorForSeverity(value.SeverityLevel), 8);
		}

		public static UIColor TextColorForSeverity(SeverityLevel severityLevel)
		{
			switch (severityLevel)
			{
				case SeverityLevel.Missing:
					return Colors.StandardTextColor; 

				case SeverityLevel.None:
					return Colors.StandardTextColor;

				case SeverityLevel.Normal:
					return Colors.GreenTextColor;

				case SeverityLevel.Low:
					return Colors.OrangeTextColor;

				case SeverityLevel.Medium:
					return Colors.OrangeTextColor;

				case SeverityLevel.High:
					return Colors.ErrorTextColor;

				case SeverityLevel.Extreme:
					return Colors.ErrorTextColor;
			}

			return Colors.StandardTextColor;
		}

		public static IconColor IconColorForSeverity(SeverityLevel severityLevel)
		{
			switch (severityLevel)
			{
				case SeverityLevel.Missing:
					return IconColor.Gray;

				case SeverityLevel.None:
					return IconColor.Gray;

				case SeverityLevel.Normal:
					return IconColor.Green;

				case SeverityLevel.Low:
					return IconColor.Orange;

				case SeverityLevel.Medium:
					return IconColor.Orange;

				case SeverityLevel.High:
					return IconColor.Red;

				case SeverityLevel.Extreme:
					return IconColor.Red;
			}

			return IconColor.Gray;
		}

		public static UIImage CreateColoredRect(UIColor color, CGSize size)
		{
			UIGraphics.BeginImageContext(size);
			CGContext ctxt = UIGraphics.GetCurrentContext();

			UIImage img = null;
			if (ctxt != null)
			{
				ctxt.SetFillColor(color.CGColor);
				ctxt.FillRect(new CGRect(new CGPoint(0, 0), size));

				img = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}

			return img;
		}

		public static UIImage CreateColoredCircle(UIColor color, int diameter)
		{
			var size = new CGSize(diameter, diameter);
			UIGraphics.BeginImageContext(size);
			CGContext ctxt = UIGraphics.GetCurrentContext();

			UIImage img = null;
			if (ctxt != null)
			{
				ctxt.SetFillColor(color.CGColor);
				ctxt.FillEllipseInRect(new CGRect(new CGPoint(0, 0), size)); 

				img = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}

			return img;
		}

        public static UIColor ColorForReconBar(ReconnectingView.DisplayMode displayMode)
        {
            switch(displayMode)
            {
                case ReconnectingView.DisplayMode.Connected:
                    return Colors.AquamonixGreen;  //UIColor.Green;
                case ReconnectingView.DisplayMode.Reconnecting:
                    return UIColor.Orange;
                case ReconnectingView.DisplayMode.ServerDown:
                    return UIColor.Red;
            }

            return UIColor.Orange;
        }
	}
}
