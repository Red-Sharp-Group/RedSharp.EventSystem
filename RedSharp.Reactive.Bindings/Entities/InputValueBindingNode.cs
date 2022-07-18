using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Abstracts;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Node placeholder, I use it for the start of a binding expression.
    /// </summary>
    /// <remarks>
    /// Doesn't support <see cref="Previous"/> node, 
    /// <see cref="CanAcceptPreviousNode(IBindingNode)"/> is always false.
    /// </remarks>
    public class InputValueBindingNode<TItem> : DisposableBase, IBindingNode<TItem>
    {
        private TItem _value;

        private bool _isFollowingChanging;

        private IBindingNode _following;

        /// <inheritdoc/>
        /// <remarks>
        /// NOT SUPPORTED for any action
        /// </remarks>
        /// <exception cref="NotSupportedException"/>
        public IBindingNode Previous
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IBindingNode Following
        {
            get => _following;
            set
            {
                if (_isFollowingChanging)
                    return;

                ThrowIfDisposed();
                ThrowIfFrozen();

                if (!CanAcceptFollowingNode(value))
                    throw new ArgumentException("The input node cannot be accepted.");

                if (value != null && !value.CanAcceptPreviousNode(this))
                    throw new ArgumentException("The input node cannot accept this node.");

                try
                {
                    _isFollowingChanging = true;

                    if (_following == value)
                        return;

                    var old = _following;

                    _following = value;

                    if (old != null)
                        old.Previous = null;

                    if (_following != null)
                        _following.Previous = this;

                    _following?.Update();
                }
                finally
                {
                    _isFollowingChanging = false;
                }
            }
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Always support value get/set, is used as "DataContext".
        /// </summary>
        /// <remarks>
        /// If you set a new value it will update a whole chain.
        /// </remarks>
        public TItem Value
        {
            get => _value;
            set
            {
                ThrowIfDisposed();

                if (EqualityComparer<TItem>.Default.Equals(_value, value))
                    return;

                _value = value;

                Following?.Update();
            }
        }

        /// <inheritdoc/>
        public bool IsFrozen { get; protected set; }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool CanAcceptPreviousNode(IBindingNode node) => false;

        /// <summary>
        /// Always true.
        /// </summary>
        public bool CanAcceptFollowingNode(IBindingNode node) => true;

        /// <inheritdoc/>
        public void Freeze()
        {
            IsFrozen = true;
        }

        /// <summary> <inheritdoc/> </summary>
        /// <remarks>
        /// NOT SUPPORTED according to <see cref="IBindingNode"/> description.
        /// </remarks>
        public void Unfreeze()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Does nothing, this item doesn't support manual update 
        /// to prevent multiple updates for no reason.
        /// </remarks>
        public void Update()
        {
            ThrowIfDisposed();
            
            /*AND DOES NOTHING, because this item cannot have a parent*/
        }

        public object Clone()
        {
            ThrowIfDisposed();

            return new InputValueBindingNode<TItem>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfFrozen()
        {
            if (IsFrozen)
                throw new Exception("You cannot change the frozen object.");
        }
    }
}
