using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Utils;
using RedSharp.Sys.Collections;

namespace RedSharp.Events.Sys.Abstracts
{
    /// <summary>
    /// <inheritdoc cref="StrongReferenceObservableDisposableBase{TItem}"/>
    /// </summary>
    /// <remarks>
    /// Uses weak references.
    /// </remarks>
    public abstract class WeakReferenceObservableDisposableBase<TItem> : ObservableDisposableBase<TItem>
    {
        public WeakReferenceObservableDisposableBase() : base(new WeakShrinkableCollection<IObserver<TItem>>())
        { }

        protected override IDisposable CreateListenerForObserver(IObserver<TItem> observer)
        {
            return new WeakReferenceListener<TItem>(this, observer);
        }
    }
}
