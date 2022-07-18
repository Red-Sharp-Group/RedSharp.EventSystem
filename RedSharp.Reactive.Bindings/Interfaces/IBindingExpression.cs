using System;
using System.Collections.Generic;
using RedSharp.Sys.Interfaces.Shared;

namespace RedSharp.Reactive.Bindings.Interfaces
{
    /// <summary>
    /// The object that contains the full chain of <see cref="IBindingNode"/>
    /// </summary>
    /// <remarks>
    /// After cloning has to be non-frozen.
    /// Cannot be unfrozen.
    /// </remarks>
    public interface IBindingExpression<TInput, TOutput> : IEnumerable<IBindingNode>, IDisposable, IDisposeIndication, IFreezable, IFreezeIndication, ICloneable
    {
        /// <summary>
        /// The first node of the chain.
        /// </summary>
        /// <remarks>
        /// Usually it is a node "placeholder" where 
        /// its value can be only set by external code.
        /// </remarks>
        IBindingNode<TInput> StartNode { get; }

        /// <summary>
        /// The last node of the chain.
        /// </summary>
        IBindingNode<TOutput> EndNode { get; }

        /// <summary>
        /// Number of the nodes inside this expression.
        /// </summary>
        int Count { get; }
    }
}
