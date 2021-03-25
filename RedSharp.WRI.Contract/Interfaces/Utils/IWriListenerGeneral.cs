using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// General event functionality, with this interface you can keep the listener in a collection.
    /// </summary>
    public interface IWriListenerGeneral : IDisposable
    {
        /// <summary>
        /// Allows to temporary off event receiving.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// If it's true, object won't receive event messages.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Unexpected behavior of event raising.
        /// </summary>
        /// <remarks>
        /// According to standard <see cref="IWriEvent{TModel}"/> must not fail in any case, 
        /// so if raising is fail <see cref="IWriEvent{TModel}"/> invokes this to message user about problems.
        /// </remarks>
        void ReceiveException(Exception exception);
    }
}
