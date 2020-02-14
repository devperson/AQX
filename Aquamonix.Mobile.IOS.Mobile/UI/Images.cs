using System;
using System.Collections.Generic;

using UIKit;

namespace Aquamonix.Mobile.IOS.UI
{
    /// <summary>
    /// UI utility for images
    /// </summary>
	public static class Images
	{
		public readonly static UIImage BackArrow = UIImage.FromFile("Images/Arrow.png"); 
		public readonly static UIImage StatusGray = UIImage.FromFile("Images/statusgray.png");
		public readonly static UIImage StatusRed = UIImage.FromFile("Images/statusred.png");
		public readonly static UIImage StatusGreen = UIImage.FromFile("Images/statusgreen.png");
		public readonly static UIImage StatusOrange = UIImage.FromFile("Images/statusorange.png");
		public readonly static UIImage AlertsIcon = UIImage.FromFile("Images/alerts.png");
		public readonly static UIImage AlertsIconRed = UIImage.FromFile("Images/alerts_red.png");
		public readonly static UIImage AlertsIconOrange = UIImage.FromFile("Images/alerts_orange.png");
		public readonly static UIImage AlertsIconGreen = UIImage.FromFile("Images/alerts_green.png");
		public readonly static UIImage ProgramPercentIcon = UIImage.FromFile("Images/water.png");
		public readonly static UIImage ProgramPercentIconRed = UIImage.FromFile("Images/water_red.png");
		public readonly static UIImage ProgramPercentIconOrange = UIImage.FromFile("Images/water_orange.png");
		public readonly static UIImage ProgramPercentIconGreen = UIImage.FromFile("Images/water_green.png");
		public readonly static UIImage ProgramsIcon = UIImage.FromFile("Images/programs.png");
		public readonly static UIImage ProgramsIconRed = UIImage.FromFile("Images/programs_red.png");
		public readonly static UIImage ProgramsIconGreen = UIImage.FromFile("Images/programs_green.png");
		public readonly static UIImage ProgramsIconOrange = UIImage.FromFile("Images/programs_orange.png");
		public readonly static UIImage TableRightArrow = UIImage.FromFile("Images/Carrot.png");
		public static readonly UIImage GreenCheckboxChecked = UIImage.FromFile("Images/selectedgreen.png");
		public static readonly UIImage GreenCheckboxUnchecked = UIImage.FromFile("Images/selectgreen.png");
		public static readonly UIImage BrownCheckboxChecked = UIImage.FromFile("Images/selected.png");
		public static readonly UIImage BrownCheckboxUnchecked = UIImage.FromFile("Images/select.png");
		public static readonly UIImage AquamonixLogo = UIImage.FromFile("Images/logo.png");
        public static readonly UIImage PivotImage = UIImage.FromFile("Images/circle.png");
        public static readonly UIImage Farrow = UIImage.FromFile("Images/active_forward.png");
		public static readonly UIImage RevImage = UIImage.FromFile("Images/rev.png");
		public static readonly UIImage Wet = UIImage.FromFile("Images/wet.png");
		public static readonly UIImage Dry = UIImage.FromFile("Images/disable_wet.png");
		public static readonly UIImage Speed = UIImage.FromFile("Images/speed.png");

		public static readonly UIImage Running = UIImage.FromFile("Images/running.png");
        public static readonly UIImage Waiting = UIImage.FromFile("Images/waiting.png");
		public static readonly UIImage Stop = UIImage.FromFile("Images/stop.png");
        public static readonly UIImage Slice = UIImage.FromFile("Images/arrow_1.png");
        public static readonly UIImage PartCircle = UIImage.FromFile("Images/partcircle.png");
		public static readonly UIImage MainImage = UIImage.FromFile("Images/image_1.png");

        public static readonly UIImage PowerOff = UIImage.FromFile("Images/powerOff.png");
        public static readonly UIImage PowerOn = UIImage.FromFile("Images/poweron.png");
        public static readonly Dictionary<IconColor, UIImage> ProgramsIcons = new Dictionary<IconColor, UIImage>();
		public static readonly Dictionary<IconColor, UIImage> ProgramsPercentIcons = new Dictionary<IconColor, UIImage>();
		public static readonly Dictionary<IconColor, UIImage> AlertsIcons = new Dictionary<IconColor, UIImage>();

		static Images()
		{
			ProgramsIcons.Add(IconColor.Green, ProgramsIconGreen);
			ProgramsIcons.Add(IconColor.Red, ProgramsIconRed);
			ProgramsIcons.Add(IconColor.Orange, ProgramsIconOrange);
			ProgramsIcons.Add(IconColor.Gray, ProgramsIcon);

			ProgramsPercentIcons.Add(IconColor.Green, ProgramPercentIconGreen);
			ProgramsPercentIcons.Add(IconColor.Red, ProgramPercentIconRed);
			ProgramsPercentIcons.Add(IconColor.Orange, ProgramPercentIconOrange);
			ProgramsPercentIcons.Add(IconColor.Gray, ProgramPercentIcon);

			AlertsIcons.Add(IconColor.Green, AlertsIconGreen);
			AlertsIcons.Add(IconColor.Red, AlertsIconRed);
			AlertsIcons.Add(IconColor.Orange, AlertsIconOrange);
			AlertsIcons.Add(IconColor.Gray, AlertsIcon);
		}
	}

	public enum IconColor
	{
		Red, Green, Orange, Gray
	}
}
