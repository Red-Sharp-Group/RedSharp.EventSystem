using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Nodes that invokes callback on the update.
    /// </summary>
    /// <remarks>
    /// Does not mutate the input value, just passes it to the next node.
    /// </remarks>
    public class CallbackBindingNode<TValue> : BindingNodeBase, IBindingNode<TValue>
    {
        private Action<TValue> _callback;

        public CallbackBindingNode(Action<TValue> callback)
        {
            ArgumentsGuard.ThrowIfNull(callback);

            _callback = callback;
        }

        /// <inheritdoc/>
        public bool IsReadOnly => Previous == null || ((IBindingNode<TValue>)Previous).IsReadOnly;

        /// <inheritdoc/>
        public TValue Value 
        {
            get
            {
                if (Previous == null)
                    return default;
                else
                    return ((IBindingNode<TValue>)Previous).Value;
            }
            set
            {
                ThrowIfReadOnly();

                ((IBindingNode<TValue>)Previous).Value = value;
            }
        }

        /// <inheritdoc/>
        public override bool CanAcceptPreviousNode(IBindingNode node)
        {
            return node == null || node is IBindingNode<TValue>;
        }

        /// <summary>
        /// Invokes callback in the safe statement.
        /// </summary>
        public override void Update()
        {
            try
            {
                _callback.Invoke(Value);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            Following?.Update();
        }

        public override object Clone()
        {
            return new CallbackBindingNode<TValue>(_callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException("You cannot perform this operation because the node is in read-only state.");
        }
    }
}
