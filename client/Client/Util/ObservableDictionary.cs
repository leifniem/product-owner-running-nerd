using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LoadRunnerClient
{
	/// <summary>
	/// Utility class providing functionality C# utterly misses
	/// </summary>
	/// <typeparam name="TKey">Type of Keys</typeparam>
	/// <typeparam name="TValue">Type of Value</typeparam>
    public class ObservableDictionary<TKey, TValue> :
     IDictionary<TKey, TValue>,
     INotifyCollectionChanged,
     INotifyPropertyChanged
    {
        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private List<TKey> _keyList = new List<TKey>();

        #region Implementation of INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Implementation of INotifyCollectionChanged

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Implementation of INotifyPropertyChanged

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Implementation of IEnumerable

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item);
            _keyList.Add(item.Key);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }
        }

        public void Clear()
        {
            int keysCount = _dictionary.Keys.Count;

            _dictionary.Clear();
            _keyList.Clear();

            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool remove = _dictionary.Remove(item);
            int index = _keyList.IndexOf(item.Key);
            if (!remove) return false;
            _keyList.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item.Value, index));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
            return true;
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        #endregion Implementation of ICollection<KeyValuePair<TKey,TValue>>

        #region Implementation of IDictionary<TKey,TValue>

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;
            _dictionary.Add(key, value);
            if (!_keyList.Contains(key))
                _keyList.Add(key);

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _dictionary[key]));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }
        }

        public bool Remove(TKey key)
        {
            TValue value = _dictionary[key];
            int index = _keyList.IndexOf(key);
            bool remove = _dictionary.Remove(key);

            if (!remove) return false;

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set
            {
                bool changed = _dictionary[key].Equals(value);

                if (!changed) return;
                TValue oldObject = _dictionary[key];
                _dictionary[key] = value;

                if (CollectionChanged != null)
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldObject));

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Values"));
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        #endregion Implementation of IDictionary<TKey,TValue>

        public BlockCursor BlockCursor
        {
            get => default(BlockCursor);
            set
            {
            }
        }
    }
}