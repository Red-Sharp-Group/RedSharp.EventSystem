using System.Collections.Generic;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Implementation of <see cref="IDictionary{TKey, TValue}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReactiveDictionary{TKey, TValue}.Count"/> can be observed (the interface inherits <see cref="IReactiveEntity"/>)
    /// </remarks>
    public interface IReactiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReactiveEnumerable<KeyValuePair<TKey, TValue>>, IReactiveEntity
    {
        /// <summary>
        /// A new read-only collection for the dictionary keys with reactivity.
        /// </summary>
        new IReactiveCollection<TKey> Keys { get; }

        /// <summary>
        /// A new read-only collection for the dictionary values with reactivity.
        /// </summary>
        new IReactiveCollection<TValue> Values { get; }
    }
}
