using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Reactive.Bindings.Structures;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// It looks like <see cref="CollectionItemsBindingNode{TInput, TOutput}"/>
    /// <br/><inheritdoc cref="CollectionItemsBindingNode{TInput, TOutput}"/>
    /// But this node also can react on the collection changed event.
    /// </summary>
    public class NotifyCollectionChangedBindingNode<TInput, TOutput, TCollection> : BindingNodeBase<TCollection, IList<TOutput>> where TCollection : INotifyCollectionChanged, IEnumerable<TInput>
    {
        private BindingExpression<TInput, TOutput> _expression;
        private List<IBindingExpression<TInput, TOutput>> _expressionsList;
        private ExpressionsReadOnlyList<TInput, TOutput> _valuesList;

        private bool _isUpdateLocked;

        public NotifyCollectionChangedBindingNode(BindingChain<TInput, TOutput> chain)
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

            return new NotifyCollectionChangedBindingNode<TInput, TOutput, TCollection>(chain);
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
        /// Contains reactions on the observed collection changed.
        /// </summary>
        private void OnDataContextCollectionChanged(Object sender, NotifyCollectionChangedEventArgs arguments)
        {
            switch (arguments.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(arguments.NewItems.OfType<TInput>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(arguments.OldItems.OfType<TInput>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    AddItems(arguments.NewItems.OfType<TInput>());
                    RemoveItems(arguments.OldItems.OfType<TInput>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearItems();
                    break;
            }

            Following?.Update();
        }

        /// <summary>
        /// Removes expressions from the previous collection and makes them for the new one.
        /// <br/> Reassign event sibscribition.
        /// </summary>
        protected override void OnInputUpdate(TCollection previous, TCollection current)
        {
            ClearItems();

            if (previous != null)
                previous.CollectionChanged -= OnDataContextCollectionChanged;

            if (current != null)
            {
                AddItems(current);

                current.CollectionChanged += OnDataContextCollectionChanged;
            }

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

        /// <summary>
        /// Removes items from the input collection.
        /// </summary>
        /// <remarks>
        /// This is a heavy function because this node holds expressions, 
        /// not a raw values, so the corresponded expression has to be found first.
        /// </remarks>
        private void RemoveItems(IEnumerable<TInput> items)
        {
            try
            {
                _isUpdateLocked = true;

                foreach (var item in items)
                {
                    var index = IndexOf(item);

                    if (index != -1)
                    {
                        _expressionsList[index].Dispose();

                        _expressionsList.RemoveAt(index);
                    }
                }
            }
            finally
            {
                _isUpdateLocked = false;
            }
        }

        /// <summary>
        /// Copy-pasted method to find correspond expression's index by the input item.
        /// </summary>
        private int IndexOf(TInput item)
        {
            for (int i = 0; i < _expressionsList.Count; i++)
                if (EqualityComparer<TInput>.Default.Equals(_expressionsList[i].StartNode.Value, item))
                    return i;

            return -1;
        }

        protected override void InternalDispose(bool manual)
        {
            if (CachedInputValue != null)
                CachedInputValue.CollectionChanged -= OnDataContextCollectionChanged;

            ClearItems();

            base.InternalDispose(manual);
        }
    }
}
