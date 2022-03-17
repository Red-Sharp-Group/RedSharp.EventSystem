using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Abstracts;
using RedSharp.Sys.Helpers;

namespace RedSharp.Events.Sys.Utils
{
    /// <summary>
    /// Listener for strong ref observables.
    /// <br/>Will unsubscribe on finalize.
    /// </summary>
    public class StrongReferenceListener<TItem> : DisposableBase
    {
        private WeakReference<IEventSource<TItem>> _ownerWeakRef;
        private IObserver<TItem> _observer;

        public StrongReferenceListener(IEventSource<TItem> owner, IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(owner);
            ArgumentsGuard.ThrowIfNull(observer);

            _ownerWeakRef = new WeakReference<IEventSource<TItem>>(owner);
            _observer = observer;
        }

        protected override void InternalDispose(bool manual)
        {
            if (_ownerWeakRef.TryGetTarget(out IEventSource<TItem> owner))
                owner.Unsubscribe(_observer);

            _ownerWeakRef = null;
            _observer = null;

            base.InternalDispose(manual);
        }
    }
}
