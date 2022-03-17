using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Events.Sys.Interfaces;
using RedSharp.Sys.Helpers;
using RedSharp.Sys.Interfaces.Shared;

namespace RedSharp.Events.Sys.Utils
{
    /// <summary>
    /// Listener for weak ref observables.
    /// <br/>Does not contain finalizer.
    /// </summary>
    public class WeakReferenceListener<TItem> : IDisposable, IDisposeIndication
    {
        private WeakReference<IEventSource<TItem>> _ownerWeakRef;
        private IObserver<TItem> _observer;

        public WeakReferenceListener(IEventSource<TItem> owner, IObserver<TItem> observer)
        {
            ArgumentsGuard.ThrowIfNull(owner);
            ArgumentsGuard.ThrowIfNull(observer);

            _ownerWeakRef = new WeakReference<IEventSource<TItem>>(owner);
            _observer = observer;
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            try
            {
                if (_ownerWeakRef.TryGetTarget(out IEventSource<TItem> owner))
                    owner.Unsubscribe(_observer);

                _ownerWeakRef = null;
                _observer = null;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            IsDisposed = true;
        }
    }
}
