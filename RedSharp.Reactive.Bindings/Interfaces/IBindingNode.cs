using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Sys.Interfaces.Shared;

namespace RedSharp.Reactive.Bindings.Interfaces
{
    /// <summary>
    /// The basic interface for the binding chains.
    /// <br/>Represents the single object that has to be observable.
    /// </summary>
    /// <remarks>
    /// This is a <see cref="IFreezable"/> object, and it has to be frozen only once.
    /// <br/> Also this object has to create non-frozen clone of itself without <see cref="Previous"/> and <see cref="Following"/> nodes.
    /// </remarks>
    public interface IBindingNode : IDisposable, IDisposeIndication, IFreezable, IFreezeIndication, ICloneable
    {
        /// <summary>
        /// The previous node, that has to update current if has any changes.
        /// </summary>
        IBindingNode Previous { get; set; }

        /// <summary>
        /// The next node that has to updated by this object if it has any changes.
        /// </summary>
        IBindingNode Following { get; set; }

        /// <summary>
        /// Returns true if the node can be applied as a <see cref="Previous"/>
        /// <br/>Can accept null without exceptions.
        /// </summary>
        bool CanAcceptPreviousNode(IBindingNode node);

        /// <summary>
        /// Returns true if the node can be applied as a <see cref="Following"/>
        /// <br/>Can accept null without exceptions.
        /// </summary>
        bool CanAcceptFollowingNode(IBindingNode node);

        /// <summary>
        /// Updates a chain from this node to the end node.
        /// </summary>
        /// <remarks>
        /// It is not recommended to call this method manually, can cause performance degradation.
        /// </remarks>
        void Update();
    }

    /// <summary>
    /// Version of the <see cref="IBindingNode"/> with the strong value.
    /// </summary>
    /// <remarks>
    /// Unfortunately, I cannot unite these two interfaces, 
    /// so nodes authors has to cast every time they want to use this thing.
    /// </remarks>
    public interface IBindingNode<TValue> : IBindingNode
    {
        /// <summary>
        /// Marks that the binding node cannot set a value to the source.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Current value of the observable source.
        /// </summary>
        TValue Value { get; set; }
    }
}
