using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain.Requests;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class ItemsDictionary<T> :
        //IDictionary<string, T>,
        //ICollection<KeyValuePair<string, T>>,
        //IEnumerable<KeyValuePair<string, T>>,
        //IEnumerable,
        //IDictionary,
        //ICollection,
        //IReadOnlyDictionary<string, T>,
        //IReadOnlyCollection<KeyValuePair<string, T>>,
        //ISerializable,
        //IDeserializationCallback, 
        ICloneable<ItemsDictionary<T>>
    {
        private IDictionary<string, T> _innerDict;
        //private Requests.Program program;

        [DataMember(Name = PropertyNames.Summary)]
        public Summary Summary { get; set; }

        #region For Pivot

        [DataMember(Name = PropertyNames.Status)]
        public ValueVisible Status { get; set; }

        [DataMember(Name = PropertyNames.CurrentProgramId)]
        public string CurrentProgramId { get; set; }

        [DataMember(Name = PropertyNames.StatusDescription)]
        public IEnumerable<string> StatusDescription { get; set; }

        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }



        #endregion

        [DataMember(Name = PropertyNames.RemoveList)]
        public IEnumerable<string> RemoveList { get; set; }

        internal void Add(string alertid, Alerts alerts)
        {
            throw new NotImplementedException();
        }

        [DataMember(Name = PropertyNames.Items)]
        public IDictionary<string, T> Items
        {
            get { return this._innerDict; }
            set { this._innerDict = value; }
        }

        public T this[string key]
        {
            get { return this._innerDict[key]; }
            set { this._innerDict[key] = value; }
        }

        public int Count
        {
            get { return this._innerDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)this._innerDict).IsReadOnly; }
        }

        public bool IsFixedSize
        {
            get { return ((IDictionary)this._innerDict).IsFixedSize; }
        }

        public bool IsSynchronized
        {
            get { return ((IDictionary)this._innerDict).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IDictionary)this._innerDict).SyncRoot; }
        }

        public ICollection<string> Keys
        {
            get { return this._innerDict?.Keys; }
        }

        public ICollection<T> Values
        {
            get { return this._innerDict?.Values; }
        }

        public ItemsDictionary()
        {
            this._innerDict = new Dictionary<string, T>();
        }

        public ItemsDictionary(IDictionary<string, T> dictionary)
        {
            this._innerDict = new Dictionary<string, T>(dictionary);
        }

        public T GetIfExists(string key)
        {
            if (this.ContainsKey(key))
                return this[key];
            else
                return default(T);
        }

        public void Clear()
        {
            this._innerDict?.Clear();
        }

        public void Add(string key, T value)
        {
            this._innerDict?.Add(key, value);
        }

        public bool TryGetValue(string key, out T value)
        {
            return this._innerDict.TryGetValue(key, out value);
        }

        public bool ContainsKey(string key)
        {
            return this._innerDict.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return this._innerDict.Remove(key);
        }

        public bool Contains(object key)
        {
            return ((IDictionary)this._innerDict).Contains(key);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)this._innerDict).CopyTo(array, index);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return this._innerDict.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return ((ICollection<KeyValuePair<string, T>>)this._innerDict).Remove(item);
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return ((ICollection<KeyValuePair<string, T>>)this._innerDict).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int index)
        {
            ((ICollection<KeyValuePair<string, T>>)this._innerDict).CopyTo(array, index);
        }

        public void Add(KeyValuePair<string, T> item)
        {
            ((ICollection<KeyValuePair<string, T>>)this._innerDict).Add(item);
        }
        /*
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this._innerDict.GetObjectData(info, context);
		}

		public void OnDeserialization(object sender)
		{
			this._innerDict.OnDeserialization(sender);
   		}

		*/

        public virtual ItemsDictionary<T> Clone()
        {
            return new ItemsDictionary<T>()
            {
                Items = _innerDict.Clone(),
                Summary = this.Summary?.Clone(),
                RemoveList = this.RemoveList
            };
        }

        public virtual void ReadyDictionary()
        {
            foreach (var item in this._innerDict)
            {
                if (item.Value is IDomainObjectWithId)
                {
                    var obj = (item.Value as IDomainObjectWithId);
                    if (obj != null)
                    {
                        (obj as IDomainObjectWithId).Id = item.Key;
                        (obj as IDomainObjectWithId).ReadyChildIds();
                    }
                }
                else
                    break;
            }
        }

        public virtual void ProcessRemoveList()
        {
            if (this.RemoveList != null && this.RemoveList.Any())
            {
                foreach (var item in this.RemoveList)
                {
                    if (this.ContainsKey(item))
                        this.Remove(item);
                }
            }
        }

        public static ItemsDictionary<T> MergePropertyLists(ItemsDictionary<T> child, ItemsDictionary<T> parent, bool removeIfMissingFromParent, bool parentIsMetadata)
        {
            return ExceptionUtility.Try<ItemsDictionary<T>>(() =>
            {
                if (child == null)
                {
                    if (parent == null)
                        return null;
                    else
                    {
                        if (parent._innerDict != null)
                            child = new ItemsDictionary<T>(parent._innerDict.Clone());
                        else
                            child = new ItemsDictionary<T>();
                    }
                }
                else
                {
                    //merge collections 
                    if (child?.Items != null)
                        child.Items = MergeExtensions.MergePropertyLists(child?.Items, parent?.Items, removeIfMissingFromParent, parentIsMetadata);

                    //merge summaries 
                    if (child != null)
                    {
                        if (child.Summary == null)
                            child.Summary = parent?.Summary;

                        if (child.Summary != null)
                            child.Summary.MergeFromParent(parent?.Summary, removeIfMissingFromParent, parentIsMetadata);

                        //merge removers 
                        if (child.RemoveList == null)
                            child.RemoveList = parent?.RemoveList;
                        else
                        {
                            if (parent != null && child.RemoveList != null && parent.RemoveList != null && parent.RemoveList.Any())
                            {
                                var childList = child.RemoveList.ToList();
                                foreach (var item in parent.RemoveList)
                                {
                                    if (!childList.Contains(item))
                                        childList.Add(item);
                                }
                                child.RemoveList = childList;
                            }
                        }


                        if (child.RemoveList != null)
                        {
                            foreach (string key in child.RemoveList)
                            {
                                if (child.ContainsKey(key))
                                    child.Remove(key);
                            }
                        }
                    }
                }

                return child;
            });
        }

        #region ExplicitInterface

        /*

		object IDictionary.this[object key]
		{
			get { return this[(string)key]; }
			set { this[(string)key] = (T)value; }
		}
		
		ICollection IDictionary.Keys
		{
			get { return ((IDictionary)this._innerDict).Keys; }
		}

		ICollection IDictionary.Values
		{
			get { return ((IDictionary)this._innerDict).Values; }
		}

		IEnumerable<string> IReadOnlyDictionary<string, T>.Keys
		{
			get { return ((IReadOnlyDictionary<string, T>)this._innerDict).Keys; }
		}

		IEnumerable<T> IReadOnlyDictionary<string, T>.Values
		{
			get { return ((IReadOnlyDictionary<string, T>)this._innerDict).Values; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this._innerDict).GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)this._innerDict).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			((IDictionary)this._innerDict).Remove(key);
		}

		void IDictionary.Add(object key, object value)
		{
			((IDictionary)this._innerDict).Add(key, value);
		}
		*/
        #endregion
    }
}
