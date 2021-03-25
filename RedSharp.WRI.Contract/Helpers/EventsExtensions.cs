using RedSharp.WRI.Interfaces.Utils;
using RedSharp.WRI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Helpers
{
    public static class EventsExtensions
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
        /// <exception cref="NullReferenceException">
        /// Event was null.
        /// </exception>
        public static IWriListener<TModel> Subscribe<TModel>(this IWriEvent<TModel> wriEvent, Action<TModel> eventHandler, Action<Exception> exceptionHandler = null)
        {
            if (wriEvent == null)
                throw new NullReferenceException("You tried to use this method with invalid event.");

            var listener = new DefaultListener<TModel>(eventHandler, exceptionHandler);

            wriEvent.Subscribe(listener);

            return listener;
        }
    }
}
