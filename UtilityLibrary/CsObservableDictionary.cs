#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Serialization;

#endregion

namespace UtilityLibrary
{
	// [DataContract(Namespace = "")]
	// public struct Kvp<TKey, TValue>
	// {
	// 	[DataMember]
	// 	public TKey Key { get; set; }
	// 	[DataMember]
	// 	public TValue Value { get; set; }
	//
	// 	public Kvp(TKey key, TValue value)
	// 	{
	// 		Key = key;
	// 		Value = value;
	// 	}
	// }

	[Serializable]
	[CollectionDataContract(Namespace = "", ItemName = "KeyValuePair")]
	public class ObservableDictionary<TKey, TValue> : INotifyCollectionChanged,  ISerializable,
		IDictionary<TKey, TValue>

	{
		private ObservableCollection<KeyValuePair<TKey, TValue>> list;
		private ICollection<TKey> keys;
		private ICollection<TValue> values;

		private KeyValuePair<TKey, TValue> found;
		private int foundIdx;
		private bool isReadOnly;

		public ObservableDictionary()
		{
			list = new ObservableCollection<KeyValuePair<TKey, TValue>>();
		}

		public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			Dictionary<string, string> d = new Dictionary<string, string>();

			list = new ObservableCollection<KeyValuePair<TKey, TValue>>(collection);
		}

	#region public properties

		public int Count => list.Count;

		public ICollection<TValue> Values
		{
			get
			{
				values = new List<TValue>(list.Count);

				foreach (KeyValuePair<TKey, TValue> KeyValuePair in list)
				{
					values.Add(KeyValuePair.Value);
				}

				return values;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				keys = new List<TKey>(list.Count);

				foreach (KeyValuePair<TKey, TValue> KeyValuePair in list)
				{
					keys.Add(KeyValuePair.Key);
				}

				return keys;
			}
		}

		public KeyValuePair<TKey, TValue> Found
		{
			get => found;
			set => found = value;
		}

		public int FoundIdx
		{
			get => foundIdx;
			set => foundIdx = value;
		}


	#endregion

	#region indexer

		/// <summary>
		/// Get a Value using the Key as an index<br/>
		/// throw exception of key is null
		/// </summary>
		public TValue this[TKey key]
		{
			get
			{
				nullKeyTest(key);

				if (ContainsKey(key))
				{
					return found.Value;
				}

				throw new KeyNotFoundException();
			}
			set
			{
				if (ContainsKey(key))
				{
					found = new KeyValuePair<TKey, TValue>(key, value);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		/// <summary>
		/// Get a Key-Value pair using the Key as an index<br/>
		/// throw exception of key is null
		/// </summary>
		public KeyValuePair<TKey, TValue> this[int idx]
		{
			get => list[idx];
		
			set => list[idx] = value;
		}

	#endregion

		/// <summary>
		/// Add a Key and Value<br/>
		/// throw exception of key is null
		/// </summary>
		public void Add(TKey key, TValue value)
		{
			existKeyTest(key);

			list.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool TryAdd(TKey key, TValue value)
		{
			if (ContainsKey(key)) return false;
			list.Add(new KeyValuePair<TKey, TValue>(key, value));
			return true;
		}

		/// <summary>
		/// Clear the Dictionary
		/// </summary>
		public void Clear()
		{
			list.Clear();
		}

		/// <summary>
		/// Determine of Key exists<br/>
		/// throw exception of key is null
		/// </summary>
		public bool ContainsKey(TKey key)
		{
			nullKeyTest(key);

			for (var i = 0; i < list.Count; i++)
			{
				if (list[i].Key.Equals(key))
				{
					found = list[i];
					foundIdx = i;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determine of Value exists
		/// </summary>
		public bool ContainsValue(TValue value)
		{
			for (var i = 0; i < list.Count; i++)
			{
				if (list[i].Value.Equals(value))
				{
					found = list[i];
					foundIdx = i;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Copy the Dictionary to a KeyValue array
		/// </summary>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove the Value based on the Key<br/>
		/// throw exception of key is null
		/// </summary>
		public bool Remove(TKey key)
		{
			nullKeyTest(key);

			if (ContainsKey(key))
			{
				list.RemoveAt(foundIdx);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove the Value based on the Key<br/>
		/// Return the removed Value,<br/>
		/// throw exception of key is null
		/// </summary>
		public bool Remove(TKey key, out TValue value)
		{
			nullKeyTest(key);

			value = default;

			if (ContainsKey(key))
			{
				value = list[foundIdx].Value;
				list.RemoveAt(foundIdx);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Remove the Value based on an index<br/>
		/// Return the removed Value<br/>
		/// throw exception of index is out of range
		/// </summary>
		public bool Remove(int idx, out TValue value)
		{
			value = default;

			if (idx >= 0 && idx < list.Count)
			{
				value = list[idx].Value;
				list.RemoveAt(idx);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Try to get a Value based on the Key<br/>
		/// return false if Key is null or not found.
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value)
		{
			bool result;
			value = default(TValue);

			try
			{
				if (ContainsKey(key))
				{
					result = true;
					value = found.Value;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception e)
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Get the index based on the Key<br/>
		/// throw exception of key is null
		/// </summary>
		public int IndexOf(TKey key)
		{
			if (ContainsKey(key))
			{
				return foundIdx;
			}

			return -1;
		}

		/// <summary>
		///Remove the Value based on an index<br/>
		/// throw exception of index is out of range
		/// </summary>
		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		/// <summary>
		/// Replace <c>oldKey</c> with <c>newKey</c> if possible<br/>
		/// throw exception of key is null
		/// </summary>
		public bool ReplaceKey(TKey oldKey, TKey newKey)
		{
			if (oldKey == null || newKey == null || ContainsKey(newKey)) { return false; }

			TValue value;

			if (!Remove(oldKey, out value)) return false;

			Add(newKey, value);

			return true;

		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => isReadOnly;

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return list.Remove(new KeyValuePair<TKey, TValue>( item.Key, item.Value));
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> KeyValuePair)
		{
			this.Add(KeyValuePair.Key, KeyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return list.Contains(new KeyValuePair<TKey, TValue>( item.Key, item.Value));
		}

	#region private methods

		private void nullKeyTest(TKey key)
		{
			if (ReferenceEquals(key, null))
			{
				throw new ArgumentNullException();
			}
		}

		private void existKeyTest(TKey key)
		{
			if (ContainsKey(key)) throw new ArgumentException();
		}

	#endregion

	#region event publish

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			remove { list.CollectionChanged -= value; }
			add { list.CollectionChanged += value; }
		}

	#endregion

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) throw new ArgumentNullException("Info");

			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[list.Count];

			CopyTo(array, 0);

			info.AddValue("KeyValuePairs", (object) array, typeof(KeyValuePair<TKey, TValue>));
			
			// info.AddValue("ObservableCollection", (object) list, typeof(ObservableCollection<KeyValuePair<TKey, TValue>>));
		}

	}
}