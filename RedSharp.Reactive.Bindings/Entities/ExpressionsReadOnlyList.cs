using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Special read-only collection that allows only read-only actions (and setting by indexer)
    /// </summary>
    public class ExpressionsReadOnlyList<TInput, TOutput> : IList<TOutput>
    {
        /// <summary>
        /// Enumerator that returns a bunch of expressions values.
        /// </summary>
        private class ExpressionsToValueEnumerator<TOutput> : IEnumerator<TOutput>
        {
            private IList<IBindingExpression<TInput, TOutput>> _collection;
            private int _index;

            public ExpressionsToValueEnumerator(IList<IBindingExpression<TInput, TOutput>> collection)
            {
                _collection = collection;

                Reset();
            }

            object IEnumerator.Current => Current;

            public TOutput Current { get; private set; }

            public void Dispose() => Reset();

            public bool MoveNext()
            {
                _index++;

                if (_index < _collection.Count)
                {
                    Current = _collection[_index].EndNode.Value;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                Current = default;
                _index = -1;
            }
        }

        private IList<IBindingExpression<TInput, TOutput>> _collection;

        public ExpressionsReadOnlyList(IList<IBindingExpression<TInput, TOutput>> collection)
        {
            ArgumentsGuard.ThrowIfNull(collection);

            _collection = collection;
        }

        /// <inheritdoc/>
        public int Count => _collection.Count;

        /// <summary>
        /// Always true.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Despite the <see cref="IsReadOnly"/> value, it allows to set a value.
        /// </summary>
        public TOutput this[int index]
        {
            get => _collection[index].EndNode.Value;
            set => _collection[index].EndNode.Value = value;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Add(TOutput item) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Insert(int index, TOutput item) => throw new NotSupportedException();

        /// <inheritdoc/>
        public int IndexOf(TOutput item)
        {
            for (int i = 0; i < _collection.Count; i++)
                if (EqualityComparer<TOutput>.Default.Equals(_collection[i].EndNode.Value, item))
                    return i;

            return -1;
        }

        /// <inheritdoc/>
        public bool Contains(TOutput item)
        {
            return _collection.Any(temp => EqualityComparer<TOutput>.Default.Equals(temp.EndNode.Value, item));
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public bool Remove(TOutput item) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        public void RemoveAt(int index) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        public void Clear() => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TOutput> GetEnumerator() => new ExpressionsToValueEnumerator<TOutput>(_collection);

        /// <inheritdoc/>
        /// <remarks>
        /// Not optimized.
        /// </remarks>
        public void CopyTo(TOutput[] array, int arrayIndex)
        {
            //TODO this was written fast, but works slow.
            _collection.Select(item => item.EndNode.Value)
                       .ToList()
                       .CopyTo(array, arrayIndex);
        }
    }
}
