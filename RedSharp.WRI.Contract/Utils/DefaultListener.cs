using RedSharp.WRI.Interfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Utils
{
    /// <inheritdoc/>
    internal class DefaultListener<TModel> : IWriListener<TModel>
    {
        private Action<TModel> _eventHandler;
        private Action<Exception> _exeptionHandler;

        public DefaultListener(Action<TModel> eventHandler, Action<Exception> exeptionHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            _exeptionHandler = exeptionHandler;
        }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public void ReceiveEvent(TModel model)
        {
            if (IsDisposed || !IsEnabled)
                return;

            _eventHandler.Invoke(model);
        }

        /// <inheritdoc/>
        public void ReceiveException(Exception exception)
        {
            if (IsDisposed || !IsEnabled || _exeptionHandler == null)
                return;

            _exeptionHandler.Invoke(exception);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
