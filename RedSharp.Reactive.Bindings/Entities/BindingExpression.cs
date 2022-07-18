using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Reactive.Bindings.Structures;
using RedSharp.Sys.Abstracts;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Another default implementation, for this time for <see cref="IBindingExpression{TInput, TOutput}"/>
    /// </summary>
    public class BindingExpression<TInput, TOutput> : DisposableBase, IBindingExpression<TInput, TOutput>
    {
        /// <summary>
        /// Special enumerator that can walk through binding nodes, these links look like nodes from <see cref="LinkedList{T}"/>
        /// </summary>
        private class AnonymousEnumerator : IEnumerator<IBindingNode>
        {
            private IBindingNode _startNode;
            private bool _isStarted;

            public AnonymousEnumerator(IBindingNode startNode)
            {
                _startNode = startNode;

                Reset();
            }

            object IEnumerator.Current => Current;

            public IBindingNode Current { get; private set; }

            public void Dispose() => Reset();

            public bool MoveNext()
            {
                if (!_isStarted)
                {
                    _isStarted = true;

                    Current = _startNode;

                    return true;
                }
                else
                {
                    Current = Current.Following;

                    return Current != null;
                }
            }

            public void Reset()
            {
                Current = default;

                _isStarted = false;
            }
        }

        public BindingExpression(BindingChain<TInput, TOutput> bindingExpression) : this(bindingExpression.Start, bindingExpression.Current)
        { }

        /// <inheritdoc/>
        public IBindingNode<TInput> StartNode { get; private set; }

        /// <inheritdoc/>
        public IBindingNode<TOutput> EndNode { get; private set; }

        /// <inheritdoc/>
        /// <remarks>
        /// Can change after freezing (this process makes recount)
        /// </remarks>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc/>
        public virtual void Freeze()
        {
            IsFrozen = true;

            Count = 0;

            foreach (var item in this)
            {
                item.Freeze();

                Count++;
            }
        }

        /// <summary> <inheritdoc/> </summary>
        /// <remarks>
        /// NOT SUPPORTED according to <see cref="IBindingExpression{TInput, TOutput}"/> description.
        /// </remarks>
        public virtual void Unfreeze()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public object Clone()
        {
            ThrowIfDisposed();

            //TODO simplify this process

            IBindingNode current = StartNode;

            IBindingNode startNode = (IBindingNode)current.Clone();
            IBindingNode currentNew = startNode;

            if (current != null)
            {
                do
                { 
                    current = current.Following;

                    var newNode = (IBindingNode)current.Clone();

                    currentNew.Following = newNode;
                    currentNew = newNode;
                }
                while (current != EndNode);
            }

            return new BindingExpression<TInput, TOutput>((IBindingNode<TInput>)startNode, (IBindingNode<TOutput>)currentNew);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IBindingNode> GetEnumerator()
        {
            ThrowIfDisposed();

            return new AnonymousEnumerator(StartNode);
        }

        private BindingExpression(IBindingNode<TInput> startNode, IBindingNode<TOutput> endNode)
        {
            if (!(startNode is InputValueBindingNode<TInput>))
                throw new ArgumentException("Invalid type of the start node");

            StartNode = startNode;
            EndNode = endNode;

            Count = 1;

            if (startNode == endNode)
                return;

            var current = startNode.Following;

            do
            {
                if (current == null)
                    throw new Exception("Cannot reach the end node.");

                if (current == startNode)
                    throw new Exception("A loop was found.");

                current = current.Following;

                Count++;

            } while (current != EndNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfFrozen()
        {
            if (IsFrozen)
                throw new Exception("You cannot change the frozen object.");
        }

        protected override void InternalDispose(bool manual)
        {
            var node = (IBindingNode)StartNode;

            while (node != null)
            {
                node.Dispose();
                node = node.Following;
            }

            base.InternalDispose(manual);
        }
    }
}
