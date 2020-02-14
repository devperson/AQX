using System;

namespace Aquamonix.Mobile.IOS.UI
{
    /// <summary>
    /// UI utility for standardizing sizes of things.
    /// </summary>
	public static class Sizes
    {
        private static int _notchOffset = 0;
        //when we launch from storyboard, we have the notch to contend with, hence the extra vertical offset 

        public static readonly bool IsIPhoneX = (UIKit.UIScreen.MainScreen.NativeBounds.Size.Height == 2436);
        public static readonly bool IsIPhoneXMax = (UIKit.UIScreen.MainScreen.NativeBounds.Size.Height == 2688);
        public static readonly bool IsIPhoneXR = (UIKit.UIScreen.MainScreen.NativeBounds.Size.Height == 1792);

        public static int NotchOffset
        {
            get { return _notchOffset; }
        }

        public static int NavigationHeaderHeight
        {
            get { return 65 + NotchOffset; }
        }

        public static int FooterHeight
        {
            get { return 83 + NotchOffset; }
        }

        public const int StandardButtonHeight = 55;

		public const int FontSize1 = 10;
		public const int FontSize2 = 11;
		public const int FontSize3 = 12;
		public const int FontSize4 = 13;
		public const int FontSize5 = 14;
		public const int FontSize6 = 15;
		public const int FontSize7 = 16;
		public const int FontSize8 = 17;
		public const int FontSize9 = 18;
		public const int FontSize10 = 19;
		public const int FontSize11 = 20;
		public const int FontSize12 = 21;

        static Sizes()
        {
            #if __LAUNCH_STORYBOARD__
            if (IsIPhoneX || IsIPhoneXMax || IsIPhoneXR)
            {
                _notchOffset = 24; 
            }
            #endif
        }
	}
}
