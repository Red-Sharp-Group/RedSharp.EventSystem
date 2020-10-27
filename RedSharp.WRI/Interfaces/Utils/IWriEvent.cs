using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// The main object of whole assembly.
    /// Allows you to subscribe and unsubscribe without memory leaks.
    /// Just dispose your personal listener and leave it, that's all.
    /// </summary>
    /// <typeparam name="TModel">Arguments.</typeparam>
    public interface IWriEvent<TModel> : IWriEventDescriptor
    {
        /// <summary>
        /// Allows to subscribe by input handler. That's the most usual case of using.
        /// </summary>
        /// <remarks>
        /// Must be thread safe.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Handler was null.
        /// </exception>
        IWriPersonalListener Register(Action<TModel> handler);

        /// <summary>
        /// Allows to subscribe by special object-listener.
        /// </summary>
        /// <remarks>
        /// Must be thread safe.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Listener was null.
        /// </exception>
        void Subscribe(IWriListener listener);

        /// <summary>
        /// Allows to unsubscribe by special object-listener.
        /// Can work with <see cref="IWriPersonalListener"/> - but it won't be disposed.
        /// </summary>
        /// <remarks>
        /// Must be thread safe.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Listener was null.
        /// </exception>
        void Unsubscribe(IWriListener listener);
    }
}
