using RedSharp.EventSystem.Interfaces.General;
using RedSharp.EventSystem.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces
{
    public interface IWrEvent<TArgument> : IEvent, IEventSource<Action<TArgument>>
    { }
}
