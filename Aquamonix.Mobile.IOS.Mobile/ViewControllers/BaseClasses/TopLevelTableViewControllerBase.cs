using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Base class for UITableViewControllers that are the top-level view controller on the screen. 
    /// </summary>
	public abstract class TopLevelTableViewControllerBase<TValue, TSource> : TableViewControllerBase 
		where TSource : TableViewSourceBase<TValue>
	{
		private IEnumerable<TValue> _values = new List<TValue>();

		protected IEnumerable<TValue> Values
		{
			get { return this._values; }
		}

		public TSource TableViewSource
		{
			get
			{
				if (this.TableView != null)
					return (TSource)this.TableView.Source;

				return null;
			}
		}

		public bool Disposed { get; private set; }

		public virtual bool ShouldDispose { get { return true; } }


		public TopLevelTableViewControllerBase() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                //this.TableView.ScrollEnabled = true; // reverse change from build 84
            });
		}

		public void LoadData(IEnumerable<TValue> values)
		{
			ExceptionUtility.Try(() =>
			{
				_values = values;

				if (_values == null)
					_values = new List<TValue>();

				this.TableViewSource.Values = _values.ToList();
				TableView.ReloadData();
			});
   		}

		protected override void HandleViewDidLoad()
		{
			base.HandleViewDidLoad();

			if (this._values == null)
				this._values = new List<TValue>();

			TSource source = this.CreateTableSource(this._values.ToList());

			TableView.Source = source;
			TableView.SectionFooterHeight = 0;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
		}

		protected abstract TSource CreateTableSource(IList<TValue> values);

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
	}
}

