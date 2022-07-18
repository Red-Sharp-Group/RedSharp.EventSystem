using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Implementation of <see cref="IList{T}"/> with reactivity
    /// </summary>
    /// <remarks>
    /// The property <see cref="IReactiveList{T}.Count"/> can be observed (the interface inherits <see cref="IReactiveEntity"/>)
    /// </remarks>
    public interface IReactiveList<TItem> : IList<TItem>, IReactiveEnumerable<TItem>, IReactiveEntity
    { }
}
