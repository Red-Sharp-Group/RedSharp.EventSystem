using System;
using System.Collections.Generic;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Reactive.Bindings.Structures;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Uses the input binding chain for all elements from the collection from the previous node.
    /// </summary>
    public class CollectionItemsBindingNode<TInput, TOutput> : BindingNodeBase<IEnumerable<TInput>, IList<TOutput>>
    {
        private BindingExpression<TInput, TOutput> _expression;
        private List<IBindingExpression<TInput, TOutput>> _expressionsList;
        private ExpressionsReadOnlyList<TInput, TOutput> _valuesList;

        private bool _isUpdateLocked;

        public CollectionItemsBindingNode(BindingChain<TInput, TOutput> chain)
        {
            chain = chain.Append(new CallbackBindingNode<TOutput>(OnAnyChainUpdated));

            _expression = new BindingExpression<TInput, TOutput>(chain);
            _expressionsList = new List<IBindingExpression<TInput, TOutput>>();
            _valuesList = new ExpressionsReadOnlyList<TInput, TOutput>(_expressionsList);
        }

        /// <summary>
        /// Always read-only, internal collection it is simply imitation.
        /// </summary>
        public override bool IsReadOnly => true;

        public override IList<TOutput> Value
        {
            get => _valuesList;
            set => throw new NotSupportedException();
        }

        public override object Clone()
        {
            var chain = new BindingChain<TInput, TOutput>(_expression.StartNode, (IBindingNode<TOutput>)_expression.EndNode.Previous);

            return new CollectionItemsBindingNode<TInput, TOutput>(chain);
        }

        /// <summary>
        /// Updates next node if any of the internal expressions is updated.
        /// </summary>
        /// <param name="output"></param>
        private void OnAnyChainUpdated(TOutput output)
        {
            if (!_isUpdateLocked)
                Following?.Update();
        }

        /// <summary>
        /// Removes expressions from the previous collection and makes them for the new one.
        /// </summary>
        protected override void OnInputUpdate(IEnumerable<TInput> previous, IEnumerable<TInput> current)
        {
            ClearItems();

            if(current != null)
                AddItems(current);

            Following?.Update();
        }

        /// <summary>
        /// Creates an expression for each item in the items.
        /// </summary>
        protected void AddItems(IEnumerable<TInput> items)
        {
            try
            {
                _isUpdateLocked = true;

                foreach (var item in items)
                {
                    var expression = (IBindingExpression<TInput, TOutput>)_expression.Clone();

                    expression.StartNode.Value = item;
                    expression.Freeze();

                    _expressionsList.Add(expression);
                }
            }
            finally 
            {
                _isUpdateLocked = false;
            }
        }

        /// <summary>
        /// Removes all existed expressions.
        /// </summary>
        protected void ClearItems()
        {
            try
            {
                _isUpdateLocked = true;

                foreach (var item in _expressionsList)
                    item.Dispose();

                _expressionsList.Clear();
            }
            finally
            {
                _isUpdateLocked = false;
            }
        }

        protected override void InternalDispose(bool manual)
        {
            ClearItems();

            base.InternalDispose(manual);
        }
    }
}
