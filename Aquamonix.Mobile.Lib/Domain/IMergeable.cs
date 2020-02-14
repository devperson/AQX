using System;

namespace Aquamonix.Mobile.Lib.Domain
{
    /// <summary>
    /// Base interface for domain objects which can be merged with a parent version.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public interface IMergeable<T>
	{
		void MergeFromParent(T parent, bool removeIfMissingFromParent, bool parentIsMetadata);
	}
}
