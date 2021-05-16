using RedSharp.EventSystem.Abstracts;
using RedSharp.EventSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem
{
    internal class StrongRefEventListener<TArgument> : ListenerBase<ISrEvent<TArgument>, Action<TArgument>>, ISrListener<TArgument>
    {
        ~StrongRefEventListener()
        {
            InternalDispose(false);
        }

        public void Initialize(ISrEvent<TArgument> source, Action<TArgument> subscriber)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("EventListener");

            if (InternalTarget != null)
                throw new Exception("Object is already initialized.");

            InternalTarget = source ?? throw new ArgumentNullException(nameof(source));
            InternalDelegate = subscriber ?? throw new ArgumentNullException(nameof(subscriber));

            source.Subscribe(subscriber);
        }

        protected override void Unsubscribe(ISrEvent<TArgument> target)
        {
            target.Unsubscribe(InternalDelegate);
        }

        protected override void InternalDispose(bool manualDispose)
        {
            base.InternalDispose(manualDispose);

            if (manualDispose)
                GC.SuppressFinalize(this);
        }
    }
}
