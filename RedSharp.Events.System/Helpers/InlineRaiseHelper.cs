using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.Events.Sys.Helpers
{
    internal static class InlineRaiseHelper
    {
        /// <summary>
        /// Raises each observer in the try...catch... statement.
        /// If an observer throws an exception will invoke <see cref="IObserver{T}.OnError(Exception)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="IObserver{T}.OnError(Exception)"/> also is invoked in try catch
        /// but what I have to do in the catch case I don`t know.
        /// <br/>Uses lock statement to prevent using it from several threads simultaneously, 
        /// so not invoke unsubscribing during this process.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseOnNext<TItem>(Object lockObject, ICollection<IObserver<TItem>> collection, TItem value)
        {
            lock (lockObject)
            {
                foreach (var item in collection)
                {
                    try
                    {
                        item.OnNext(value);
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        Trace.WriteLine(exception.StackTrace);

                        try
                        {
                            item.OnError(exception);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        /// <summary>
        /// Raises completed for each observer in the try...catch... statement.
        /// </summary>
        /// <remarks>
        /// Does not use lock statement to prevent selflock
        /// Makes a copy of collection to prevent error during foreach statement.
        /// Clears the input collection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseCompleted<TItem>(ICollection<IObserver<TItem>> collection)
        {
            //Usually people do unsusbcribing during this process
            //to prevent exception I do copy
            var copiedSubscribers = collection.ToArray();

            foreach (var item in copiedSubscribers)
            {
                try
                {
                    item.OnCompleted();
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    Trace.WriteLine(exception.StackTrace);
                }
            }

            //for people who do not do unsusbcribing
            collection.Clear();
        }

        /// <summary>
        /// Raises each observer in the try...catch... statement.
        /// If an observer throws an exception will invoke <see cref="IObserver{T}.OnError(Exception)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="IObserver{T}.OnError(Exception)"/> also is invoked in try catch
        /// but what I have to do in the catch case I don`t know.
        /// <br/>Uses lock statement to prevent using it from several threads simultaneously, 
        /// so not invoke unsubscribing during this process.
        /// Has to be the most efficient way to raise this method, because doesn't use <see cref="IEnumerable{T}"/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseOnNext<TItem>(Object lockObject, IObserver<TItem>[] buffer, bool[] isAlive, TItem value)
        {
            lock (lockObject)
            {
                var index = 0;

                while (index < buffer.Length)
                {
                    try
                    {
                        for (; index < buffer.Length; index++)
                            if (isAlive[index])
                                buffer[index].OnNext(value);
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        Trace.WriteLine(exception.StackTrace);

                        try
                        {
                            buffer[index].OnError(exception);
                        }
                        catch
                        { }

                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Raises completed for each observer in the try...catch... statement.
        /// </summary>
        /// <remarks>
        /// Does not use lock statement to prevent selflock
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseCompleted<TItem>(IObserver<TItem>[] buffer, bool[] isAlive)
        {
            var index = 0;

            while (index < buffer.Length)
            {
                try
                {
                    for (; index < buffer.Length; index++)
                        if (isAlive[index])
                            buffer[index].OnCompleted();
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    Trace.WriteLine(exception.StackTrace);

                    index++;
                }
            }
        }

        /// <summary>
        /// Raises each observer in the try...catch... statement.
        /// If an observer throws an exception will invoke <see cref="IObserver{T}.OnError(Exception)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="IObserver{T}.OnError(Exception)"/> also is invoked in try catch
        /// but what I have to do in the catch case I don`t know.
        /// <br/>Uses lock statement to prevent using it from several threads simultaneously, 
        /// so not invoke unsubscribing during this process.
        /// Has to be the most efficient way to raise this method, because doesn't use <see cref="IEnumerable{T}"/>
        /// If finds dead object will mark it as ... dead object.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseOnNext<TItem>(Object lockObject, WeakReference<IObserver<TItem>>[] buffer, bool[] isAlive, TItem value)
        {
            lock (lockObject)
            {
                var index = 0;

                IObserver<TItem> temp = null;

                while (index < buffer.Length)
                {
                    try
                    {
                        for (; index < buffer.Length; index++)
                        {
                            if (isAlive[index])
                            {
                                if (buffer[index].TryGetTarget(out temp))
                                    temp.OnNext(value);
                                else
                                    isAlive[index] = false;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        Trace.WriteLine(exception.StackTrace);

                        try
                        {
                            temp?.OnError(exception);
                        }
                        catch
                        { }

                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Raises completed for each observer in the try...catch... statement.
        /// </summary>
        /// <remarks>
        /// Does not use lock statement to prevent selflock
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineRaiseCompleted<TItem>(WeakReference<IObserver<TItem>>[] buffer, bool[] isAlive)
        {
            var index = 0;

            IObserver<TItem> temp = null;

            while (index < buffer.Length)
            {
                try
                {
                    for (; index < buffer.Length; index++)
                    {
                        if (isAlive[index])
                        {
                            if (buffer[index].TryGetTarget(out temp))
                                temp.OnCompleted();
                            else
                                isAlive[index] = false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    Trace.WriteLine(exception.StackTrace);

                    index++;
                }
            }
        }
    }
}
