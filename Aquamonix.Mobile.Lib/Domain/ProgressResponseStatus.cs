using System;
using System.Collections.Generic;

namespace Aquamonix.Mobile.Lib.Domain
{
	public struct ProgressResponseStatus
	{
		//HARDCODED
		private static readonly List<string> _finalStatuses = new List<string>(new string[] {
			"CompletedSuccessfully",
			"StoppedSuccess",
			"StoppedFailed"
		});

		private string _value;

		public bool IsFinal
		{
			get {
				return (this._value != null && _finalStatuses.Contains(this._value)); 
			}
		}

		public string Value
		{
			get { return _value; }
		}

		public ProgressResponseStatus(string value)
		{
			this._value = value;
		}
	}
}
