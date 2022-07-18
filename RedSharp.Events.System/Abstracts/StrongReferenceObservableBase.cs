using System;
using RedSharp.Events.Sys.Utils;
using RedSharp.Sys.Collections;

namespace RedSharp.Events.Sys.Abstracts
{
    /// <summary>
    /// Basic object for <see cref="IObservable{T}"/> pattern, in the case you want to use it in inheritance.
    /// </summary>
    /// <remarks>
    /// Uses strong references.
    /// </remarks>
    public abstract class StrongReferenceObservableBase<TItem> : ObservableBase<TItem>
    {
        public StrongReferenceObservableBase() : base(new ShrinkableCollection<IObserver<TItem>>())
        { }

        protected override IDisposable CreateListenerForObserver(IObserver<TItem> observer)
        {
            return new StrongReferenceListener<TItem>(this, observer);
        }
    }
}
