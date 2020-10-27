using RedSharp.WRI.Interfaces.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.WRI.Utils
{
    /// <summary>
    /// Built-in, common realization of weak referenced events.
    /// </summary>
    public class WriEvent<TModel> : IWriEvent<TModel>
    {
        #region Internal Types
        //========================================//
        //Internal Types

        /// <summary>
        /// Special wrapper is created for handler subscription.
        /// </summary>
        private class WriEventPersonalListener : IWriPersonalListener
        {
            public WriEventPersonalListener(Object action)
            {
                IsEnabled = true;
                IsDisposed = false;

                Action = action;
            }

            ///<inheritdoc/>
            public bool IsEnabled { get; set; }

            ///<inheritdoc/>
            public bool IsDisposed { get; private set; }

            /// <summary>
            /// Non casted action.
            /// </summary>
            public Object Action { get; private set; }

            /// <summary>
            /// Invoke to stop receiving.
            /// </summary>
            public void Dispose()
            {
                IsDisposed = true;
            }

            ///<inheritdoc/>
            public void Raise<TArguments>(TArguments model)
            {
                if (!IsEnabled || IsDisposed)
                    return;

                var castedAction = Action as Action<TArguments>;

                castedAction.Invoke(model);
            }
        }

        #endregion

        #region Members
        //========================================//
        //Members

        private Type _internalType = null;
        private WriLinkedList<IWriListener> _listeners;
        private Object _lockKey;

        #endregion

        #region Public
        //========================================//
        //Public

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If name is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If owner is null.
        /// </exception>
        public WriEvent(String name, Object owner)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            Name = name;
            Owner = owner;

            _listeners = new WriLinkedList<IWriListener>();
            _lockKey = new Object();
        }

        ///<inheritdoc/>
        public String Name { get; private set; }

        ///<inheritdoc/>
        public Object Owner { get; private set; }

        ///<inheritdoc/>
        /// <remarks>
        /// Lazy loading.
        /// </remarks>
        public Type Type
        {
            get
            {
                if (_internalType == null)
                    _internalType = typeof(TModel);

                return _internalType;
            }
        }

        ///<inheritdoc/>
        public IWriPersonalListener Register(Action<TModel> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var listener = new WriEventPersonalListener(handler);

            lock(_lockKey)
                _listeners.Add(listener);

            return listener;
        }

        ///<inheritdoc/>
        public void Subscribe(IWriListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            lock (_lockKey)
            {
                if (!_listeners.Contains(listener))
                    _listeners.Add(listener);
            }
        }

        ///<inheritdoc/>
        public void Unsubscribe(IWriListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            lock (_lockKey)
                _listeners.Remove(listener);
        }

        /// <summary>
        /// Allows to raise event. 
        /// Pay attention: this object doesn't check input arguments on null.
        /// </summary>
        public void Raise(TModel arguments)
        {
            foreach(var item in _listeners)
            {
                try
                {
                    item.Raise(arguments);
                }
                catch { }
            }
        }

        #endregion
    }
}
