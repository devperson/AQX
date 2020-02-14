using System;

using UIKit;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	public partial class ErrorViewController : TopLevelViewControllerBase
	{
		private static ErrorViewController _instance;
		private bool _isConnectionError;

		private ErrorViewController(bool connectionError) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this._isConnectionError = connectionError;
				this.Initialize();
			});
		}

		public static ErrorViewController CreateInstance(IApiResponse response = null, bool connectionError = true)
		{
			ExceptionUtility.Try(() =>
			{
				if (response != null)
					LogUtility.LogAppError(response?.ErrorBody); 
				
				if (_instance != null)
				{
					_instance.Dispose();
					_instance = null;
				}

				_instance = new ErrorViewController(connectionError);
			});

			return _instance;
   		}

		protected override void InitializeViewController()
		{
			ExceptionUtility.Try(() =>
			{
				base.InitializeViewController();

				this._retryButton.Hidden = (!this._isConnectionError);

				this._retryButton.TouchUpInside += (o, e) =>
				{
					LogUtility.LogMessage("User clicked retry button (error screen).");
					this.RetryConnection();
				};

				this._settingsButton.TouchUpInside += (o, e) =>
				{
					this.NavigateUserConfig();
				};
			});
		}

		private void RetryConnection()  
		{
			ProgressUtility.SafeShow("Connecting", () =>
			{
				WebSocketsClient.ResetWebSocketsClientUrl(new WebSocketsClientIos(), User.Current.ServerUri);
                ConnectionManager.Initialize(); 

				ServiceContainer.UserService.RequestConnection(User.Current.Username, User.Current.Password).ContinueWith((r) =>
				{
					MainThreadUtility.InvokeOnMain(() =>
					{
						ProgressUtility.Dismiss();

						if (r.Result != null && r.Result.IsSuccessful)
							this.NavigateHome();
						//else {
						//	if (r.Result.HasError)
						//		AlertUtility.ShowAppError(r.Result?.ErrorBody);
						//}
					});
				});
			}, true);
   		}
	}
}

