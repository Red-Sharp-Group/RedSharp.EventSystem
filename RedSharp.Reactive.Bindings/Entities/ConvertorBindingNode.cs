using System;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Uses <see cref="IBindingConverter{TInput, TOutput}"/> to mutate the input value.
    /// </summary>
    /// <remarks>
    /// WARNING: Doesn't cache a converted value.
    /// </remarks>
    public class ConvertorBindingNode<TInput, TOutput> : BindingNodeBase<TInput, TOutput>
    {
        private IBindingConverter<TInput, TOutput> _converter;

        public ConvertorBindingNode(IBindingConverter<TInput, TOutput> converter)
        {
            ArgumentsGuard.ThrowIfNull(converter);

            _converter = converter;
        }

        /// <summary>
        /// Will be writable when converter and previous node support it.
        /// </summary>
        public override bool IsReadOnly => base.IsReadOnly || !_converter.IsTwoSide;

        public override TOutput Value
        {
            get => _converter.Forward(CachedInputValue);
            set
            {
                ThrowIfReadOnly();

                ((IBindingNode<TInput>)Previous).Value = _converter.Backward(value);
            }
        }

        public override object Clone()
        {
            return new ConvertorBindingNode<TInput, TOutput>(_converter);
        }

        protected override void OnInputUpdate(TInput previous, TInput current)
        {
            Following?.Update();
        }

        protected override void InternalDispose(bool manual)
        {
            _converter = null;

            base.InternalDispose(manual);
        }
    }
}
