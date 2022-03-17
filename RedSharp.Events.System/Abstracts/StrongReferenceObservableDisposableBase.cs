using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Utils;
using RedSharp.Sys.Utils;

namespace RedSharp.Events.Sys.Abstracts
{
    /// <summary>
    /// Basic object for <see cref="IObservable{T}"/> pattern, in the case you want to use it in inheritance.
    /// Also implements IDisposable pattern.
    /// </summary>
    /// <remarks>
    /// Uses strong references.
    /// </remarks>
    public abstract class StrongReferenceObservableDisposableBase<TItem> : ObservableDisposableBase<TItem>
    {
        public StrongReferenceObservableDisposableBase() : base(new ShrinkableCollection<IObserver<TItem>>())
        { }

        protected override IDisposable CreateListenerForObserver(IObserver<TItem> observer)
        {
            return new StrongReferenceListener<TItem>(this, observer);
        }
    }
}
