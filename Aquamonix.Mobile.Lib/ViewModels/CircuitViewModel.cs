using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class CircuitViewModel
	{
		private Circuit _circuit; 
		private Action _selectedChanged; 

		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; private set; }

		public bool Running { get; set; }

		public bool Selected
		{
			get { return this._circuit.Selected; }
			set { this._circuit.Selected = value; }
		}

		public bool Starting
		{
			get { return this._circuit.Starting; }
			set { this._circuit.Starting = value; }
		}

		public Action SelectedChanged 
		{
			get { return this._selectedChanged; }
			set { this._selectedChanged = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public bool HasFault { get; set; }

		public bool DisplayPowerIcon { get; private set; }

		public bool HighlightGreen { get; private set; }

		public CircuitViewModel(Circuit circuit)
		{
			this._circuit = circuit;

			this.Id = circuit.Id;
			this.Name = circuit.Name;
			this.Running = circuit.IsRunning;

			this.DisplayPowerIcon = circuit.ActiveState;
			this.HighlightGreen = (circuit.TestState || circuit.ScheduleState);
		}
	}
}
