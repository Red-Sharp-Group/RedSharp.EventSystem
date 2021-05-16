using RedSharp.EventSystem.Abstract;
using RedSharp.EventSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.EventSystem
{
    public class WeakRefEvent<TArgument> : WeakEventDelegatesStorage<Action<TArgument>>, IWrEvent<TArgument>
    {
        private Object _lock;

        public WeakRefEvent(String eventName)
        {
            if (String.IsNullOrWhiteSpace(eventName))
                throw new ArgumentOutOfRangeException(nameof(eventName));

            Name = eventName;

            _lock = new Object();
        }

        public String Name { get; private set; }

        public void Subscribe(Action<TArgument> subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            lock (_lock)
                Add(subscriber);
        }

        public void Unsubscribe(Action<TArgument> subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            lock (_lock)
                Remove(subscriber);
        }

        public void Invoke(TArgument argument)
        {
            lock (_lock)
            {
                List<Exception> exceptions = null;
                Action<TArgument> tempDelegate = null;

                var buffer = Buffer;
                var isAlive = IsAlive;
                var index = 0;

                while (index < buffer.Length)
                {
                    try
                    {
                        for (; index < buffer.Length; index++)
                        {
                            if (!isAlive[index])
                                continue;

                            if (buffer[index].TryGetTarget(out tempDelegate)) 
                                tempDelegate.Invoke(argument);
                            else
                                isAlive[index] = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exceptions == null)
                            exceptions = new List<Exception>();

                        exceptions.Add(exception);

                        index++;
                    }
                }

                if (NeedToDecreaseBuffer())
                    TryDecreaseBuffer();

                if (exceptions != null)
                    throw new AggregateException(exceptions);                
            }
        }
    }
}
