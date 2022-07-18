using System.Collections.Generic;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Uses <see cref="IListConverter{TInput, TOutput}"/> to mutate the input list of values.
    /// </summary>
    /// <remarks>
    /// WARNING: Doesn't cache a converted value.
    /// </remarks>
    public class ListConverterBindingNode<TInput, TOutput> : BindingNodeBase<IList<TInput>, TOutput>
    {
        private IListConverter<TInput, TOutput> _converter;
        private bool _isUpdating;

        public ListConverterBindingNode(IListConverter<TInput, TOutput> converter)
        {
            ArgumentsGuard.ThrowIfNull(converter);

            _converter = converter;
        }

        /// <summary>
        /// I .. really don't know how to know that the items from the collection are read-only or not, 
        /// so it makes check by the previous node and by the converter.
        /// </summary>
        public override bool IsReadOnly => base.IsReadOnly || !_converter.IsTwoSide;

        public override TOutput Value 
        { 
            get => _converter.Forward(CachedInputValue); 
            set
            {
                if (_isUpdating )
                    return;

                var previous = (IBindingNode<IList<TInput>>)Previous;

                if (previous == null || previous.Value == null)
                    return;

                try
                {
                    _isUpdating = true;

                    _converter.Backward(previous.Value, value);
                }
                finally
                {
                    _isUpdating = false;
                }

                Update();
            }
        }

        /// <summary><inheritdoc/></summary>
        /// <remarks>
        /// Will send update for the next node only once for all items, <see cref="Value"/> set section.
        /// </remarks>
        public override void Update()
        {
            base.Update();

            if (!_isUpdating)
                Following?.Update();
        }

        public override object Clone()
        {
            return new ListConverterBindingNode<TInput, TOutput>(_converter);
        }
    }
}
