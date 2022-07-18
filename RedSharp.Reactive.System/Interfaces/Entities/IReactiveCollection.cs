using System.Collections.Generic;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Implementation of <see cref="ICollection{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReactiveCollection{T}.Count"/> can be observed (the interface inherits <see cref="IReactiveEntity"/>)
    /// </remarks>
    public interface IReactiveCollection<TItem> : ICollection<TItem>, IReactiveEnumerable<TItem>, IReactiveEntity
    { }
}
