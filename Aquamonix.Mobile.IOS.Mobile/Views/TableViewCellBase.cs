using System;

using UIKit;

using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Views;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS
{
	public class TableViewCellBase : UITableViewCell
	{
		protected readonly DividerLine _divider = new DividerLine();

		public TableViewCellBase(IntPtr handle) : base(handle)
		{
			ExceptionUtility.Try(() =>
			{
				this.AddSubview(_divider);
			});
		}

		public sealed override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this._divider.Frame = new CoreGraphics.CGRect()
				{
					X = 0,
					Y = this.ContentView.Frame.Height - 1,
					Height = 1,
					Width = this.ContentView.Frame.Width
				};

				this.HandleLayoutSubviews();
			});
		}

		protected virtual void HandleLayoutSubviews() { }
	}
}
