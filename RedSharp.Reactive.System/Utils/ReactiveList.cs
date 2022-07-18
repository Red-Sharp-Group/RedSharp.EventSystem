using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Utils
{
    /// <summary>
    /// A simple wrapper that implements <see cref="IReactiveList{TItem}"/>.
    /// </summary>
    /// <remarks>
    /// I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ReactiveList<TItem> : ReactiveEnumerableBase<TItem>, IReactiveList<TItem>
    {
        private IList<TItem> _internalCollection;

        /// <summary>
        /// Creates an instance of a <see cref="List{T}"/> as an internal collection.
        /// </summary>
        public ReactiveList() : this(new List<TItem>())
        { }

        /// <summary>
        /// Creates an instance of a <see cref="List{T}"/> as an internal collection with given capacity.
        /// </summary>
        public ReactiveList(int capacity) : this(new List<TItem>(capacity))
        { }

        public ReactiveList(IList<TItem> internalCollection)
        {
            ArgumentsGuard.ThrowIfNull(internalCollection);

            _internalCollection = internalCollection;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Can be observed.
        /// </remarks>
        public int Count => _internalCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _internalCollection.IsReadOnly;

        /// <inheritdoc/>
        public TItem this[int index] 
        { 
            get => _internalCollection[index];
            set
            {
                ThrowIfReadOnly();

                var oldItem = _internalCollection[index];

                if (EqualityComparer<TItem>.Default.Equals(oldItem, value))
                    return;

                _internalCollection[index] = value;

                RaiseReplacing(oldItem, value, index);
            }
        }

        /// <inheritdoc/>
        public void Add(TItem item)
        {
            ThrowIfReadOnly();

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Add(item);

            RaisePropertyChanged(nameof(Count));
            RaiseAdding(item);
        }

        /// <inheritdoc/>
        public bool Contains(TItem item)
        {
            return _internalCollection.Contains(item);
        }

        /// <inheritdoc/>
        public int IndexOf(TItem item)
        {
            return _internalCollection.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, TItem item)
        {
            ThrowIfReadOnly();

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Insert(index, item);

            RaisePropertyChanged(nameof(Count));
            RaiseAdding(item, index);
        }

        /// <inheritdoc/>
        public bool Remove(TItem item)
        {
            ThrowIfReadOnly();

            var index = _internalCollection.IndexOf(item);

            if (index == -1)
                return false;

            RaisePropertyChanging(nameof(Count));

            _internalCollection.RemoveAt(index);

            RaisePropertyChanged(nameof(Count));
            RaiseRemoving(item);

            return true;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();

            var item = _internalCollection[index];

            RaisePropertyChanging(nameof(Count));

            _internalCollection.RemoveAt(index);

            RaisePropertyChanged(nameof(Count));
            RaiseRemoving(item, index);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ThrowIfReadOnly();

            if (_internalCollection.Count == 0)
                return;

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Clear();

            RaisePropertyChanged(nameof(Count));
            RaiseClearing();
        }

        public override IEnumerator<TItem> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException("This is read-only collection.");
        }
    }
}
