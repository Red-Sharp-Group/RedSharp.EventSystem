using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem.Interfaces.General
{
    public interface IListener : IDisposable
    { 
        Object Target { get; }

        Delegate Delegate { get; }

        bool IsDisposed { get; }
    }
}
