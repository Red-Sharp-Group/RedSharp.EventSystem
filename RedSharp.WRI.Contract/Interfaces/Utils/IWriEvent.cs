using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// The main object of whole assembly.
    /// Allows you to subscribe and unsubscribe without memory leaks.
    /// Just dispose your listener or/and leave it, that's all.
    /// </summary>
    /// <typeparam name="TModel">Arguments.</typeparam>
    public interface IWriEvent<TModel> : IWriEventGeneral
    {
        /// <summary>
        /// Allows to subscribe by special object-listener.
        /// </summary>
        /// <remarks>
        /// Must be thread safe.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Listener was null.
        /// </exception>
        void Subscribe(IWriListener<TModel> listener);

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
        void Unsubscribe(IWriListener<TModel> listener);
    }
}
