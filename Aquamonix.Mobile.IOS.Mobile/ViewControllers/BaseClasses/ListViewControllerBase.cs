using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;


namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Base class for a certain type of top-level view controller in the app; these screens contain common elements like a 
    /// header, a table for the main body, a footer (and a few other elements). 
    /// Examples: 
    /// - Stations
    /// - Programs 
    /// - Device List 
    /// </summary>
	public abstract class ListViewControllerBase : TopLevelViewControllerBase
	{
		private static readonly FontWithColor NothingToDisplayFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, Colors.StandardTextColor);

        //for async progress updates 
        private bool _waitingForAsyncUpdate;
        private Action _asyncUpdateCallback;
		private readonly AquamonixLabel _emptyScreenLabel = new AquamonixLabel();

        /// <summary>
        /// Gets/sets the header view.
        /// </summary>
		public UIView HeaderView { get; set; }

        /// <summary>
        /// Gets/sets the table vc that makes up the main body.
        /// </summary>
		public TableViewControllerBase TableViewController { get; set; }

        /// <summary>
        /// Gets/sets the footer view.
        /// </summary>
		public UIView FooterView { get; set; }

        /// <summary>
        /// Gets/sets the top margin height.
        /// </summary>
		public int TopMargin { get; set; }

        /// <summary>
        /// Gets/sets the Device that's the focus of the screen. 
        /// </summary>
		public  DeviceDetailViewModel Device { get; set;}

        /// <summary>
        /// Gets/sets the id of the device.
        /// </summary>
		public string DeviceId { get; set;}

        /// <summary>
        /// Gets/sets the action that handles async device updates.
        /// </summary>
		protected Action AsyncUpdateCallback
		{
			get { return this._asyncUpdateCallback; }
			set { this._asyncUpdateCallback = WeakReferenceUtility.MakeWeakAction(value); }
		}


		public ListViewControllerBase() : base()
		{
		}

		public ListViewControllerBase(string deviceId) : base() 
		{ 
			ExceptionUtility.Try(() =>
			{
				this.DeviceId = deviceId;
				var device = DataCache.GetDeviceFromCache(deviceId);

				if (device != null)
				{
					this.Device = new DeviceDetailViewModel(device);
				}
			});

			ExceptionUtility.Try(() =>
			{
				this.Initialize();

                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.Activated.ToString()), (n) => { this.OnReconnected(); }); 
            });
		}


        /// <summary>
        /// Loads the subject data into the main table, and other screen data.
        /// </summary>
		public void LoadData()
		{
			ExceptionUtility.Try(() =>
			{
				System.Diagnostics.Debug.WriteLine(this.GetType().Name + " loading data");
                this.DoLoadData();
				this.SetEmptyScreen(); 
			});
		}

        /// <summary>
        /// Override to do the specific work of loading the screen data.
        /// </summary>
		protected virtual void DoLoadData()
		{
		}

		protected override void HandleViewWillAppear(bool animated)
		{
			base.HandleViewWillAppear(animated);
			this.LoadData();

			if (this.DeviceId != null)
				DataCache.SubscribeToDeviceUpdates(this.DeviceId, this.GotDeviceUpdate);

            this.TabBar.Hidden = (!this.ShowTabBar);
            this.ConnectionBar.Hidden = (!this.ShowConnectionBar);
        }

		protected override void HandleViewDidLayoutSubviews()
		{
			base.HandleViewDidLayoutSubviews();

			//header
			int y = Sizes.NavigationHeaderHeight;
			if (this.HeaderView != null)
			{
				//NOTE: the height of the header must be set beforehand in the derived class 
				this.HeaderView.SetFrameLocation(0, y);
				this.HeaderView.SetFrameWidth(this.PrimeView.Frame.Width);
				y = (int)this.HeaderView.Frame.Bottom;
			}

			int footerHeight = (this.FooterView == null ? 0 : Sizes.FooterHeight);
			int tabBarHeight = ((this.ShowTabBar || this.ShowConnectionBar) ? TabBarView.Height : 0);

			//table 
			if (this.TableViewController != null)
			{
				this.TableViewController.TableView.SetFrameLocation(0, y + TopMargin);
				this.TableViewController.TableView.SetFrameWidth(this.PrimeView.Frame.Width);
				this.TableViewController.TableView.SetFrameHeight(this.PrimeView.Frame.Height - y - footerHeight - tabBarHeight);
				this.TableViewController.TableView.SetHeightToContent();
			}

			//footer
			if (this.FooterView != null)
			{
				y = (int)this.PrimeView.Frame.Bottom - footerHeight;
				if (this.ShowTabBar || this.ShowConnectionBar)
					y -= tabBarHeight;

				this.FooterView.SetFrameLocation(0, y);
				this.FooterView.SetFrameSize(this.PrimeView.Frame.Width, footerHeight);
			}

			//empty screen label 
			if (!this._emptyScreenLabel.Hidden)
			{
				this._emptyScreenLabel.SetFrameWidth(this.PrimeView.Frame.Width - 24);
				this._emptyScreenLabel.SetFrameHeight(60);

				this._emptyScreenLabel.CenterHorizontallyInParent();
				this._emptyScreenLabel.SetFrameY(200);
            }

            if (this.ShowTabBar)
            {
                this.TabBar.SetFrameSize(this.PrimeView.Frame.Width, TabBarView.Height);
                this.TabBar.SetFrameLocation(0, this.PrimeView.Frame.Bottom - TabBarView.Height);
            }

            if (this.ShowConnectionBar)
            {
                this.ConnectionBar.SetFrameSize(this.PrimeView.Frame.Width, ConnectionStateDebugView.Height);
                this.ConnectionBar.SetFrameLocation(0, this.PrimeView.Frame.Bottom - (TabBarView.Height + ConnectionStateDebugView.Height));
            }
        }

        /// <summary>
        /// Callback that's called to handle an async device update.
        /// </summary>
        /// <param name="device">The device updated.</param>
		protected virtual void GotDeviceUpdate(Device device)
		{
			ExceptionUtility.Try(() =>
			{
				//handle async device update cleanup
				if (this._waitingForAsyncUpdate && this._asyncUpdateCallback != null)
					this._asyncUpdateCallback();
				this._waitingForAsyncUpdate = false;

				if (this.IsShowing)
				{
					LogUtility.LogMessage("Handling device update for device " + device.Id);
					this.Device = new DeviceDetailViewModel(device);

					MainThreadUtility.InvokeOnMain(() =>
					{
						this.LoadData();
					});
				}
			});
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			ExceptionUtility.Try(() =>
			{
				if (!String.IsNullOrEmpty(DeviceId))
					DataCache.UnsubscribeFromDeviceUpdates(this.DeviceId, this.GotDeviceUpdate);

				base.Dispose(disposing);
			});
		}

		protected override void InitializeViews()
		{
			base.InitializeViews();

            this._emptyScreenLabel.TextAlignment = UITextAlignment.Center;
			this._emptyScreenLabel.Hidden = true;
			this._emptyScreenLabel.LineBreakMode = UILineBreakMode.WordWrap;
			this._emptyScreenLabel.Lines = 0;
			this._emptyScreenLabel.SetFontAndColor(NothingToDisplayFont);

			this.PrimeView.AddSubview(this._emptyScreenLabel);
        }

        /// <summary>
        /// Override to determine when the screen is empty (and the 'empty-screen' message should be shown).
        /// </summary>
        /// <param name="emptyMessage">The message to display if the screen is empty.</param>
        /// <returns>True if the screen should be considered empty.</returns>
		protected virtual bool ScreenIsEmpty(out string emptyMessage)
		{
			emptyMessage = null;
			return false;
		}

        /// <summary>
        /// Shows the 'empty screen' message.
        /// </summary>
		protected void SetEmptyScreen()
		{
			string emptyMessage = null;
			this._emptyScreenLabel.Hidden = !this.ScreenIsEmpty(out emptyMessage);
			this._emptyScreenLabel.Text = String.IsNullOrEmpty(emptyMessage) ? "It looks like there's nothing to display here at the moment." : emptyMessage;
		}

		protected void RegisterForAsyncUpdate(Action callback, int waitMs = 0)
		{
			if (waitMs > 0)
				callback();
			else
			{
				this._waitingForAsyncUpdate = true;
				this.AsyncUpdateCallback = () =>
				{
					ExceptionUtility.Try(() => { 
						if (_waitingForAsyncUpdate)
							callback();
					});
				};

                //NOTE: here we know that waitMs is always 0; otherwise we would not be here. that causes ArgumentException. 
                // so I changed to hard-coded 1 (and why do we need a delay here at all?) 
                //this.AsyncUpdateCallback.RunAfter(waitMs);
                this.AsyncUpdateCallback.RunAfter(1);
            }
		}

        protected override void OnReconnected()
        {
            base.OnReconnected(); 

            MainThreadUtility.InvokeOnMain(() =>
            {
                if (this.IsShowing)
                {
                    if (this.Device?.Device != null)
                    {
                        ServiceContainer.DeviceService.RequestDevice(this.Device.Device, silentMode: true);
                    }
                }
            });
        }
    }
}
