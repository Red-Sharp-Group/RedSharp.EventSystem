using System;
using RedSharp.Events.Sys.Helpers;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Abstracts;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Utils
{
    /// <summary>
    /// <inheritdoc cref="StrongReferenceObservable{TItem}"/>
    /// </summary>
    /// <remarks>
    /// Uses weak references in its internal work
    /// </remarks>
    public class WeakReferenceObservable<TItem> : WeakShrinkableCollectionBase<IObserver<TItem>>, IEventSource<TItem>
    {
        private Object _lock;

        public WeakReferenceObservable()
        {
            _lock = new Object();
        }
        
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                AddNewItem(observer);

            return new StrongReferenceListener<TItem>(this, observer);
        }

        /// <inheritdoc/>
        public bool Unsubscribe(IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(observer);

            lock (_lock)
                return RemoveFirstInput(observer);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseOnNext{TItem}(object, WeakReference{IObserver{TItem}}[], bool[], TItem)"/>
        public void RaiseNext(TItem value)
        {
            InlineRaiseHelper.InlineRaiseOnNext(_lock, ElementsBuffer, IsAliveBuffer, value);

            lock(_lock)
                if (NeedToDecreaseBuffer())
                    TryDecreaseBuffer();
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseCompleted{TItem}(WeakReference{IObserver{TItem}}[], bool[])"/>
        public void RaiseCompleted()
        {
            InlineRaiseHelper.InlineRaiseCompleted(ElementsBuffer, IsAliveBuffer);

            InitializeDefaultMembers();
        }
    }
}
