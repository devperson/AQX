using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// Base class for UITableViewSource classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class TableViewSourceBase<T> : UITableViewSource
	{
		public const int DefaultRowHeight = 55;

		public IList<T> Values { get; set; }

		public TableViewSourceBase() : base()
		{
			this.Values = new List<T>();
		}

		public TableViewSourceBase(IList<T> values) : base()
		{
			if (values == null)
				values = new List<T>();
			
			this.Values = values;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			return 0;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return DefaultRowHeight;
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			return null;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.Values == null ? 0 : this.Values.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return ExceptionUtility.Try<UITableViewCell>(() =>
			{
				var cell = this.GetCellInternal(tableView, indexPath);
				if (cell != null)
				{
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				return cell;
			});
		}

		protected abstract UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath);
	}
}

