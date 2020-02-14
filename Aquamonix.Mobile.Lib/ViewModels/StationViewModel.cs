using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class StationViewModel
	{
		private Station _station;
		private string _description = null;
		private Action _selectedChanged;

		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { 
			get { return (AppSettings.AdvancedView ? _description : null);}
			set { _description = value;}
		}

		public bool Running { get; set; }

		public int? ZoneColor { get; set; }

		public Action SelectedChanged
		{
			get { return this._selectedChanged; }
			set { this._selectedChanged = WeakReferenceUtility.MakeWeakAction(value); }
		}

		public bool Selected
		{
			get { return this._station.Selected; }
			set { this._station.Selected = value; }
		}

		//TODO: replace this with IsUpdatingStatus
		public bool Starting
		{
			get { return this._station.Starting;}
			set { this._station.Starting = value;}
		}

		public bool IsUpdatingStatus { get; set; }

		public StationViewModel(Station station)
		{
			this._station = station; 

			this.Id = station.Id;
			this.Name = station.Name;
			//this.Description = String.Format("{0}", station.Name); 
			this.Running = station.IsRunning;
			this.IsUpdatingStatus = station.IsUpdatingStatus;

			this.ZoneColor = DataCache.ApplicationMetadata?.GetColorForZone(station.Zone);
		}
	}
}
