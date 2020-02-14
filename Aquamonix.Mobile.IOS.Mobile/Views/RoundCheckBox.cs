using System;

using UIKit;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Views
{
	public class RoundCheckBox : AquamonixView
    {
		private readonly UIButton _checkButton = new UIButton(UIButtonType.Custom);
		private UIImage _checkedImage;
		private UIImage _uncheckedImage;
		private Action _onCheckedChanged;

		public bool Checked { get; private set;}

		public bool Enabled
		{
			get { return this._checkButton.Enabled;}
			set { this._checkButton.Enabled = value; }
		}

		public Action OnCheckedChanged 
		{
			get { return this._onCheckedChanged; }
			set { this._onCheckedChanged = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public RoundCheckBox(UIImage checkedImage, UIImage uncheckedImage) : base()
		{
			ExceptionUtility.Try(() =>
			{
				this._checkedImage = checkedImage;
				this._uncheckedImage = uncheckedImage;

				this.SetChecked(false);

				this._checkButton.TouchUpInside += (o, e) =>
				{
					this.SetChecked(!this.Checked);
					if (this.OnCheckedChanged != null)
						this.OnCheckedChanged();
				};

				this.AddSubview(_checkButton);
			});
		}

		public void SetChecked(bool value, bool fireEvent = true)
		{
			ExceptionUtility.Try(() =>
			{
				var image = value ? _checkedImage : _uncheckedImage;

				_checkButton.SetBackgroundImage(image, UIControlState.Normal);

				this.Checked = value;

				if (fireEvent)
				{
					if (this.OnCheckedChanged != null)
						this.OnCheckedChanged();
				}
			});
		}

		public override void SizeToFit()
		{
			ExceptionUtility.Try(() =>
			{
				this.SetButtonSize();
				this.SetFrameSize(this._checkButton.Frame.Size);
			});
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this.SetButtonSize();
				this._checkButton.SetFrameLocation(0, 0);
			});
		}

		private void SetButtonSize()
		{
			ExceptionUtility.Try(() =>
			{
				this._checkButton.SetFrameSize(_checkedImage.Size);
			});
		}
	}
}
