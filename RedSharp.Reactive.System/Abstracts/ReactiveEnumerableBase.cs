using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Sys.Interfaces.Entities;

namespace RedSharp.Reactive.Sys.Abstracts
{
    /// <summary>
    /// Abstract class that contains everything that will be needed to make a collection support "reactivity".
    /// </summary>
    public abstract class ReactiveEnumerableBase<TItem> : ReactiveEntityBase, IReactiveEnumerable<TItem>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        //This is the only case when I actually want this thing to be implemented in the interface by default
        IEnumerator IEnumerable.GetEnumerator() =>  GetEnumerator();

        /// <inheritdoc/> 
        public abstract IEnumerator<TItem> GetEnumerator();

        /// <summary>
        /// Invokes a <see cref="CollectionChanged"/>.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs arguments)
        {
            try
            {
                CollectionChanged?.Invoke(this, arguments);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Has to be invoked on single item adding, possible with index (if it is an inserting).
        /// </summary>
        protected void RaiseAdding(TItem item, int? index = null)
        {
            if(index.HasValue)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index.Value));
            else
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Using for range adding.
        /// </summary>
        /// <remarks>
        /// Currently I don't know a collection that can add a range by specific index, so I do not use it for this case.
        /// </remarks>
        protected void RaiseAdding(IEnumerable<TItem> items)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        /// <summary>
        /// Has to be invoked or set by indexer, better with index.
        /// </summary>
        protected void RaiseReplacing(TItem oldItem, TItem newItem, int? index = null)
        {
            if (index.HasValue)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index.Value));
            else
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));
        }

        /// <summary>
        /// Has to be invoked on single item removing, possible with index (if it is a removing by index).
        /// </summary>
        protected void RaiseRemoving(TItem item, int? index = null)
        {
            if (index.HasValue)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index.Value));
            else
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        /// <summary>
        /// Using for range removing.
        /// </summary>
        /// <remarks>
        /// Currently I don't know a collection that can remove a range by specific index, so I do not use it for this case.
        /// </remarks>
        protected void RaiseRemoving(IEnumerable<TItem> items)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
        }

        /// <summary>
        /// Used for as Microsoft documentation says "dramatic changing" of the collection as clearing f.e.
        /// </summary>
        protected void RaiseClearing()
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
