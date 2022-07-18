using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Abstracts;

namespace RedSharp.Reactive.Bindings.Abstracts
{
    /// <summary>
    /// Basic implementation of <see cref="IBindingNode"/>
    /// with the most annoying functionality: <see cref="IBindingNode.Previous"/> and <see cref="IBindingNode.Following"/>
    /// </summary>
    /// <remarks>
    /// I have to warn you - this object IS NOT a thread safe.
    /// </remarks>
    public abstract class BindingNodeBase : DisposableBase, IBindingNode
    {
        //Unfortunately I have to use this two fields
        //to prevent stack overflow, currently I don't know better
        //way to do this. 
        private bool _isPreviousChanging;
        private bool _isFollowingChanging;

        private IBindingNode _previous;
        private IBindingNode _following;

        /// <inheritdoc/>
        /// <remarks>
        /// Provokes <see cref="Update"/> invocation for this object.
        /// Can be used only in unfrozen state.
        /// </remarks>
        public IBindingNode Previous
        {
            get => _previous;
            set
            {
                if (_isPreviousChanging)
                    return;

                ThrowIfDisposed();
                ThrowIfFrozen();

                if (!CanAcceptPreviousNode(value))
                    throw new ArgumentException("The input node cannot be accepted.");

                if (value != null && !value.CanAcceptFollowingNode(value))
                    throw new ArgumentException("The input node cannot accept this node.");

                try
                {
                    _isPreviousChanging = true;

                    if (_previous == value)
                        return;

                    var old = _previous;

                    _previous = value;

                    if (old != null)
                        old.Following = null;

                    if (_previous != null)
                        _previous.Following = this;

                    Update();
                }
                finally
                {
                    _isPreviousChanging = false;
                }
            }
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
                }
                finally
                {
                    _isFollowingChanging = false;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc/>
        public void Freeze()
        {
            IsFrozen = true;
        }

        /// <summary> <inheritdoc/> </summary>
        /// <remarks>
        /// NOT SUPPORTED according to <see cref="IBindingNode"/> description.
        /// </remarks>
        public void Unfreeze() => throw new NotSupportedException();

        /// <inheritdoc/>
        public virtual bool CanAcceptPreviousNode(IBindingNode node)
        {
            return true;
        }

        /// <inheritdoc/>
        public virtual bool CanAcceptFollowingNode(IBindingNode node)
        {
            return true;
        }

        /// <inheritdoc/>
        public abstract void Update();

        /// <inheritdoc/>
        public abstract object Clone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfFrozen()
        {
            if (IsFrozen)
                throw new Exception("You cannot change the frozen object.");
        }

        protected override void InternalDispose(bool manual)
        {
            _previous = null;
            _following = null;

            base.InternalDispose(manual);
        }
    }

    /// <summary>
    /// The implementation of the <see cref="IBindingNode{TValue}"/> with caching of the value from the previous node.
    /// </summary>
    public abstract class BindingNodeBase<TInput, TOutput> : BindingNodeBase, IBindingNode<TOutput>
    {
        private TInput _cachedInputValue;

        /// <inheritdoc/>
        public virtual bool IsReadOnly => Previous == null; //This is a default condition.

        /// <inheritdoc/>
        public abstract TOutput Value { get; set; }

        /// <summary>
        /// Cached value from the previous node.
        /// </summary>
        /// <remarks>
        /// It is needed for many cases, f.e. <see cref="INotifyPropertyChanged"/> 
        /// where you need to subscribe to a new item and unsubscribe from a previous.
        /// <br/>If the previous node is null this property will contain a default value.
        /// </remarks>
        protected virtual TInput CachedInputValue 
        {
            get => _cachedInputValue;
            private set
            {
                if (EqualityComparer<TInput>.Default.Equals(_cachedInputValue, value))
                    return;

                var oldValue = _cachedInputValue;
                var newValue = value;

                _cachedInputValue = value;

                OnInputUpdate(oldValue, newValue);
            }
        }

        /// <inheritdoc/>
        public override bool CanAcceptPreviousNode(IBindingNode node)
        {
            return node == null || node is IBindingNode<TInput>;
        }

        /// <summary>
        /// Updates a <see cref="CachedInputValue"/> or make it default.
        /// </summary>
        /// <remarks>
        /// Internal casting is safe, because the type check is performed by <see cref="CanAcceptPreviousNode(IBindingNode)"/>
        /// </remarks>
        public override void Update()
        {
            ThrowIfDisposed();

            if (Previous == null)
                CachedInputValue = default;
            else
                CachedInputValue = ((IBindingNode<TInput>)Previous).Value;
        }

        /// <summary>
        /// Will be invoked if the <see cref="CachedInputValue"/> was changed, 
        /// to make possible to manipulate with old value and value.
        /// </summary>
        protected virtual void OnInputUpdate(TInput previous, TInput current) 
        { }

        protected override void InternalDispose(bool manual)
        {
            _cachedInputValue = default;

            base.InternalDispose(manual);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfReadOnly()
        {
            if (IsReadOnly)
                throw new NotSupportedException("You cannot perform this operation because the node is in read-only state.");
        }
    }
}
