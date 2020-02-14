using System;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class MinMax : ICloneable<MinMax>
	{
		[DataMember(Name = PropertyNames.Min)]
		public double Min { get; set; }

		[DataMember(Name = PropertyNames.Max)]
		public double Max { get; set; }

		public MinMax Clone()
		{
			return new MinMax() { Max = this.Max, Min = this.Min };
		}
	}
}
