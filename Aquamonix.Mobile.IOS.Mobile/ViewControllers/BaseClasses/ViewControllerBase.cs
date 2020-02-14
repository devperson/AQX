using System;

using UIKit;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Generic base class for all non-table viewcontrollers in app. Includes automatic logging & exception handling.
    /// </summary>
	public abstract class ViewControllerBase : UIViewController
	{
		public ViewControllerBase(string nibName, Foundation.NSBundle bundle) : base(nibName, null)
		{
		}

		public ViewControllerBase(IntPtr handle) : base(handle)
		{
		}

		public ViewControllerBase() : base()
		{
		}

		public sealed override void ViewDidLoad()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewDidLoad: " + this.GetType().Name);
                base.ViewDidLoad();
				this.HandleViewDidLoad();
			});
		}

		public sealed override void ViewWillUnload()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewWillUnload: " + this.GetType().Name);
				base.ViewWillUnload();
				this.HandleViewWillUnload();
			});
		}

		public sealed override void ViewDidUnload()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewDidUnload: " + this.GetType().Name);
				base.ViewDidUnload();
				this.HandleViewDidUnload();
			});
		}

		public sealed override void ViewWillAppear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewWillAppear: " + this.GetType().Name);
				base.ViewWillAppear(animated);
				this.HandleViewWillAppear(animated);
			});
		}

		public sealed override void ViewDidAppear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewDidAppear: " + this.GetType().Name);
                base.ViewDidAppear(animated);
				this.HandleViewDidAppear(animated);
			});
		}

		public sealed override void ViewDidDisappear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewDidDisappear: " + this.GetType().Name);
				base.ViewDidDisappear(animated);
				this.HandleViewDidDisappear(animated);
			});
		}

		public sealed override void ViewWillDisappear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewWillDisappear: " + this.GetType().Name);
				base.ViewWillDisappear(animated);
				this.HandleViewWillDisappear(animated);
			});
		}

		public sealed override void ViewWillLayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewWillLayoutSubviews: " + this.GetType().Name);
				base.ViewWillLayoutSubviews();
				this.HandleViewWillLayoutSubviews();
			});
		}

		public sealed override void ViewDidLayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("ViewDidLayoutSubviews: " + this.GetType().Name);
				base.ViewDidLayoutSubviews();
				this.HandleViewDidLayoutSubviews();
			});
		}

		public sealed override void DidReceiveMemoryWarning()
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("DidReceiveMemoryWarning: " + this.GetType().Name);
				base.DidReceiveMemoryWarning();
				LogUtility.LogMessage("VC Received memory warning", LogSeverity.Warn);
				this.HandleDidReceivedMemoryWarning();
			});
		}

		public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, Action completionHandler)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("Presenting View Controller: " + viewControllerToPresent.GetType().Name);
				base.PresentViewController(viewControllerToPresent, animated, completionHandler);

				if (this.NavigationController != null)
					this.NavigationController.Dispose();
			});
		}


		protected virtual void HandleViewWillAppear(bool animated) { }

		protected virtual void HandleViewDidAppear(bool animated) { }

		protected virtual void HandleViewWillDisappear(bool animated) { }

		protected virtual void HandleViewDidDisappear(bool animated) { }

		protected virtual void HandleViewDidLoad() { }

		protected virtual void HandleViewDidUnload() { }

		protected virtual void HandleViewWillUnload() { }

		protected virtual void HandleViewWillLayoutSubviews() { }

		protected virtual void HandleViewDidLayoutSubviews() { }

		protected virtual void HandleDidReceivedMemoryWarning() { }
	}
}

