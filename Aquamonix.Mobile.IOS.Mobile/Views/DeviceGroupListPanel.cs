using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.IOS.UI;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.Views
{
    /// <summary>
    /// InfoPanel that displays devices in a group.
    /// </summary>
	public class DeviceGroupListPanel : InfoPanel
	{
		public DeviceGroupListPanel() : base()
		{
		}

		public void SetDevices(IEnumerable<SensorGroupViewModel> devices)
		{
			ExceptionUtility.Try(() =>
			{
				this._rowValues = FlattenGroups(devices);
				this._dataTable.Source = new DataTableViewSource(_rowValues.ToList());
				this._dataTable.ReloadData();
			});
   		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();
			});
   		}

		public static int CalculateHeight(IEnumerable<SensorGroupViewModel> devices)
		{
            return CalculateHeight(FlattenGroups(devices));
   		}

		private static IEnumerable<DataTableRowViewModel> FlattenGroups(IEnumerable<SensorGroupViewModel> groups)
		{
			var output = new List<DataTableRowViewModel>();

			ExceptionUtility.Try(() =>
			{
				foreach (var group in groups)
				{
					if (group.SensorValues != null && group.SensorValues.Count() > 0)
					{
						var groupHeader = (new DataTableRowViewModel()
						{
							IsGroupName = true,
							LeftValue = group.Name + (String.IsNullOrEmpty(group.Name) ? String.Empty : ":"),
							LeftColor = Colors.StandardTextColor
						});

						if (!String.IsNullOrEmpty(groupHeader.LeftValue))
						{
							var childValues = new List<DataTableRowViewModel>();

							foreach (var value in group.SensorValues)
							{
								if (!String.IsNullOrEmpty(value.Name))
								{
									childValues.Add(new DataTableRowViewModel()
									{
										IsGroupName = false,
										LeftValue = value.Name,
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
	}
}
