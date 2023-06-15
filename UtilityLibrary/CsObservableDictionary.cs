#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
		IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>

	{
		private ObservableCollection<KeyValuePair<TKey, TValue>> list;
		private ICollection<TKey> keys;
		private ICollection<TValue> values;

		private KeyValuePair<TKey, TValue> matched;
		private int matchedIdx;
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

	#endregion


	#region indexer

		public TValue this[TKey key]
		{
			get
			{
				nullKeyTest(key);

				if (ContainsKey(key))
				{
					return matched.Value;
				}

				throw new KeyNotFoundException();
			}
			set
			{
				if (ContainsKey(key))
				{
					matched = new KeyValuePair<TKey, TValue>(key, value);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		public KeyValuePair<TKey, TValue> this[int idx]
		{
			get => list[idx];

			set => list[idx] = value;
		}

	#endregion

		public void Add(TKey key, TValue value)
		{
			existKeyTest(key);

			list.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			nullKeyTest(key);

			for (var i = 0; i < list.Count; i++)
			{
				if (list[i].Key.Equals(key))
				{
					matched = list[i];
					matchedIdx = i;
					return true;
				}
			}

			return false;
		}

		public bool ContainsValue(TValue value)
		{
			for (var i = 0; i < list.Count; i++)
			{
				if (list[i].Value.Equals(value))
				{
					matched = list[i];
					matchedIdx = i;
					return true;
				}
			}

			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public bool Remove(TKey key)
		{
			nullKeyTest(key);

			if (ContainsKey(key))
			{
				list.RemoveAt(matchedIdx);
				return true;
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool result;
			value = default(TValue);

			try
			{
				if (ContainsKey(key))
				{
					result = true;
					value = matched.Value;
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

		public int IndexOf(TKey key)
		{
			if (ContainsKey(key))
			{
				return matchedIdx;
			}

			return -1;
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
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