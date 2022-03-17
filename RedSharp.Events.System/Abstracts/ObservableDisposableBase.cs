using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Helpers;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Abstracts;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Abstracts
{
    public abstract class ObservableDisposableBase<TItem> : DisposableBase, IEventSource<TItem>
    {
        private ICollection<IObserver<TItem>> _subscribers;
        private Object _lock;

        protected ObservableDisposableBase(ICollection<IObserver<TItem>> subscribers)
        {
            _subscribers = subscribers;
            _lock = new Object();
        }

        /// <inheritdoc cref="ObservableBase{TItem}.Subscribe(IObserver{TItem})"/>
        /// <exception cref="ObjectDisposedException"/>
        public IDisposable Subscribe(IObserver<TItem> observer)
        {
            ThrowIfDisposed();

            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                _subscribers.Add(observer);

            return CreateListenerForObserver(observer);
        }

        /// <inheritdoc cref="ObservableBase{TItem}.Unsubscribe(IObserver{TItem})"/>
        /// <exception cref="ObjectDisposedException"/>
        public bool Unsubscribe(IObserver<TItem> observer)
        {
            ThrowIfDisposed();

            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                return _subscribers.Remove(observer);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseOnNext{TItem}(object, ICollection{IObserver{TItem}}, TItem)"/>
        /// <exception cref="ObjectDisposedException"/>
        protected void RaiseNext(TItem value)
        {
            ThrowIfDisposed();

            InlineRaiseHelper.InlineRaiseOnNext(_lock, _subscribers, value);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseCompleted{TItem}(ICollection{IObserver{TItem}})"/>
        /// <exception cref="ObjectDisposedException"/>
        protected void RaiseCompleted()
        {
            ThrowIfDisposed();

            InlineRaiseHelper.InlineRaiseCompleted(_subscribers);
        }

        /// <inheritdoc cref="ObservableBase{TItem}.CreateListenerForObserver(IObserver{TItem})"/>
        protected abstract IDisposable CreateListenerForObserver(IObserver<TItem> observer);

        protected override void InternalDispose(bool manual)
        {
            RaiseCompleted();

            base.InternalDispose(manual);
        }
    }
}
