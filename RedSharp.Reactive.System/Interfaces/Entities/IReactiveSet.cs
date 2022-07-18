using System.Collections.Generic;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Implementation of <see cref="ISet{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReactiveSet{T}.Count"/> can be observed (the interface inherits <see cref="IReactiveEntity"/>)
    /// </remarks>
    public interface IReactiveSet<TItem> : ISet<TItem>, IReactiveEnumerable<TItem>, IReactiveEntity
    { }
}
