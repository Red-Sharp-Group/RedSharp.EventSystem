using System;
using RedSharp.Events.Sys.Utils;
using RedSharp.Sys.Utils;

namespace RedSharp.Events.Sys.Abstracts
{
    /// <summary>
    /// <inheritdoc cref="StrongReferenceObservableBase{TItem}"/>
    /// </summary>
    /// <remarks>
    /// Uses weak references.
    /// </remarks>
    public abstract class WeakReferenceObservableBase<TItem> : ObservableBase<TItem>
    {
        public WeakReferenceObservableBase() : base(new WeakShrinkableCollection<IObserver<TItem>>())
        { }

        protected override IDisposable CreateListenerForObserver(IObserver<TItem> observer)
        {
            return new WeakReferenceListener<TItem>(this, observer);
        }
    }
}
