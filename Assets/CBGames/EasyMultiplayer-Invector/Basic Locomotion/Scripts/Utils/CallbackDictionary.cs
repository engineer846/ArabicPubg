using System;
using System.Collections;
using System.Collections.Generic;

namespace EMI.Utils
{
    public class CallbackDictionary<K, V> : Dictionary<K, V>, ICollection, IDictionary
    {
        #region Delegates
        public event Action<K, V> OnAdded;
        public event Action<K> OnRemoved;
        public event Action<K, V> OnSetIndex;
        public event Action OnClear;
        #endregion

        #region Dictionary
        private Dictionary<K, V> _dict = new Dictionary<K, V>();
        #endregion

        #region Refs
        public new int Count => _dict.Count;
        public new KeyCollection Keys => _dict.Keys;
        public new ValueCollection Values => _dict.Values;
        public bool IsFixedSize => ((IDictionary)_dict).IsFixedSize;
        public bool IsSynchronized => ((IDictionary)_dict).IsSynchronized;
        public object SyncRoot => ((IDictionary)_dict).SyncRoot;
        public bool IsReadOnly => throw new System.NotImplementedException();
        public new V this[K key]
        {
            get { return ((IDictionary<K, V>)_dict)[key]; }
            set 
            { 
                ((IDictionary<K, V>)_dict)[key] = value;
                OnSetIndex?.Invoke(key, value);
            }
        }
        public object this[object key]
        {
            get { return ((IDictionary)_dict)[key]; }
            set 
            { 
                ((IDictionary)_dict)[key] = value;
                OnSetIndex?.Invoke((K)key, (V)value);
            }
        }
        #endregion

        #region Methods
        public void Add(KeyValuePair<K, V> item)
        {
            ((IDictionary<K, V>)_dict).Add(item);
            OnAdded?.Invoke(item.Key, item.Value);
        }
        public new void Add(K key, V value)
        {
            _dict.Add(key, value);
            OnAdded?.Invoke(key, value);
        }
        public new void Clear()
        {
            _dict.Clear();
            OnClear?.Invoke();
        }
        public bool Contains(KeyValuePair<K, V> item)
        {
            return ((IDictionary<K, V>)_dict).Contains(item);
        }
        public bool Contains(object key)
        {
            return ((IDictionary)_dict).Contains(key);
        }
        public new bool ContainsKey(K key)
        {
            return _dict.ContainsKey(key);
        }
        public new bool ContainsValue(V value)
        {
            return _dict.ContainsValue(value);
        }
        public new bool Remove(K key)
        {
            if (_dict.Remove(key))
            {
                OnRemoved?.Invoke(key);
                return true;
            }

            return false;
        }
        public void Remove(object key)
        {
            ((IDictionary)_dict).Remove(key);
            OnRemoved?.Invoke((K)key);
        }
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>)_dict).CopyTo(array, arrayIndex);
        }
        public void CopyTo(Array array, int index)
        {
            ((IDictionary)_dict).CopyTo(array, index);
        }
        public new bool TryGetValue(K key, out V value)
        {
            return _dict.TryGetValue(key, out value);
        }
        #endregion

        #region IEnumerators
        public new IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return ((IDictionary<K, V>)_dict).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<K, V>)_dict).GetEnumerator();
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dict).GetEnumerator();
        }
        #endregion

    }
}
