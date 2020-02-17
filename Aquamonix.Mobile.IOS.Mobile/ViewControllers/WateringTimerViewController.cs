using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	public partial class WateringTimerViewController : TopLevelViewControllerBase
	{
		private static WateringTimerViewController _instance;

		private IEnumerable<PumpViewModel> _pumps;
		private DeviceDetailViewModel _device;
		private Action<IEnumerable<PumpViewModel>, int> _waterSelectedStations;

        protected override nfloat ReconBarVerticalLocation
        {
            get { 
                var output = (_headerTextView.IsDisposed) ? 0 : _headerTextView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            }
        }

        private WateringTimerViewController(DeviceDetailViewModel device, Action<IEnumerable<PumpViewModel>, int> waterSelectedStations) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this._device = device;
				this.Initialize();

				if (waterSelectedStations != null)
					this._waterSelectedStations = WeakReferenceUtility.MakeWeakAction(waterSelectedStations);
			});
		}

		public static WateringTimerViewController CreateInstance(DeviceDetailViewModel device, Action<IEnumerable<PumpViewModel>, int> waterSelectedStations)
		{
			ExceptionUtility.Try(() =>
			{
				if (_instance != null)
				{
					_instance.Dispose();
					_instance = null;
				}

				_instance = new WateringTimerViewController(device, waterSelectedStations);
			});

			return _instance;
		}


		protected override void InitializeViewController()
		{
			ExceptionUtility.Try(() =>
			{
				base.InitializeViewController();

				this._navBarView.OnCancel = () =>
				{
					this.NavigationController.PopViewController(true);
				};

				this.NavigationBarView = this._navBarView;
				this.NavigationItem.HidesBackButton = true;


				//pumps table 
				this._pumpsTable.RegisterClassForCellReuse(typeof(PumpsTableViewCell), PumpsTableViewCell.TableCellKey);

				var pumps = this._device.Pumps?.Where((p) => p.Manual)?.ToList();
				this._pumps = pumps ?? new List<PumpViewModel>();

				this._pumpsTable.Source = new PumpsTableViewSource(this._pumps.ToList());
				//this._startButton.Enabled = (!this._pumps.Any());
				this._pumpsTable.Hidden = (!this._pumps.Any());
				this._tableHeaderTextView.Hidden = this._pumpsTable.Hidden;

				this._intervalPickerView.Value = TimeSpan.FromMinutes(10); 

				this._startButton.TouchUpInside += (o, e) =>
				{
					this.SubmitChanges();
				}; 

				/*
				if (pumps != null)
				{
					foreach (var pump in this._pumps)
					{
						pump.SelectedChanged = this.PumpSelectedChanged;
					}
				}
				*/
			});
		}

		/*
		private void PumpSelectedChanged()
		{
			ExceptionUtility.Try(() =>
			{
				var count = this._pumps.Where((p) => p.Selected)?.Count();
				//this._startButton.Enabled = (count > 0);
			});
		}*/

		private void SubmitChanges()
		{
			ExceptionUtility.Try(() =>
			{
				this.NavigationController.PopViewController(true);

				if (this._waterSelectedStations != null)
				{
					var selectedPumps = this._pumps.Where(p => p.Selected).ToList();
					this._waterSelectedStations(selectedPumps, (int)this._intervalPickerView.Value.TotalMinutes);
				}
			});
   		}


		private class NavBarView : NavigationBarView
		{
			public const int Margin = 16;

			private readonly UIButton _cancelButton = new UIButton();
			private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
			private Action _onCancel;

			public Action OnCancel
			{
				get { return this._onCancel; }
				set { this._onCancel = WeakReferenceUtility.MakeWeakAction(value); }
			}

			public NavBarView() : base(fullWidth: true)
			{
				ExceptionUtility.Try(() =>
				{
					//cancel button 
					this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Normal);
					this._cancelButton.SetFontAndColor(new FontWithColor(Fonts.RegularFontName, Sizes.FontSize8, UIColor.White));
					this._cancelButton.TouchUpInside += (o, e) =>
					{
						if (this.OnCancel != null)
							this.OnCancel();
					};

					//selected label 
					this._titleLabel.SetFontAndColor(new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, UIColor.White));
					this._titleLabel.Text = StringLiterals.StartWatering;

					this.BackgroundColor = Colors.AquamonixGreen;

					this.AddSubviews(_titleLabel, _cancelButton);
				});
			}

			public override void LayoutSubviews()
			{
				ExceptionUtility.Try(() =>
				{
					base.LayoutSubviews();

					//cancel button 
					this._cancelButton.SizeToFit();
					this._cancelButton.SetFrameHeight(Height);
					this._cancelButton.SetFrameLocation(Margin, 0);

					//selection label 
					this._titleLabel.SizeToFit();
					this._titleLabel.SetFrameHeight(Height);
					this._titleLabel.SetFrameLocation(this.Frame.Width / 2 - this._titleLabel.Frame.Width / 2, 0);
					//this._titleLabel.EnforceMaxXCoordinate(this.Frame.Width - this._setButton.Frame.Left);
				});
			}
   		}

		private class PumpsTableViewSource : TableViewSourceBase<PumpViewModel>
		{
			public PumpsTableViewSource(IList<PumpViewModel> values) : base(values)
			{
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				ExceptionUtility.Try(() =>
				{
					var cell = tableView.CellAt(indexPath) as PumpsTableViewCell;
					if (cell != null)
					{
						cell.ToggleCheckbox();
					}
				});
			}

			protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (PumpsTableViewCell)tableView.DequeueReusableCell(PumpsTableViewCell.TableCellKey, indexPath);

				PumpViewModel pump = null;
				if (indexPath.Row < Values.Count)
					pump = Values[indexPath.Row];

				//create cell style
				if (pump != null)
				{
					cell.LoadCellValues(pump);
				}

				return cell;
			}
		}

		private class PumpsTableViewCell : TableViewCellBase
		{
			public const string TableCellKey = "PumpsTableViewCell";

			private readonly AquamonixLabel _textLabel = new AquamonixLabel(); 
			private readonly RoundCheckBox _checkbox = new RoundCheckBox(Images.GreenCheckboxChecked, Images.GreenCheckboxUnchecked);
			private PumpViewModel _pump;

			public PumpsTableViewCell(IntPtr handle) : base(handle)
			{
				ExceptionUtility.Try(() =>
				{
					this._textLabel.SetFontAndColor(TextFont);

					this._checkbox.OnCheckedChanged = () =>
					{
						if (_pump != null)
						{
							_pump.Selected = _checkbox.Checked;

							if (_pump.SelectedChanged != null)
								_pump.SelectedChanged();
						}
					};

					this.ContentView.AddSubviews(_checkbox, _textLabel);
				});
			}

			public void LoadCellValues(PumpViewModel pump)
			{
				ExceptionUtility.Try(() =>
				{
					this._pump = pump;
					this._textLabel.Text = pump.Text;
					this._textLabel.SizeToFit();
					this._checkbox.SetChecked(this._pump.Selected);
				});
			}

			public void ToggleCheckbox()
			{
				this._checkbox.SetChecked(!_checkbox.Checked);
			}

			protected override void HandleLayoutSubviews()
			{
				base.HandleLayoutSubviews();

				this._checkbox.SizeToFit();
				this._checkbox.AlignToRightOfParent(LeftTextMargin);
				this._checkbox.CenterVerticallyInParent(); 

				this._textLabel.SetFrameLocation(LeftTextMargin, 0);
				this._textLabel.SetFrameSize(this.ContentView.Frame.Width - LeftTextMargin - _checkbox.Frame.Width, this.ContentView.Frame.Height);
			}
		}
	}
}

