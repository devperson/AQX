using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class ProgramViewModel
	{
		public Program _program;
		private string _description;
		private Action<ProgramViewModel, bool> _stopStartProgram;

		public string Id { get; set; }

		public string Name { get; set; }

		public string Description
		{
			get { return AppSettings.AdvancedView ? _description : null; }
			set { this._description = value;}
		}

		public bool Running
		{
			get { return this._program.IsRunning;}
		}

		//TODO: replace this with IsUpdatingStatus
		public bool Starting
		{
			get { return this._program.Starting;}
			set { this._program.Starting = value; }
		}

		public bool IsUpdatingStatus { get; set; }

		public bool Scheduled { get; set; }

		public Action<ProgramViewModel, bool> StopStartProgram 
		{
			get { return this._stopStartProgram; }
			set { this._stopStartProgram = WeakReferenceUtility.MakeWeakAction(value); }
		} 

		public ProgramViewModel(Program program)
		{
			this._program = program;

			this.Id = program.Id;
			this.Name = program.Name;
			this.Scheduled = program.SetToRun.GetValueOrDefault();
			this.IsUpdatingStatus = program.IsUpdatingStatus;
		}
	}

	public enum ProgramStatus
	{
		Running,
		Starting,
		Stopping,
		Stopped
	}
}
