using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Helpers;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Abstracts
{
    public abstract class ObservableBase<TItem> : IEventSource2<TItem>
    {
        private ICollection<IObserver<TItem>> _subscribers;
        private Object _lock;

        protected ObservableBase(ICollection<IObserver<TItem>> subscribers)
        {
            _subscribers = subscribers;
            _lock = new Object();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Uses lock statement to prevent using it from several threads simultaneously.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If observer is null.</exception>
        public IDisposable Subscribe(IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                _subscribers.Add(observer);

            return CreateListenerForObserver(observer);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Uses lock statement to prevent using it from several threads simultaneously.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If observer is null.</exception>
        public bool Unsubscribe(IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                return _subscribers.Remove(observer);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseOnNext{TItem}(object, ICollection{IObserver{TItem}}, TItem)"/>
        protected void RaiseNext(TItem value)
        {
            InlineRaiseHelper.InlineRaiseOnNext(_lock, _subscribers, value);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseCompleted{TItem}(ICollection{IObserver{TItem}})"/>
        protected void RaiseCompleted()
        {
            InlineRaiseHelper.InlineRaiseCompleted(_subscribers);
        }

        /// <summary>
        /// Has to return <see cref="IDisposable"/> object for the given <see cref="IObserver{T}"/>.
        /// External code guarantees that the input object is valid.
        /// </summary>
        protected abstract IDisposable CreateListenerForObserver(IObserver<TItem> observer);
    }
}
