using System;
using System.Collections.Generic;
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
    public class AnonymousListConverter<TInput, TOutput> : IListConverter<TInput, TOutput>
    {
        private Func<IList<TInput>, TOutput> _forwardFunction;
        private Action<IList<TInput>, TOutput> _backwardFunction;

        public AnonymousListConverter(Func<IList<TInput>, TOutput> forward, Action<IList<TInput>, TOutput> backward = null)
        {
            ArgumentsGuard.ThrowIfNull(forward);

            _forwardFunction = forward;
            _backwardFunction = backward;
        }

        /// <inheritdoc/>
        public bool IsTwoSide => _backwardFunction != null;

        /// <inheritdoc/>
        public TOutput Forward(IList<TInput> collection)
        {
            return _forwardFunction.Invoke(collection);
        }

        /// <inheritdoc/>
        public void Backward(IList<TInput> collection, TOutput value)
        {
            _backwardFunction.Invoke(collection, value);
        }
    }
}
