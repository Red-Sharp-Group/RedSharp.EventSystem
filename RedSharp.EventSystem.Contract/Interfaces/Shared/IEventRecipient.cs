using RedSharp.EventSystem.Interfaces.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces.Shared
{
    public interface IEventRecipient<TEventSource, TDelegate> where TEventSource : class where TDelegate : Delegate
    {
        void Initialize(TEventSource source, TDelegate subscriber);
    }
}
