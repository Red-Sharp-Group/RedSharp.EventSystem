using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces
{
    public static class SrListenerHelper
    {
        public static ISrListener<TArgument> Listen<TArgument>(this ISrEvent<TArgument> eventSource, Action<TArgument> action)
        {
            var listener = new StrongRefEventListener<TArgument>();

            listener.Initialize(eventSource, action);

            return listener;
        }
    }
}
