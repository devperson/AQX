//#define DETECT_SLEEP_CONDITION

using System;
using System.Threading;
using System.Runtime.InteropServices;

using Foundation;
using UIKit;

using Newtonsoft.Json;
using ServiceStack.Text;
using CoreFoundation;

#if INCLUDE_HOCKEYAPP
using HockeyApp.iOS; 
#endif

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using ObjCRuntime;
using Aquamonix.Mobile.IOS.ViewControllers;
using Aquamonix.Mobile.Lib.Services;

namespace Aquamonix.Mobile.IOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private const string SleepNotificationName = "com.apple.springboard.hasBlankedScreen";
		private const string HockeyAppIdentifier = "cd0bfb9d97f64062a52c9761286add7e";
        // class-level declarations
        //UINavigationController homecontroller;

        public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

#if INCLUDE_HOCKEYAPP
			Configure HockeyApp for crash reporting
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure(HockeyAppIdentifier);
			manager.StartManager();
			manager.Authenticator.AuthenticateInstallation();
#endif

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

			//User.Initialize();

			LogUtility.StartLogging();
			LogUtility.LogMessage("Starting app", LogSeverity.Info);

			//UIApplication.SharedApplication.Windows[0].RootViewController = new Aquamonix.Mobile.IOS.ViewControllers.StartViewController();

			DataCache.Initialize();
            UserCache.Initialize();

            if (User.Current != null && !String.IsNullOrEmpty(User.Current.ServerUri))
            {
                WebSocketsClient.ResetWebSocketsClientUrl(new WebSocketsClientIos(), User.Current.ServerUri);
                ConnectionManager.Initialize(); 
            }

            MainThreadUtility.Instance.SetMainThread(
				Thread.CurrentThread,
				action1 => InvokeOnMainThread(new Action(action1)),
				action2 => BeginInvokeOnMainThread(new Action(action2))
			);

            var window = new UIWindow(UIScreen.MainScreen.Bounds);
            	var navController = new Aquamonix.Mobile.IOS.ViewControllers.AquamonixNavController(new ViewControllers.StartViewController());

            LogUtility.LogMessage(String.Format("Screen bounds {0} x {1} (h x w)", UIKit.UIScreen.MainScreen.NativeBounds.Size.Height, UIKit.UIScreen.MainScreen.NativeBounds.Size.Width), LogSeverity.Info);

            if (UIApplication.SharedApplication.KeyWindow != null)
			{
				double top = 0;
				double bottom = 0;
				if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
				{
					top = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Top;
					bottom = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
				}

                LogUtility.LogMessage(String.Format("KeyWindow SafeAreaInsets {0},{1} (top,bottom)", top, bottom), LogSeverity.Info);
            }

            window.RootViewController = navController;
			window.MakeKeyAndVisible();

			#if DETECT_SLEEP_CONDITION
			CFNotificationCenter.Darwin.AddObserver(
			name: SleepNotificationName,
			objectToObserve: null,
			notificationHandler: (name, userInfo) =>
			{
				ExceptionUtility.Try(() =>
				{
					// this check should really only be necessary if you reuse this one callback method
					// for multiple Darwin notification events
					if (name.Equals(SleepNotificationName, StringComparison.Ordinal))
						Console.WriteLine("screen has either gone dark, or been turned back on!");
				}); 
			},
			suspensionBehavior: CFNotificationSuspensionBehavior.DeliverImmediately);
			#endif 

			return true;  
		}
       
        public override void OnResignActivation (UIApplication application)
		{
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
            LogUtility.LogMessage("OnResignActivation called, App moving to inactive state.");
        }

        public override void DidEnterBackground (UIApplication application)
		{
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            LogUtility.LogMessage("App entering background state.");

            /*
            if (ConnectionManager.IsReconnecting)
            {
                LogUtility.LogMessage("App entering background with reconnect process still running", LogSeverity.Warn); 
            }
            */
        }

        public override void WillEnterForeground (UIApplication application)
		{
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
            LogUtility.LogMessage("App will enter foreground");
        }

        public override void OnActivated (UIApplication application)
		{
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            LogUtility.LogMessage("OnActivated called, App is active.");
            ServiceContainer.InvalidateCache(); // invalidate cache after backgrounding app
            NotificationUtility.PostNotification(NotificationType.Activated);
        }

		public override void WillTerminate (UIApplication application)
		{
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
            LogUtility.LogMessage("App is terminating.");
        }
    }
} 


