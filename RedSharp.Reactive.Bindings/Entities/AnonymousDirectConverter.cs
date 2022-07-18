using System;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// Simple implementation of the <see cref="IBindingConverter{TInput, TOutput}"/>
    /// that works on the delegates.
    /// </summary>
    /// <remarks>
    /// All delegates run without try {..} catch {..}
    /// </remarks>
    public class AnonymousDirectConverter<TInput, TOutput> : IBindingConverter<TInput, TOutput>
    {
        private Func<TInput, TOutput> _forwardFunction;
        private Func<TOutput, TInput> _backwardFunction;

        public AnonymousDirectConverter(Func<TInput, TOutput> forward, Func<TOutput, TInput> backward = null)
        {
            ArgumentsGuard.ThrowIfNull(forward);

            _forwardFunction = forward;
            _backwardFunction = backward;
        }

        /// <inheritdoc/>
        public bool IsTwoSide => _backwardFunction != null;

        /// <inheritdoc/>
        public TOutput Forward(TInput value)
        {
            return _forwardFunction.Invoke(value);
        }

        /// <inheritdoc/>
        public TInput Backward(TOutput value)
        {
            return _backwardFunction.Invoke(value);
        }
    }
}
