using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.Extensions
{
    /// <summary>
    /// Encapsulates all logic for merging properties and lists of properties, as well as lists of objects.
    /// </summary>
	public static class MergeExtensions
	{
		private static readonly bool PreserveDictionaryOrder = true; 

        /// <summary>
        /// Merges a parent dictionary into a child dictionary.
        /// </summary>
        /// <param name="dict">The child dictionary to merge into</param>
        /// <param name="parent">The parent dictionary to merge</param>
        /// <param name="removeIfMissingFromParent">If missing from parent, remove from child as well.</param>
        /// <param name="parentIsMetadata">True if the parent is application metadata.</param>
		public static void MergeFromParent<Tk, Tv>(this IDictionary<Tk, Tv> dict, IDictionary<Tk, Tv> parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			//preserve the order from the longer list 
			//var orderList = new List<Tk>(); 

			if (parent != null)
			{
                //merge each item 
				foreach (var item in parent)
				{
					if (dict.ContainsKey(item.Key) && dict[item.Key] is IMergeable<Tv>)
					{
						((IMergeable<Tv>)dict[item.Key]).MergeFromParent(item.Value, removeIfMissingFromParent, parentIsMetadata);
					}
					else {
						if (item.Value is ICloneable<Tv>)
							dict.Add(item.Key, ((ICloneable <Tv>)item.Value).Clone());
					}
				}

                //remove items if missing from parent 
				if (removeIfMissingFromParent)
				{
					List<Tk> toRemove = new List<Tk>(); 

					foreach (var key in dict.Keys)
					{
						if (!parent.ContainsKey(key))
							toRemove.Add(key);
					}

					foreach (var key in toRemove)
						dict.Remove(key);
				}
			}
		}

        /// <summary>
        /// Copies the given dictionary; if the dictionary items are ICloneable, they are cloned; otherwise, they are simply copied.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>A clone of the given dictionary</returns>
		public static IDictionary<Tk, Tv> Clone<Tk, Tv>(this IDictionary<Tk, Tv> dict) 
		{
			var output = new Dictionary<Tk, Tv>();

			foreach (var item in dict)
			{
				if (item.Value is ICloneable<Tv>)
					output.Add(item.Key, ((ICloneable<Tv>)item.Value).Clone()); 
				else
					output.Add(item.Key, item.Value);
			}

			return output;
		}

        /// <summary>
        /// Makes a shallow copy of a dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to shallow-copy</param>
        /// <returns>Shallow copy of given dictionary</returns>
		public static IDictionary<Tk, Tv> ShallowCopy<Tk, Tv>(this IDictionary<Tk, Tv> dict) 
		{
			var output = new Dictionary<Tk, Tv>();

			foreach (var item in dict)
			{
				output.Add(item.Key, item.Value);
			}

			return output;
		}

        /// <summary>
        /// Clones the given string/string dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to clone.</param>
        /// <returns>Clone of the given dictionary.</returns>
		public static IDictionary<string, string> Clone(this IDictionary<string, string> dict) 
		{
			var output = new Dictionary<string, string>();

			foreach (var item in dict)
			{
				output.Add(item.Key, item.Value);
			}

			return output;
		}

        /// <summary>
        /// Clones a dictionary of ICloneable&lt;T&gt;s
        /// </summary>
        /// <param name="dict">Dictionary to clone</param>
        /// <returns>A clone of the given dictionary.</returns>
		public static IDictionary<Tk, Tv> CloneGeneric<Tk, Tv>(this IDictionary<Tk, Tv> dict) where Tv : ICloneable
		{
			var output = new Dictionary<Tk, Tv>();

			foreach (var item in dict)
			{
				output.Add(item.Key, (Tv)item.Value.Clone());
			}

			return output;
		}

        /// <summary>
        /// Merges two dictionaries in a parent/child object merge, where the dictionaries are properties of the child/parent 
        /// objects in the merge.
        /// </summary>
        /// <param name="child">Dictionary which is a property of the child object in the merge.</param>
        /// <param name="parent">Dictionary which is a property of the parent object in the merge.</param>
        /// <param name="removeIfMissingFromParent">If missing from parent, remove from child as well.</param>
        /// <param name="parentIsMetadata">True if the parent is application metadata.</param>
        /// <returns>Merged dictionary that can be assigned to the child object.</returns>
		public static IDictionary<string, T> MergePropertyLists<T>(IDictionary<string, T> child, IDictionary<string, T> parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (child == null && parent != null)
				child = new Dictionary<string, T>();

            //determine the order 
			List<string> orderIds = null;
			if (PreserveDictionaryOrder && parentIsMetadata && parent != null)
				orderIds = parent.Keys.ToList();

            //remove if missing from parent
			if (removeIfMissingFromParent)
			{
				if (parent == null)
					child = null;
			}

            //merge 
			if (child != null)
				child.MergeFromParent(parent, removeIfMissingFromParent, parentIsMetadata);

			//preserve the order 
			if (orderIds != null)
			{
				var childCopy = child.ShallowCopy();
				var orderedDict = new Dictionary<string, T>(); 
				foreach(string key in orderIds)
				{
					if (childCopy.ContainsKey(key))
					{
						var item = childCopy[key];
						childCopy.Remove(key);
						orderedDict.Add(key, item); 
					}
				}

				while (childCopy.Count > 0)
				{
					var item = childCopy.First();
					orderedDict.Add(item.Key, item.Value);
					childCopy.Remove(item.Key); 
				}

				child = orderedDict;
			}

			return child;
		}

        /// <summary>
        /// Merges a single value property from parent to child.
        /// </summary>
        /// <param name="childProperty">Current pre-merge value of child property.</param>
        /// <param name="parentProperty">Current value of parent property.</param>
        /// <param name="removeIfMissingFromParent">If missing from parent, remove from child as well.</param>
        /// <param name="parentIsMetadata">True if the parent is application metadata.</param>
        /// <returns>A value that can be assigned to the child object to replace previous value.</returns>
		public static T MergeProperty<T>(T childProperty, T parentProperty, bool removeIfMissingFromParent, bool parentIsMetadata) 
		{
            //remove if missing from parent
			if (removeIfMissingFromParent)
			{
				if (Object.ReferenceEquals(null, parentProperty)) //parentProperty == null)
					childProperty = default(T);
			}

            //if parent is metadata, prefer parent value
			if (parentIsMetadata)
			{
				if (Object.ReferenceEquals(null, childProperty))
					return parentProperty;
			}
			else {
				if (!Object.ReferenceEquals(null, parentProperty))
					return parentProperty;
			}

			return childProperty;
		}
	}
}
