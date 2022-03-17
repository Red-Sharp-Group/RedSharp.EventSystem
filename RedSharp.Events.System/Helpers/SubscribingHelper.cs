using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Utils;
using RedSharp.Sys.Helpers;

namespace System
{
    /// <summary>
    /// A little helper class, that helps to subscribe delegate to <see cref="IObservable{T}"/>
    /// </summary>
    /// <remarks>
    /// Yes, I put it in the same namespace as <see cref="IObservable{T}"/> and <see cref="IObserver{T}"/>, this is not a mistake.
    /// </remarks>
    public static class SubscribingHelper
    {
        /// <summary>
        /// Subscribes a delegate on input <see cref="IObservable{T}"/> source object.
        /// </summary>
        /// <exception cref="ArgumentNullException">If input source is null.</exception>
        /// <exception cref="ArgumentNullException">If input onNext parameter is null.</exception>
        public static IDisposable Subscribe<TItem>(this IObservable<TItem> source, Action<TItem> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            ArgumentsGuard.ThrowIfNull(source);

            var subscriber = new ObserverDelegateWrapper<TItem>(onNext, onError, onCompleted);

            return source.Subscribe(subscriber);
        }
    }
}
