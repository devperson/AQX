using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;

//MPG no prog scale 
//IrrigationPrev 

using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

//TODO: make this inherit from InfoPanel 
namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// Info Panel that displays device sensors.
    /// </summary>
	public class SensorsPanel : AquamonixView
    {
		private const int LeftMargin = 16;
		private const int RightMargin = 14;
		private const int TopMargin = 11;
		private const int BottomMargin = 20;
		private const int LabelHeight = 20;

		private static readonly FontWithColor NormalFont = new FontWithColor(Fonts.RegularFontName, Sizes.FontSize6, Colors.StandardTextColor);
		private static readonly FontWithColor BoldFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize6, Colors.StandardTextColor);
		private static readonly FontWithColor ButtonFont = new FontWithColor(Fonts.BoldFontName, Sizes.FontSize6, Colors.BlueButtonText);

		private readonly UIButton _updateButton = new UIButton();
		private readonly UITableView _sensorsTable = new UITableView();
		private IEnumerable<SensorsTableRowViewModel> _rowValues;
		private Action _onUpdateClicked;

		public int ContentHeight
		{
			get
			{
				return (int)(this._sensorsTable.ContentSize.Height + TopMargin + BottomMargin);
			}
		}

		public Action OnUpdateClicked
		{
			get { return this._onUpdateClicked; }
			set { this._onUpdateClicked = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public SensorsPanel() : base()
		{
			ExceptionUtility.Try(() =>
			{
				this.BackgroundColor = Colors.LightGrayBackground;

				//update button
				this._updateButton.SetTitle(StringLiterals.Update, UIControlState.Normal);
				this._updateButton.SetTitle(StringLiterals.UpdatingText, UIControlState.Disabled);
				this._updateButton.SetFontAndColor(ButtonFont);
				this._updateButton.SetTitleColor(Colors.LightGrayTextColor, UIControlState.Disabled);
				this._updateButton.SizeToFit(); 
				this._updateButton.TouchUpInside += (o, e) =>
				{
					LogUtility.LogMessage("User clicked update sensors button.");

					if (this.OnUpdateClicked != null)
						this.OnUpdateClicked();
				};


				//sensors table
				this._sensorsTable.SeparatorStyle = UITableViewCellSeparatorStyle.None;
				this._sensorsTable.BackgroundColor = this.BackgroundColor;
				this._sensorsTable.ScrollEnabled = false;
				this._sensorsTable.AllowsSelection = false;

				_sensorsTable.RegisterClassForCellReuse(typeof(SensorsTableViewCell), SensorsTableViewCell.TableCellKey);

				this.AddSubviews(_updateButton, _sensorsTable);
			});
		}

		public void SetSensors(IEnumerable<SensorGroupViewModel> sensorGroups, DeviceFeatureViewModel sensorsFeature)
		{
			ExceptionUtility.Try(() =>
			{
				this._rowValues = FlattenGroups(sensorGroups);
				this._sensorsTable.Source = new SensorsTableViewSource(_rowValues.ToList());
				this._sensorsTable.ReloadData();
				this._updateButton.Hidden = !sensorsFeature.Updatable;

				if (this._rowValues.Count() == 0)
					this._updateButton.Hidden = true;
			});
		}

		public void SetUpdatingMode(bool updating)
		{
			ExceptionUtility.Try(() =>
			{
				this._updateButton.Enabled = !updating;
				this._updateButton.SizeToFit();
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				//update button
				this._updateButton.SizeToFit();
				this._updateButton.SetFrameHeight(LabelHeight);
				//TODO: 9 here is a 'magic number' (el numero magicko) 
				this._updateButton.SetFrameLocation(this.Frame.Width - _updateButton.Frame.Width - RightMargin, TopMargin +9);

				if (this.Frame.Height == 0)
					this._updateButton.Hidden = true;

				//sensors table 
				this._sensorsTable.SetFrameWidth(this.Frame.Width); // - (this.Frame.Width - this._updateButton.Frame.Left));
				this._sensorsTable.SetFrameHeight(this._sensorsTable.ContentSize.Height);
				this._sensorsTable.SetFrameLocation(0, TopMargin);

				if (!this._updateButton.Hidden)
				{
					this.BringSubviewToFront(this._updateButton); 
				}
			});
		}

		public int CalculateHeight()
		{
			return CalculateHeight(_rowValues);
		}

		public static int CalculateHeight(IEnumerable<SensorGroupViewModel> sensorGroups)
		{
			return CalculateHeight(FlattenGroups(sensorGroups));
		}

		private static int CalculateHeight(IEnumerable<SensorsTableRowViewModel> rowValues)
		{
			var rowHeight = (rowValues.Where((i) => !i.IsGroupName).Count() * SensorsTableViewSource.DefaultRowHeight);
			var headerRowHeight = (rowValues.Where((i) => i.IsGroupName).Count() * SensorsTableViewSource.RowHeaderHeight);
			var height = (rowHeight + headerRowHeight); 

			if (height > 0)
				height += (TopMargin + BottomMargin);

			return height;
		}

		private static IEnumerable<SensorsTableRowViewModel> FlattenGroups(IEnumerable<SensorGroupViewModel> groups)
		{
			List<SensorsTableRowViewModel> output = new List<SensorsTableRowViewModel>();

			ExceptionUtility.Try(() =>
			{
				foreach (var group in groups)
				{
					if (group.SensorValues != null && group.SensorValues.Count() > 0)
					{
						var groupHeader = (new SensorsTableRowViewModel()
						{
							IsGroupName = true,
							LeftValue = group.Name + (String.IsNullOrEmpty(group.Name) ? String.Empty : ":"),
							LeftColor = Colors.StandardTextColor,
							RightColor = Colors.StandardTextColor
						});

						if (!String.IsNullOrEmpty(groupHeader.LeftValue))
						{
							var childValues = new List<SensorsTableRowViewModel>();

							foreach (var value in group.SensorValues)
							{
								if (value.HasValue) // && !String.IsNullOrEmpty(value.Name))
								{
									childValues.Add(new SensorsTableRowViewModel()
									{
										IsGroupName = false,
										LeftValue = value.Name,
										RightValue = value.Value,
										RightColor = GraphicsUtility.TextColorForSeverity(value.Severity),
										LeftColor = Colors.StandardTextColor
									});
								}
							}

							if (childValues.Count > 0)
							{
								output.Add(groupHeader);
								foreach (var i in childValues)
									output.Add(i);
							}
						}
					}
				}
			});

			return output;
		}


		private class SensorsTableViewSource : UITableViewSource
		{
			public const int DefaultRowHeight = 20;
			public const int RowHeaderHeight = 30;

			private List<SensorsTableRowViewModel> _rowValues;

			public SensorsTableViewSource(List<SensorsTableRowViewModel> rowValues)
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
					var cell = (SensorsTableViewCell)tableView.DequeueReusableCell(SensorsTableViewCell.TableCellKey, indexPath);

					SensorsTableRowViewModel rowValue = null;
					if (indexPath.Row < _rowValues.Count)
						rowValue = _rowValues[indexPath.Row];
					cell.LoadCellValues(rowValue);
					return cell;
				});
			}
		}

		private class SensorsTableViewCell : UITableViewCell
		{
			public const string TableCellKey = "SensorsTableViewCell";

			private readonly AquamonixLabel _nameLabel = new AquamonixLabel();
			private readonly AquamonixLabel _valueLabel = new AquamonixLabel();

			public SensorsTableViewCell(IntPtr handle) : base(handle)
			{
				ExceptionUtility.Try(() =>
				{
					this.BackgroundColor = Colors.LightGrayBackground;

					_nameLabel.SetFontAndColor(NormalFont);

					_valueLabel.SetFontAndColor(BoldFont);

					this.ContentView.AddSubviews(_nameLabel, _valueLabel);
				});
			}

			public void LoadCellValues(SensorsTableRowViewModel rowValue)
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
						this._nameLabel.SetFrameSize(140, this.ContentView.Frame.Height);
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

		private class SensorsTableRowViewModel
		{
			public bool IsGroupName { get; set; }
			public string LeftValue { get; set; }
			public string RightValue { get; set; }

			public UIColor LeftColor { get; set;}
			public UIColor RightColor { get; set;}
		}
	}
}
