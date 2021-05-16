using RedSharp.EventSystem.Abstracts;
using RedSharp.EventSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem
{
    internal class WeakRefEventListener<TArgument> : ListenerBase<IWrEvent<TArgument>, Action<TArgument>>, IWrListener<TArgument>
    {
        public void Initialize(IWrEvent<TArgument> source, Action<TArgument> subscriber)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("EventListener");

            if (InternalTarget != null)
                throw new Exception("Object is already initialized.");

            InternalTarget = source ?? throw new ArgumentNullException(nameof(source));
            InternalDelegate = subscriber ?? throw new ArgumentNullException(nameof(subscriber));

            source.Subscribe(subscriber);
        }

        protected override void Unsubscribe(IWrEvent<TArgument> target)
        {
            target.Unsubscribe(InternalDelegate);
        }
    }
}
