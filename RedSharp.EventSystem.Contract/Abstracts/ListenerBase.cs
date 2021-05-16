using RedSharp.EventSystem.Interfaces.General;
using RedSharp.EventSystem.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Abstracts
{
    public abstract class ListenerBase<TEventSource, TDelegate> : IListener where TEventSource : class where TDelegate : Delegate
    {
        private WeakReference<TEventSource> _target;

        public Object Target => InternalTarget;

        public Delegate Delegate => InternalDelegate;

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            InternalDispose(true);
        }

        protected TEventSource InternalTarget
        {
            get
            {
                TEventSource result = null;

                _target?.TryGetTarget(out result);

                return result;
            }
            set
            {
                if (value == null)
                    _target = null;
                else if (_target == null)
                    _target = new WeakReference<TEventSource>(value);
                else
                    _target.SetTarget(value);
            }
        }

        protected TDelegate InternalDelegate { get; set; }

        protected abstract void Unsubscribe(TEventSource target);

        protected virtual void InternalDispose(bool manualDispose)
        {
            var target = InternalTarget;

            if (target != null)
                Unsubscribe(target);

            InternalDelegate = null;
            InternalTarget = null;
        }
    }
}
