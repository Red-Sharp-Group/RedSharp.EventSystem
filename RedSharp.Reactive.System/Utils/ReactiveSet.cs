using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Utils
{
    /// <summary>
    /// A simple wrapper that implements <see cref="IReactiveCollection{TItem}"/>.
    /// </summary>
    /// <remarks>
    /// WARNING This collection cannot perform <see cref="INotifyCollectionChanged"/> operation 
    /// in a way that does f.e. <see cref="SortedSet{T}"/> so it cannot imitate it fully.
    /// <br/>Also, I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ReactiveSet<TItem> : ReactiveEnumerableBase<TItem>, IReactiveSet<TItem>
    {
        private ISet<TItem> _internalCollection;

        /// <summary>
        /// Creates an instance of a <see cref="HashSet{T}"/> as an internal collection.
        /// </summary>
        public ReactiveSet() : this(new HashSet<TItem>())
        { }

        public ReactiveSet(ISet<TItem> internalCollection)
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
        void ICollection<TItem>.Add(TItem item) => Add(item);

        /// <inheritdoc/>
        public bool Add(TItem item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Contains(item))
                return false;

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Add(item);

            RaisePropertyChanged(nameof(Count));
            RaiseAdding(item);

            return true;
        }

        /// <inheritdoc/>
        public bool Contains(TItem item)
        {
            return _internalCollection.Contains(item);
        }

        /// <inheritdoc/>
        public bool Remove(TItem item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.Contains(item))
                return false;

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Remove(item);

            RaisePropertyChanged(nameof(Count));
            RaiseClearing();

            return true;
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

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<TItem> other) => _internalCollection.IntersectWith(other);

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<TItem> other) => _internalCollection.IsProperSubsetOf(other);

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<TItem> other) => _internalCollection.IsProperSupersetOf(other);

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<TItem> other) => _internalCollection.IsSubsetOf(other);

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<TItem> other) => _internalCollection.IsSupersetOf(other);

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<TItem> other) => _internalCollection.Overlaps(other);

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<TItem> other) => _internalCollection.SetEquals(other);

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other);

            var newItems = new List<TItem>();

            foreach (var item in other)
                if (!_internalCollection.Contains(item))
                    newItems.Add(item);

            if (newItems.Count == 0)
                return;

            RaisePropertyChanging(nameof(Count));

            foreach (var item in newItems)
                _internalCollection.Add(item);

            RaisePropertyChanged(nameof(Count));
            RaiseAdding(newItems);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other);

            var oldItems = new List<TItem>();

            foreach (var item in other)
                if (_internalCollection.Contains(item))
                    oldItems.Add(item);

            if (oldItems.Count == 0)
                return;

            RaisePropertyChanging(nameof(Count));

            foreach (var item in oldItems)
                _internalCollection.Remove(item);

            RaisePropertyChanged(nameof(Count));
            RaiseRemoving(oldItems);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<TItem> other)
        {
            ThrowIfReadOnly();
            ArgumentsGuard.ThrowIfNull(other);

            var newItems = new List<TItem>();
            var oldItems = new List<TItem>();

            foreach (var item in other)
            {
                if (_internalCollection.Contains(item))
                    oldItems.Add(item);
                else
                    newItems.Add(item);
            }

            if (oldItems.Count == 0 && newItems.Count == 0)
                return;

            RaisePropertyChanging(nameof(Count));

            foreach (var item in newItems)
                _internalCollection.Add(item);

            foreach (var item in oldItems)
                _internalCollection.Remove(item);

            RaisePropertyChanged(nameof(Count));
            RaiseRemoving(oldItems);
            RaiseAdding(newItems);
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
