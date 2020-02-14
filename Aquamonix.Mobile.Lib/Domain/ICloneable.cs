using System;

namespace Aquamonix.Mobile.Lib.Domain
{
    /// <summary>
    /// Base interface for domain objects which can be cloned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public interface ICloneable<T> 
	{
		T Clone();
	}
}
