using System;

namespace Aquamonix.Mobile.Lib.Domain
{
	//HARDCODED
	//TODO:* add a lookup function for these; instead of parsing the numeric value, do a lookup in app metadata 
	public enum SeverityLevel
	{
		Missing = -1,
		None = 0,
		Normal, 
		Low, 
		Medium, 
		High, 
		Extreme
	}
}
