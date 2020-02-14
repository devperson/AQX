using System;
using System.Collections.Generic;

using UIKit;

using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;

//DONE: Stations, advanced view mode remove second line 
//DONE: Circuits, advanced view mode remove second line 
//DONE: Stations: disable Test button if more than one chumpy selected
//DONE: Make sure no prev buttons showing 
//DONE: Circuits: change STOP button text to STOP TEST 
//DONE: Programs: Start button centered vertically
//DONE: Programs: remove second line if not in advanced view mode 
//certain table cells should not show selection highlight 

//DONE: make sure that disposal is happening during navigation, and that disposed exceptions are handled 
//DONE: email sending with attachemnt  
//DONE: handle the device database & updating/subscribing (devices, stations, circuits, and programs) 

namespace Aquamonix.Mobile.IOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			//TODO: is this needed? also, set the NSArbitraryLoads thing to proper values 
			System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, 
			                                                                    System.Security.Cryptography.X509Certificates.X509Certificate certificate, 
			                                                                    System.Security.Cryptography.X509Certificates.X509Chain chain, 
			                                                                    System.Net.Security.SslPolicyErrors sslPolicyErrors) { 
				return true; 
			};

			//Xamarin.Insights.Initialize("7a20edd149b749d76bb6cc74e6a730302e208012");
			//AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			//set providers 
			Providers.FileUtility = new IOS.Utilities.FileUtilityIOS();
			Providers.AppSettingsUtility = new Aquamonix.Mobile.IOS.Utilities.AppSettingsUtilityIos();
			Providers.LogUtility = new Aquamonix.Mobile.Lib.Utilities.FileBasedLogUtility();
			Providers.ProgressUtility = new Aquamonix.Mobile.IOS.Utilities.BTProgressUtility();
			Providers.AlertUtility = new Aquamonix.Mobile.IOS.Utilities.AlertUtilityIos();
			Providers.JsonUtility = new Aquamonix.Mobile.Lib.Utilities.ServiceStackJsonUtility();
			Providers.LocationUtility = new Aquamonix.Mobile.IOS.Utilities.LocationUtilityIos();
			Providers.NotificationUtility = new Aquamonix.Mobile.IOS.Utilities.NotificationUtilityIos();

			//Aquamonix.Mobile.Lib.Database.LocalDb.InitializeDatabase(typeof(Application).Assembly);

			UIApplication.Main (args, null, "AppDelegate");
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;
			System.Diagnostics.Debug.WriteLine(exception.ToString());
			LogUtility.LogException(exception, "CurrentDomain_UnhandledException", LogSeverity.Error, new Dictionary<string, string> {
                {
					"Scope",
					"CurrentDomain_UnhandledException"
				}
			});
		}
	}
}