using System;
using System.Linq;
using System.Threading.Tasks;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.ViewModels;
using Foundation;
using UIKit;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Base class for UIViewControllers that are the top-level viewcontroller for a screen.
    /// </summary>
    public abstract class TopLevelViewControllerBase : ViewControllerBase
    {
        public static TopLevelViewControllerBase CurrentViewController { get; set;}
        private AlertViewModel AlertID;
        protected TabBarView TabBar;
        protected ConnectionStateDebugView ConnectionBar;
        protected ReconnectingView ReconBar;

        public NavigationBarView NavigationBarView { get; set; }

        public bool ShowTabBar { get; set; }

        public bool ShowConnectionBar { get; set; }

        public bool Disposed { get; private set; }

        public virtual bool ShouldDispose { get { return true; } }

        public bool IsShowing
        {
            get {
                return (CurrentViewController != null && Object.ReferenceEquals(this, CurrentViewController));
            }
        }

        public UIViewController Predecessor { get; set; }

        protected virtual nfloat ReconBarVerticalLocation
        {
            get { return 60 + Sizes.NotchOffset; }
        }

        public UIView PrimeView { get; private set; }

        public bool ShowConnectionError { get; set; }


        public TopLevelViewControllerBase(string nibName, Foundation.NSBundle bundle,AlertViewModel alert) : base(nibName, null)
        {
            ExceptionUtility.Try(() =>
            {
                SubscribeConnectionEvents();
                this.TabBar = new TabBarView(this);
                this.ConnectionBar = new ConnectionStateDebugView(this);
                this.ReconBar = new ReconnectingView(this); 
                AlertID = alert;
            });
        }

        public TopLevelViewControllerBase(IntPtr handle) : base(handle)
        {
            ExceptionUtility.Try(() =>
            {
                SubscribeConnectionEvents();
                this.TabBar = new TabBarView(this);
                this.ConnectionBar = new ConnectionStateDebugView(this);
                this.ReconBar = new ReconnectingView(this);
            });
        }

        public TopLevelViewControllerBase() : base()
        {
            ExceptionUtility.Try(() =>
            {
                SubscribeConnectionEvents();
                this.TabBar = new TabBarView(this);
                this.ConnectionBar = new ConnectionStateDebugView(this);
                this.ReconBar = new ReconnectingView(this);
            });
        }


        public void SetNavigationBarView(NavigationBarView view)
        {
            ExceptionUtility.Try(() =>
            {
                this.ClearNavigationBarView();

                if (this.NavigationController != null)
                {
                    if (this.NavigationController.NavigationBar != null)
                    {
                        if (view != null)
                            this.NavigationController.NavigationBar.AddSubview(view);
                    }
                }
            });
        }

        public void ClearNavigationBarView()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.NavigationController != null)
                {
                    if (this.NavigationController.NavigationBar != null)
                    {
                        foreach (var view in this.NavigationController.NavigationBar.Subviews)
                        {
                            if (view is NavigationBarView)
                                view.RemoveFromSuperview();
                        }
                    }
                }
            });
        }

        public void SetCustomBackButton()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.NavigationItem != null)
                {
                    this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.BackArrow, UIBarButtonItemStyle.Plain, (o, e) =>
                    {
                        if (this.NavigationController != null)
                           
                            this.NavigationController.PopViewController(true);
                    });
                }
            });
        }

        public virtual void NavigateUserConfig()
        {
            ExceptionUtility.Try(() =>
            {
                //PresentViewController(new AquamonixNavController(UserConfigViewController.CreateInstance()), true, null);
                this.NavigateTo(UserConfigViewController.CreateInstance(this), inCurrentNavController: false);
            });
        }

        public virtual void NavigateHome()
        {
            ExceptionUtility.Try(() =>
            {
                if (User.Current != null && User.Current.HasConfig && DataCache.HasDevices)
                    this.NavigateTo(DeviceListViewController.CreateInstance(), inCurrentNavController: false);
                    //PresentViewController(new AquamonixNavController(DeviceListViewController.CreateInstance()), true, null);
            });
        }

        public virtual void NavigateAlerts()
        {
            ExceptionUtility.Try(() =>
            {
                LogUtility.LogMessage("Navigating to Alerts");

                if (User.Current != null && User.Current.HasConfig && DataCache.HasDevices)
                {
                    ProgressUtility.SafeShow("Getting Alerts", () =>
                    {
                        var devices = DataCache.GetAllDevicesFromCache();
                        ServiceContainer.AlertService.RequestGlobalAlerts(devices.Select((d) => d.Id), NavigateAlerts).ContinueWith((r) =>
                        {
                            MainThreadUtility.InvokeOnMain(() =>
                            {
                                ProgressUtility.Dismiss();

                                if (r.Result != null && r.Result.IsSuccessful)
                                {
                                    this.NavigateTo(GlobalAlertListViewController.CreateInstance(), inCurrentNavController:false);
                                }
                                //else {
                                //    AlertUtility.ShowAppError(r.Result?.ErrorBody);
                                //}
                            });
                        });
                    },true);
                }
            });
           }

        public virtual void NavigateTo(UIViewController viewController, bool inCurrentNavController = true, bool animated = true)
        {
            LogUtility.LogMessage("Navigating to " + viewController.GetType().Name);

            MainThreadUtility.InvokeOnMain(() =>
            {
                this.WillNavigateAway(viewController);
                if (inCurrentNavController)
                {
                    this.NavigationController.PushViewController(viewController, animated);
                }
                else 
                {
                    PresentViewController(new AquamonixNavController(viewController), animated, null);
                }
            });
        }


        protected void Initialize()
        {
            ExceptionUtility.Try(() =>
            {
                this.InitializeViews();
                this.InitializeViewController();

                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.AuthFailure.ToString()), (ns) => { OnAuthFailure(); });
                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.Reconnected.ToString()), (ns) => { OnReconnected(); });
            });
        }

        protected override void HandleViewDidAppear(bool animated)
        {
            base.HandleViewDidAppear(animated);
        }

        protected override void HandleViewWillAppear(bool animated)
        {
            CurrentViewController = this;
            base.HandleViewWillAppear(animated);

            this.TabBar.Hidden = (!this.ShowTabBar);
            this.ConnectionBar.Hidden = (!this.ShowConnectionBar);

            this.ConditionalShowReconBar();

            this.SetNavigationBarView(this.NavigationBarView);

            if (this.TabBar != null)
                this.TabBar.SetAlertsButtonImage(); 
        }

        protected override void HandleViewDidDisappear(bool animated)
        {
            base.HandleViewDidDisappear(animated);
        }

        protected virtual void ConditionalShowReconBar()
        {
            //manage reconnecting bar view 


            if (ConnectionManager.ShowingReconBar || this.ShowConnectionError)
                this.ShowReconBar();
            else
                this.HideReconBar(); 

            //this.ShowReconnectingBar();
            //WaitAndShow();
        }

        protected virtual void OnAuthFailure()
        {
            ExceptionUtility.Try(() =>
            {
                if (this.IsShowing)
                {
                    //cancel reconnecting 
                    ConnectionManager.CancelReconnecting();

                    //clear cache
                    Caches.ClearCachesForAuthFailure();

                    MainThreadUtility.InvokeOnMain(() => {

                        //show alert
                        ProgressUtility.Dismiss();
                        AlertUtility.ShowAlert(StringLiterals.AuthFailure, StringLiterals.AuthFailureMessage);

                        //nav to login screen 
                        this.NavigateUserConfig();
                    });
                }
            });
        }

        protected virtual void OnReconnected()
        {
            //override 
        }

        protected override void HandleViewDidLoad()
        {
            base.HandleViewDidLoad();
        }

        protected override void HandleViewDidLayoutSubviews()
        {
            base.HandleViewDidLayoutSubviews();

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

        protected virtual void InitializeViewController()
        {
        
        }

        protected virtual void InitializeViews()
        {
            ExceptionUtility.Try(() =>
            {
                //this.ShowConnectionBar = true;
                this.PrimeView = new UIView(this.View.Frame);

                this.PrimeView.BackgroundColor = UIColor.White;
                TabBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin;
                ConnectionBar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin;

                this.View.AddSubview(this.PrimeView); 
                this.PrimeView.AddSubviews(TabBar);
                this.PrimeView.AddSubviews(ConnectionBar);
            });
        }

        protected virtual void WillNavigateAway(UIViewController destination) { }

        protected override void Dispose(bool disposing)
        {
            ExceptionUtility.Try(() =>
            {
                if (!this.Disposed)
                {
                    this.Disposed = true;

                    MainThreadUtility.BeginInvokeOnMain(() =>
                    {
                        ExceptionUtility.Try(() =>
                        {
                            if (this.View != null)
                                this.View.DisposeEx();
                        });
                    });
                }

                base.Dispose(disposing);
            });
        }

        protected virtual void ShowReconBar(string text = null)
        {
            MainThreadUtility.InvokeOnMain(() =>
            {
                //if reconnecting view has been disposed, take care of it 
                if (this.ReconBar.IsDisposed)
                {
                    try { this.ReconBar.RemoveFromSuperview(); }
                    catch (Exception) { }
                    this.ReconBar = new ReconnectingView(this);
                }

                if (String.IsNullOrEmpty(text))
                    text = StringLiterals.Reconnecting;

                ReconnectingView.DisplayMode displayMode = ReconnectingView.DisplayMode.Reconnecting;
                if (ConnectionManager.State.IsDead) {
                    displayMode = ReconnectingView.DisplayMode.ServerDown;
                    text = StringLiterals.ServerNotAvailable;
                }

                this.ReconBar.SetFrameSize(this.PrimeView.Frame.Width, ReconnectingView.Height);
                this.ReconBar.SetFrameLocation(0, this.ReconBarVerticalLocation);
                this.ReconBar.UserInteractionEnabled = false;
                this.PrimeView.AddSubview(this.ReconBar);
                this.ReconBar.SetTextAndMode(text, displayMode);
                this.PrimeView.BringSubviewToFront(this.ReconBar);

                this.AfterShowReconBar();
            });
        }

        protected virtual void HideReconBar()
        {
            MainThreadUtility.InvokeOnMain(async () =>
            {
                this.ReconBar.SetTextAndMode(null, ReconnectingView.DisplayMode.Connected);

                await Task.Delay(1500);
                MainThreadUtility.InvokeOnMain(() => {
                    this.ReconBar.RemoveFromSuperview();
                    this.AfterHideReconBar();
                });
            });
        }

        protected virtual void AfterShowReconBar()
        {

        }

        protected virtual void AfterHideReconBar()
        {

        }

        protected virtual void AdjustTableForReconBar(TableViewControllerBase tableVc, bool show)
        {
            var tableView = tableVc.TableView;
            if (show)
            {
                if (tableVc.AllRowsVisible.HasValue)
                {
                    if (!tableVc.AllRowsVisible.Value)
                    {
                        tableView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);
                        tableView.SizeToFit();
                    }
                    else
                    {
                        tableView.ScrollEnabled = true;
                        tableView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);
                        //tableView.SetFrameHeight(tableView.Frame.Height + 50);
                    }
                }
            }
            else
            {
                tableView.ContentInset = UIEdgeInsets.Zero;
                if (tableVc.AllRowsVisible.HasValue && tableVc.AllRowsVisible.Value)
                {
                    tableView.ScrollEnabled = false;
                }
            }
        }


        private void SubscribeConnectionEvents()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.ShowReconnecting.ToString()), (ns) => { 
                ShowReconBar(); 
            });
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationType.HideReconnecting.ToString()), (ns) => { 
                HideReconBar(); 
            });
        }
    }
}

