using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.Events.Sys.Interfaces
{
    /// <summary>
    /// Extension for the existing interface, just to support unsubscribing as a standard.
    /// </summary>
    public interface IEventSource<TItem> : IObservable<TItem>
    {
        /// <summary>
        /// Performs the unsubscribe process. Mostly used for internal needs.
        /// I do not recommend to use it explicitly, you have <see cref="IDisposable"/> object for it.
        /// </summary>
        bool Unsubscribe(IObserver<TItem> observer);
    }
}
