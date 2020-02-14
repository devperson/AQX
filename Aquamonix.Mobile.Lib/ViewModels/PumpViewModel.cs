using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class PumpViewModel
	{
		private Action _selectedChanged; 

		public string Id { get; set; }
		public string Text { get; set; }
		public bool Selected { get; set;}
		public bool Manual { get; set;}
		public bool IsUpdatingStatus { get; set; }

		public Action SelectedChanged
		{
			get { return this._selectedChanged;}
			set { this._selectedChanged = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public PumpViewModel()
		{
		}

		public PumpViewModel(Pump pump)
		{
			this.Id = pump.Id;
			this.Text = pump.Name;
			this.IsUpdatingStatus = pump.IsUpdatingStatus;

			//HARDCODED
			this.Manual = (pump.Mode == "Manual");
		}
	}
}
