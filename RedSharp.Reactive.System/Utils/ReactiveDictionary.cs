using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Sys.Interfaces.Entities;

namespace RedSharp.Reactive.Sys.Utils
{
    /// <summary>
    /// A simple wrapper that implements <see cref="IReactiveDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// WARNING This collection cannot perform <see cref="INotifyCollectionChanged"/> operation 
    /// in a way that does f.e. <see cref="SortedList{TKey, TValue}"/> so it cannot imitate it fully.
    /// <br/>Also, I have to warn you that this is a wrapper object with an additional functionality,
    /// so it may require more actions to do the same things.
    /// </remarks>
    public class ReactiveDictionary<TKey, TValue> : ReactiveEnumerableBase<KeyValuePair<TKey, TValue>>, IReactiveDictionary<TKey, TValue>
    {
        /// <summary>
        /// Special collection that allows to raise events from outside.
        /// </summary>
        /// <remarks>
        /// This sweet object allows to use protected methods by external objects.
        /// <br/>This is a typical "Public Morozov" anti-pattern (search Pavlik Morozov story), 
        /// but as it is a private type for anyone but <see cref="ReactiveDictionary{TKey, TValue}"/> this is OK.
        /// <br/> I also don't want to comment the methods, just because they all contain only one line and their names say enough.
        /// </remarks>
        private class InternalReactiveCollection<TItem> : ReactiveCollection<TItem>
        {
            public InternalReactiveCollection(ICollection<TItem> collection) : base(collection)
            { }

            public void InternalCountRaisePropertyChanging() => RaisePropertyChanging(nameof(Count));

            public void InternalCountRaisePropertyChanged() => RaisePropertyChanged(nameof(Count));

            public void InternalRaiseAdding(TItem item) => RaiseAdding(item);

            public void InternalRaiseReplacing(TItem oldItem, TItem newItem, int? index = null) => RaiseReplacing(oldItem, newItem, index);

            public void InternalRaiseRemoving(TItem item) => RaiseRemoving(item);

            public void InternalRaiseClearing() => RaiseClearing();
        }

        private IDictionary<TKey, TValue> _internalCollection;

        private InternalReactiveCollection<TKey> _keys;
        private InternalReactiveCollection<TValue> _values;

        /// <inheritdoc/>
        public TValue this[TKey key] 
        { 
            get => _internalCollection[key]; 
            set
            {
                ThrowIfReadOnly();

                if (!_internalCollection.ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    var oldItem = _internalCollection[key];

                    _internalCollection[key] = value;

                    RaiseReplacing(new KeyValuePair<TKey, TValue>(key, oldItem), new KeyValuePair<TKey, TValue>(key, value));

                    _values.InternalRaiseReplacing(oldItem, value);
                }
            }
        }

        /// <inheritdoc/>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;

        /// <inheritdoc/>
        IReactiveCollection<TKey> IReactiveDictionary<TKey, TValue>.Keys => _keys;

        /// <summary>
        /// The explicit <see cref="ReactiveCollection{TItem}"/> that represents keys of the dictionary.
        /// </summary>
        public ReactiveCollection<TKey> Keys => _keys;


        /// <inheritdoc/>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _values;

        /// <inheritdoc/>
        IReactiveCollection<TValue> IReactiveDictionary<TKey, TValue>.Values => _values;

        /// <summary>
        /// The explicit <see cref="ReactiveCollection{TItem}"/> that represents values of the dictionary.
        /// </summary>
        public ReactiveCollection<TValue> Values => _values;

        /// <inheritdoc/>
        public bool IsReadOnly => _internalCollection.IsReadOnly;

        /// <inheritdoc/>
        public int Count => _internalCollection.Count;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            ThrowIfReadOnly();

            if (_internalCollection.ContainsKey(key))
                throw new ArgumentException("The key is already presented.");

            _keys.InternalCountRaisePropertyChanging();
            _values.InternalCountRaisePropertyChanging();

            RaisePropertyChanging(nameof(Count));

            _internalCollection.Add(key, value);

            _keys.InternalRaiseAdding(key);
            _values.InternalRaiseAdding(value);
            _keys.InternalCountRaisePropertyChanged();
            _values.InternalCountRaisePropertyChanged();

            RaiseAdding(new KeyValuePair<TKey, TValue>(key, value));
            RaisePropertyChanged(nameof(Count));
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _internalCollection.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_internalCollection.TryGetValue(item.Key, out TValue value))
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            else
                return false;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _internalCollection.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            ThrowIfReadOnly();

            if (_internalCollection.TryGetValue(key, out TValue value))
            {
                _keys.InternalCountRaisePropertyChanging();
                _values.InternalCountRaisePropertyChanging();

                RaisePropertyChanging(nameof(Count));

                _internalCollection.Remove(key);

                _keys.InternalRaiseRemoving(key);
                _values.InternalRaiseRemoving(value);
                _keys.InternalCountRaisePropertyChanged();
                _values.InternalCountRaisePropertyChanged();

                RaiseRemoving(new KeyValuePair<TKey, TValue>(key, value));
                RaisePropertyChanged(nameof(Count));

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowIfReadOnly();

            if (_internalCollection.TryGetValue(item.Key, out TValue value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                _keys.InternalCountRaisePropertyChanging();
                _values.InternalCountRaisePropertyChanging();

                RaisePropertyChanging(nameof(Count));

                _internalCollection.Remove(item.Key);

                _keys.InternalRaiseRemoving(item.Key);
                _values.InternalRaiseRemoving(value);
                _keys.InternalCountRaisePropertyChanged();
                _values.InternalCountRaisePropertyChanged();

                RaiseRemoving(item);
                RaisePropertyChanged(nameof(Count));

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ThrowIfReadOnly();

            _keys.InternalCountRaisePropertyChanging();
            _values.InternalCountRaisePropertyChanging();

            _internalCollection.Clear();

            _keys.InternalRaiseClearing();
            _values.InternalRaiseClearing();
            _keys.InternalCountRaisePropertyChanged();
            _values.InternalCountRaisePropertyChanged();

            RaiseClearing();
            RaisePropertyChanged(nameof(Count));
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
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
