using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
	public class InfoPanel : AquamonixView
    {
		protected const int LeftMargin = 16;
		protected const int RightMargin = 14;
		protected const int TopMargin = 11;
		protected const int BottomMargin = 20;
		protected const int LabelHeight = 20;

		protected static readonly FontWithColor NormalFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize6, Colors.StandardTextColor);
		protected static readonly FontWithColor BoldFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize6, Colors.StandardTextColor);
		protected readonly UITableView _dataTable = new UITableView();
		protected IEnumerable<DataTableRowViewModel> _rowValues;

		public int ContentHeight
		{
			get
			{
				return (int)(this._dataTable.ContentSize.Height + TopMargin + BottomMargin);
			}
		}

		public InfoPanel() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = Colors.LightGrayBackground;

				//sensors table
				this._dataTable.SeparatorStyle = UITableViewCellSeparatorStyle.None;
				this._dataTable.BackgroundColor = this.BackgroundColor;
				this._dataTable.ScrollEnabled = false;
				this._dataTable.AllowsSelection = false;

				_dataTable.RegisterClassForCellReuse(typeof(DataTableViewCell), DataTableViewCell.TableCellKey);

				this.AddSubviews(_dataTable);
			});
		}

		public int CalculateHeight()
		{
			return CalculateHeight(_rowValues);
		}


		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				//data table 
				this._dataTable.SetFrameWidth(this.Frame.Width);
				this._dataTable.SetFrameHeight(this._dataTable.ContentSize.Height);
				this._dataTable.SetFrameLocation(0, TopMargin);
			});
		}

		protected static int CalculateHeight(IEnumerable<DataTableRowViewModel> rowValues)
		{
			var rowHeight = (rowValues.Where((i) => !i.IsGroupName).Count() * DataTableViewSource.DefaultRowHeight);
			var headerRowHeight = (rowValues.Where((i) => i.IsGroupName).Count() * DataTableViewSource.RowHeaderHeight);
			var height = (rowHeight + headerRowHeight);

			if (height > 0)
				height += (TopMargin + BottomMargin);

			return height;
		}


		protected class DataTableViewSource : UITableViewSource
		{
			public const int DefaultRowHeight = 20;
			public const int RowHeaderHeight = 30;

			private List<DataTableRowViewModel> _rowValues;

			public DataTableViewSource(List<DataTableRowViewModel> rowValues)
			{
				_rowValues = rowValues;
			}

			public override nfloat GetHeightForHeader(UITableView tableView, nint section)
			{
				return 0;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				var item = (_rowValues[indexPath.Row]);
				if (item.IsGroupName)
					return RowHeaderHeight;

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
				return this._rowValues == null ? 0 : this._rowValues.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				return ExceptionUtility.Try<UITableViewCell>(() =>
				{
					var cell = (DataTableViewCell)tableView.DequeueReusableCell(DataTableViewCell.TableCellKey, indexPath);

					DataTableRowViewModel rowValue = null;
					if (indexPath.Row < _rowValues.Count)
						rowValue = _rowValues[indexPath.Row];

					cell.LoadCellValues(rowValue);

					return cell;
				});
			}
		}

		protected class DataTableViewCell : UITableViewCell
		{
			public const string TableCellKey = "DataTableViewCell";

			private readonly AquamonixLabel _nameLabel = new AquamonixLabel();
			private readonly AquamonixLabel _valueLabel = new AquamonixLabel();

			public DataTableViewCell(IntPtr handle) : base(handle)
			{
				ExceptionUtility.Try(() =>
				{
					this.BackgroundColor = Colors.LightGrayBackground;

					_nameLabel.SetFontAndColor(NormalFont);

					_valueLabel.SetFontAndColor(BoldFont);

					this.ContentView.AddSubviews(_nameLabel, _valueLabel);
				});
			}

			public void LoadCellValues(DataTableRowViewModel rowValue)
			{
				ExceptionUtility.Try(() =>
				{
					if (rowValue != null)
					{
						if (rowValue.IsGroupName)
						{
							this._nameLabel.Text = rowValue.LeftValue;
							this._nameLabel.SetFontAndColor(BoldFont);
							this._valueLabel.Hidden = true;
						}
						else
						{
							this._nameLabel.Text = rowValue.LeftValue;
							this._nameLabel.SetFontAndColor(new FontWithColor(NormalFont.Font, rowValue.LeftColor));
							this._valueLabel.Text = rowValue.RightValue;
							this._valueLabel.Hidden = false;
							this._valueLabel.SetFontAndColor(new FontWithColor(BoldFont.Font, rowValue.RightColor));
						}
					}
				});
			}

			public override void LayoutSubviews()
			{
				ExceptionUtility.Try(() =>
				{
					base.LayoutSubviews();

					//name label 
					if (this._valueLabel.Hidden)
					{
						this._nameLabel.SetFrameSize(this.ContentView.Frame.Width - LeftMargin - RightMargin, this.ContentView.Frame.Height);
						this._nameLabel.SetFrameLocation(LeftMargin, 4);
						this._nameLabel.EnforceMaxXCoordinate(this.ContentView.Frame.Width - RightMargin);
					}
					else
					{
						var maxNameWidth = (String.IsNullOrEmpty(this._valueLabel.Text) ? this.ContentView.Frame.Width - LeftMargin - RightMargin : 140);

						this._nameLabel.SetFrameSize(maxNameWidth, this.ContentView.Frame.Height);
						this._nameLabel.SetFrameLocation(LeftMargin, 0);
						this._nameLabel.EnforceMaxWidth(this._nameLabel.Frame.Width);

						//value label 
						this._valueLabel.SizeToFit();
						this._valueLabel.SetFrameHeight(this._nameLabel.Frame.Height);
						this._valueLabel.SetFrameLocation(this._nameLabel.Frame.Right, 0);
						this._valueLabel.EnforceMaxXCoordinate(this.Frame.Width - RightMargin);
					}
				});
			}
		}

		protected class DataTableRowViewModel
		{
			public bool IsGroupName { get; set; }
			public string LeftValue { get; set; }
			public string RightValue { get; set; }

			public UIColor LeftColor { get; set; }
			public UIColor RightColor { get; set; }
		}
	}
}
