using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces
{
    public static class WrListenerHelper
    {
        public static IWrListener<TArgument> Listen<TArgument>(this IWrEvent<TArgument> eventSource, Action<TArgument> action)
        {
            var listener = new WeakRefEventListener<TArgument>();

            listener.Initialize(eventSource, action);

            return listener;
        }
    }
}
