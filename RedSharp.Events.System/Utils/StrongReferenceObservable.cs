using System;
using RedSharp.Events.Sys.Helpers;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Abstracts;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Utils
{
    /// <summary>
    /// Finished <see cref="IObservable{T}"/> object for using it in composition, 
    /// instead of object that used in inheritance has to be more efficient, 
    /// because it uses internal collection buffers directly.
    /// </summary>
    /// <remarks>
    /// Uses strong references in its internal work.
    /// </remarks>
    public class StrongReferenceObservable<TItem> : ShrinkableCollectionBase<IObserver<TItem>>, IEventSource<TItem>
    {
        private Object _lock;

        public StrongReferenceObservable()
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

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseOnNext{TItem}(object, IObserver{TItem}[], bool[], TItem)"/>
        public void RaiseNext(TItem value)
        {
            InlineRaiseHelper.InlineRaiseOnNext(_lock, ElementsBuffer, IsAliveBuffer, value);
        }

        /// <inheritdoc cref="InlineRaiseHelper.InlineRaiseCompleted{TItem}(IObserver{TItem}[], bool[])"/>
        public void RaiseCompleted()
        {
            InlineRaiseHelper.InlineRaiseCompleted(ElementsBuffer, IsAliveBuffer);

            InitializeDefaultMembers();
        }
    }
}
