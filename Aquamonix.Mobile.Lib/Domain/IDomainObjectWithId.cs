using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Aquamonix.Mobile.Lib
{
    /// <summary>
    /// Base interface for all domain objects that have the unique identifier "Id" property.
    /// </summary>
	public interface IDomainObjectWithId
	{
        /// <summary>
        /// Gets/sets the object's unique id.
        /// </summary>
		[DataMember]
		string Id { get; set;}

        /// <summary>
        /// Runs through child dictionaries, setting the ids of child domain objects 
        /// which have ids as well.
        /// </summary>
		void ReadyChildIds();
	}
}
