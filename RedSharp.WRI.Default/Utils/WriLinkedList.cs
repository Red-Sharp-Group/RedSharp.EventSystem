using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedSharp.WRI.Utils
{
    /// <summary>
    /// Special collection that is used weak references to hold an objects.
    /// </summary>
    public class WriLinkedList<TModel> : ICollection<TModel> where TModel : class
    {
        #region Internal Types
        //========================================//
        //Internal Types

        /// <summary>
        /// Special enumerator which will be removing expired objects while enumeration.
        /// </summary>
        private class WriLinkedListEnumerator<TSubModel> : IEnumerator<TSubModel> where TSubModel : class
        {
            #region Members
            //========================================//
            //Members

            private LinkedList<WeakReference<TSubModel>> _items;
            private LinkedListNode<WeakReference<TSubModel>> _currentNode;

            #endregion

            #region Public
            //========================================//
            //Public

            public WriLinkedListEnumerator(LinkedList<WeakReference<TSubModel>> items)
            {
                _items = items;
            }

            /// <inheritdoc/>
            object IEnumerator.Current => Current;

            /// <inheritdoc/>
            public TSubModel Current { get; private set; }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                if (_currentNode == null)
                    _currentNode = _items.First;
                else
                    _currentNode = _currentNode.Next;

                while (_currentNode != null)
                {
                    if (_currentNode.Value.TryGetTarget(out TSubModel target))
                    {
                        Current = target;

                        return true;
                    }
                    else
                    {
                        var nodeToRemove = _currentNode;

                        _currentNode = _currentNode.Next;

                        _items.Remove(nodeToRemove);
                    }
                }

                return false;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                Current = null;

                _currentNode = null;
            }

            /// <summary>
            /// Reset and clear the internal collection.
            /// </summary>
            public void Dispose()
            {
                Reset();

                _items = null;
            }

            #endregion
        }

        #endregion

        #region Members
        //========================================//
        //Members

        private LinkedList<WeakReference<TModel>> _items;

        #endregion

        #region Public
        //========================================//
        //Public

        public WriLinkedList()
        {
            _items = new LinkedList<WeakReference<TModel>>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Will check and remove expired items every time, so do not use it frequently.
        /// </summary>
        public int Count 
        {
            get
            {
                if (_items.Count == 0)
                    return 0;

                var currentNode = _items.First;

                while (currentNode != null)
                {
                    if (!currentNode.Value.TryGetTarget(out TModel noOneCare))
                    {
                        var needToRemove = currentNode;

                        currentNode = currentNode.Next;

                        _items.Remove(needToRemove);
                    }
                    else
                    {
                        currentNode = currentNode.Next;
                    }
                }

                return _items.Count;
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Current collection do not support null values;
        /// </exception>
        public void Add(TModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var weakReference = new WeakReference<TModel>(item);

            _items.AddLast(new LinkedListNode<WeakReference<TModel>>(weakReference));
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _items.Clear();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Current collection do not support null values;
        /// </exception>
        public bool Contains(TModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            foreach (var tempItem in this)
                if (tempItem == item)
                    return true;

            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Current collection do not support null values;
        /// </exception>
        public bool Remove(TModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.Count == 0)
                return false;

            LinkedListNode<WeakReference<TModel>> currentNode = _items.First;
            LinkedListNode<WeakReference<TModel>> nodeToRemove = null;

            bool nodeWasFound = false;

            while(currentNode != null)
            {
                if(currentNode.Value.TryGetTarget(out TModel target))
                {
                    if (!nodeWasFound && target == item)
                    {
                        nodeToRemove = currentNode;

                        nodeWasFound = true;
                    }
                }
                else
                {
                    nodeToRemove = currentNode;
                }

                if (nodeToRemove != null)
                {
                    _items.Remove(nodeToRemove);

                    nodeToRemove = null;
                }

                currentNode = currentNode.Next;
            }

            return nodeWasFound;
        }

        /// <inheritdoc/>
        public void CopyTo(TModel[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            this.ToArray().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<TModel> GetEnumerator()
        {
            return new WriLinkedListEnumerator<TModel>(_items);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new WriLinkedListEnumerator<TModel>(_items);
        }

        #endregion
    }
}
