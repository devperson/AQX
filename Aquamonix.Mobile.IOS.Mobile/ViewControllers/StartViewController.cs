using System;
using System.Threading.Tasks;
using System.Linq;

using UIKit;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Initial, UI-less view controller - active only on app startup.
    /// </summary>
	public partial class StartViewController : TopLevelViewControllerBase
	{
		private bool _attemptAutoLogin;

		public StartViewController(bool attemptAutoLogin = false) : base()
		{
			ExceptionUtility.Try(() =>
			{
				_attemptAutoLogin = attemptAutoLogin;
			});
		}

		public StartViewController(IntPtr handle) : base(handle)
		{
		}

		protected override void HandleViewDidLoad ()
		{
 			base.HandleViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

  			this.View.BackgroundColor = UIColor.White;
        }

        protected override void HandleViewDidAppear(bool animated)
		{
			base.HandleViewDidAppear(animated);

            //UIApplication.SharedApplication.KeyWindow.AddSubview(ReconnectingOverlay.Instance);

            //LocationUtility.DistanceInMetersFrom(13.736717, 100.523186);
				
			if (_attemptAutoLogin)
			{
				var deviceListVc = DeviceListViewController.CreateInstance();

				if (User.Current == null || !User.Current.HasConfig)
				{
					//redirect to Config VC 
					//this.NavigationController.PushViewController(ViewControllerFactory.GetViewController<UserConfigViewController>(), true);
					NavigateUserConfig();
				}
				else
				{
					ProgressUtility.SafeShow("Connecting", async () => 
					{
						var userConfigVc = UserConfigViewController.CreateInstance();

                        var response = await ServiceContainer.UserService.RequestConnection(
                            User.Current.Username,
                            User.Current.Password);
                        
                        MainThreadUtility.InvokeOnMain(() =>
						{
							ProgressUtility.Dismiss();

							//on connection error, redirect to config 
							if (response != null && response.IsSuccessful) 
							{
                                //redirect to device list 
								this.NavigateTo(deviceListVc, inCurrentNavController: false);
								//this.PresentViewController(new AquamonixNavController(deviceListVc), true, null);
							}
							else 
							{
                                //this.NavigateTo(userConfigVc, inCurrentNavController: false);

                                if (response?.ErrorBody != null && response.ErrorBody.ErrorType == Aquamonix.Mobile.Lib.Domain.Responses.ErrorResponseType.AuthFailure)
                                    Caches.ClearCachesForAuthFailure();

                                //is device cache valid?
                                if (DataCache.HasDevices)
                                {
                                    //redirect to device list 
                                    deviceListVc.ShowConnectionError = true; 
                                    this.NavigateTo(deviceListVc, inCurrentNavController: false);
                                }
                                else
                                {
                                    //redirect to user config
                                    userConfigVc.ShowConnectionError = true;
                                    this.NavigateTo(userConfigVc, inCurrentNavController: false);
                                }
                            }
						});
                    });
				}
			}
			else {
				this.PresentViewController(new StartViewController(true), false, null);
			}
        }

        protected override void ShowReconBar(string text = null)
        {
            //DO NOTHING; don't want the banner showing on this screen
        }
    }
}

