using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Utils
{
    /// <summary>
    /// Simple wrapper, just to hold a delegate inside.
    /// </summary>
    public class ObserverDelegateWrapper<TItem> : IObserver<TItem>
    {
        private Action<TItem> _onNextDelegate;
        private Action<Exception> _onErrorDelegate;
        private Action _onCompleteDelegate;

        public ObserverDelegateWrapper(Action<TItem> onNext, Action<Exception> onError = null, Action onComplete = null)
        {
            ArgumentsGuard.ThrowIfNull(onNext);

            _onNextDelegate = onNext;
            _onErrorDelegate = onError;
            _onCompleteDelegate = onComplete;
        }

        /// <inheritdoc/>
        public void OnNext(TItem value)
        {
            _onNextDelegate.Invoke(value);
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            _onErrorDelegate?.Invoke(error);
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            _onCompleteDelegate?.Invoke();
        }
    }
}
