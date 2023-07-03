using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Utils
{
    /// <summary>
    /// A wrapper list that now has exposed callbacks that you can take advantage of. I use 
    /// this to sync with the SyncList across the network.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CallbackList<T> : List<T>, ICollection<T>
    {
        #region Delegates
        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;
        public event Action<T, int> OnSetIndex;
        public event Action OnClear;
        #endregion

        #region List
        [SerializeField]
        private List<T> _lst = new List<T>();
        #endregion

        #region Refs
        public new int Count => _lst.Count;
        public bool IsReadOnly => throw new System.NotImplementedException();
        public new T this[int index]
        {
            get
            {
                return _lst[index];
            }
            set
            {
                _lst[index] = value;
                OnSetIndex?.Invoke(value, index);
            }
        }
        #endregion

        #region Methods
        public new void Add(T item)
        {
            _lst.Add(item);
            OnAdded?.Invoke(item);
        }
        public new void Clear()
        {
            OnClear?.Invoke();
            _lst.Clear();
        }
        public new bool Contains(T item)
        {
            return _lst.Contains(item);
        }
        public new void CopyTo(T[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }
        public new bool Remove(T item)
        {
            if (_lst.Remove(item))
            {
                OnRemoved?.Invoke(item);
                return true;
            }

            return false;
        }
        public new int IndexOf(T item)
        {
            return _lst.IndexOf(item);
        }
        public new T Find(Predicate<T> match)
        {
            return _lst.Find(match);
        }
        public new List<T> FindAll(Predicate<T> match)
        {
            return _lst.FindAll(match);
        }
        public new bool Exists(Predicate<T> match)
        {
            return _lst.Exists(match);
        }
        #endregion

        #region IEnumerators
        public new IEnumerator<T> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }
        #endregion
    }
}
