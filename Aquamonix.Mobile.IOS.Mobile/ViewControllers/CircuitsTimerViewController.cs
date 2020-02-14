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
	public partial class CircuitsTimerViewController : TopLevelViewControllerBase
	{
		private static CircuitsTimerViewController _instance;

		//private DeviceDetailViewModel _device;
		private Action<int> _testSelectedCircuits;

        protected override nfloat ReconBarVerticalLocation
        {
            get
            {
                var output = (_headerTextView.IsDisposed) ? 0 : _headerTextView.Frame.Bottom;
                if (output == 0) output = base.ReconBarVerticalLocation + 6;
                return output;
            }

        }

        private CircuitsTimerViewController(DeviceDetailViewModel device, Action<int> testSelectedCircuits) : base()
		{
			ExceptionUtility.Try(() =>
			{
				//this._device = device;
				this.Initialize();

				if (testSelectedCircuits != null)
					this._testSelectedCircuits = WeakReferenceUtility.MakeWeakAction(testSelectedCircuits);
			});
		}
        public static CircuitsTimerViewController CreateInstance(DeviceDetailViewModel device, Action<int> testSelectedCircuits)
		{
			ExceptionUtility.Try(() =>
			{
				if (_instance != null)
				{
					_instance.Dispose();
					_instance = null;
				}

				_instance = new CircuitsTimerViewController(device, testSelectedCircuits);
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


				this._intervalPickerView.Value = TimeSpan.FromMinutes(120);

				this._startButton.TouchUpInside += (o, e) =>
				{
					this.SubmitChanges();
				};
			});
		}

		private void SubmitChanges()
		{
			ExceptionUtility.Try(() =>
			{
				this.NavigationController.PopViewController(true);

				if (this._testSelectedCircuits != null)
					this._testSelectedCircuits((int)this._intervalPickerView.Value.TotalMinutes);
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
					this._titleLabel.Text = StringLiterals.StartCircuits;

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
	}
}

