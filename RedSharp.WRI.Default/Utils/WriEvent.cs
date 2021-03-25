using RedSharp.WRI.Interfaces.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedSharp.WRI.Utils
{
    /// <summary>
    /// Built-in, common realization of weak referenced events.
    /// </summary>
    public class WriEvent<TModel> : IWriEvent<TModel>
    {
        #region Members
        //========================================//
        //Members

        private Type _internalType = null;
        private WriLinkedList<IWriListener<TModel>> _listeners;
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

            _listeners = new WriLinkedList<IWriListener<TModel>>();
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
        public void Subscribe(IWriListener<TModel> listener)
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
        public void Unsubscribe(IWriListener<TModel> listener)
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
        public void Invoke(TModel arguments)
        {
            foreach(var item in _listeners)
            {
                try
                {
                    item.ReceiveEvent(arguments);
                }
                catch(Exception exception)
                {
                    try
                    {
                        item.ReceiveException(exception);
                    }
                    catch
                    {
                        Trace.WriteLine("Event caught exception from receiving exception to listener.");
                    }
                }
            }
        }

        #endregion
    }
}
