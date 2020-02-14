using System;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Generic base class for all UITableViewControllers in the app; contains automatic exception handling and logging.
    /// </summary>
	public abstract class TableViewControllerBase : UITableViewController 
	{
        public bool? AllRowsVisible
        {
            get {
                return ExceptionUtility.Try(() => {
                    NSIndexPath[] visibleRows = this.TableView.IndexPathsForVisibleRows;

                    if (this.TableView?.Source != null)
                    {
                        nint totalRows = this.TableView.Source.RowsInSection(this.TableView, 0);

                        if (visibleRows != null)
                            return (visibleRows.Length == totalRows);
                    }
                    return (bool?)null;
                });
            }
        }

		public TableViewControllerBase() : base()
		{
            this.TableView.EstimatedRowHeight = 0; // fix for table sizing and scrolling issue
		}

		public TableViewControllerBase(IntPtr handle) : base(handle)
		{
            this.TableView.EstimatedRowHeight = 0; // fix for table sizing and scrolling issue
        }

		public sealed override void ViewDidLoad()
		{
			ExceptionUtility.Try(() =>
			{
                base.ViewDidLoad();
				this.HandleViewDidLoad();
			});
		}

		public sealed override void ViewWillUnload()
		{
			ExceptionUtility.Try(() =>
			{
				base.ViewWillUnload();
				this.HandleViewWillUnload();
			});
		}

		public sealed override void ViewDidUnload()
		{
			ExceptionUtility.Try(() =>
			{
				base.ViewDidUnload();
				this.HandleViewDidUnload();
			});
		}

		public sealed override void ViewWillAppear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				base.ViewWillAppear(animated);
				this.HandleViewWillAppear(animated);
			});
		}

		public sealed override void ViewDidAppear(bool animated)
		{
			ExceptionUtility.Try(() =>
			{
                base.ViewDidAppear(animated);
				this.HandleViewDidAppear(animated);
			});
		}

		public sealed override void ViewWillLayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.ViewWillLayoutSubviews();
				this.HandleViewWillLayoutSubviews();
			});
		}

		public sealed override void ViewDidLayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.ViewDidLayoutSubviews();
				this.HandleViewDidLayoutSubviews();
			});
		}

		public sealed override void DidReceiveMemoryWarning()
		{
			ExceptionUtility.Try(() =>
			{
				base.DidReceiveMemoryWarning();
				LogUtility.LogMessage("VC Received memory warning", LogSeverity.Warn);
				this.HandleDidReceivedMemoryWarning();
			});
		}

		protected virtual void HandleViewWillAppear(bool animated) { }

		protected virtual void HandleViewDidAppear(bool animated) { }

		protected virtual void HandleViewDidLoad() { }

		protected virtual void HandleViewDidUnload() { }

		protected virtual void HandleViewWillUnload() { }

		protected virtual void HandleViewWillLayoutSubviews() { }

		protected virtual void HandleViewDidLayoutSubviews() { }

		protected virtual void HandleDidReceivedMemoryWarning() { }
	}
}

