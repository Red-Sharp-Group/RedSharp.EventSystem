using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// In most cases this guy will be got from event by handler subscription.
    /// </summary>
    public interface IWriPersonalListener : IWriListener, IDisposable
    {
        /// <summary>
        /// Allows to temporary off event receiving.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// If it's true, object won't receive event messages.
        /// </summary>
        bool IsDisposed { get; }
    }
}
