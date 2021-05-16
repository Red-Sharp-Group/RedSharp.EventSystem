using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces.Shared
{
    public interface IEventSource<TDelegate> where TDelegate : Delegate
    {
        void Subscribe(TDelegate subscriber);

        void Unsubscribe(TDelegate subscriber);
    }
}
