using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedSharp.Reactive.Bindings.Abstracts;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Entities
{
    /// <summary>
    /// The node that can observer <see cref="INotifyPropertyChanged"/> object.
    /// </summary>
    /// <remarks>
    /// Property value is not cached.
    /// </remarks>
    public class NotifyPropertyChangedBindingNode<TInput, TOutput> : BindingNodeBase<TInput, TOutput> where TInput : INotifyPropertyChanged
    {
        private Func<TInput, TOutput> _getter;
        private Action<TInput, TOutput> _setter;
        private string _name;

        public NotifyPropertyChangedBindingNode(string name, Func<TInput, TOutput> getter, Action<TInput, TOutput> setter = null)
        {
            ArgumentsGuard.ThrowIfNull(getter);
            ArgumentsGuard.ThrowIfNullOrEmpty(name);

            _name = name;
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Is read-only if there is not setter delegate and there is no object to apply this delegate to.
        /// </summary>
        public override bool IsReadOnly => _setter == null || CachedInputValue == null;

        public override TOutput Value
        {
            get
            {
                if (CachedInputValue == null)
                    return default;
                else
                    return _getter.Invoke(CachedInputValue);
            }
            set
            {
                ThrowIfReadOnly();

                _setter.Invoke(CachedInputValue, value);
            }
        }

        public override object Clone()
        {
            return new NotifyPropertyChangedBindingNode<TInput, TOutput>(_name, _getter, _setter);
        }

        /// <summary>
        /// Makes the following node update if property changed.
        /// </summary>
        private void ReactDataContextPropertyChanged(object sender, PropertyChangedEventArgs arguments)
        {
            if(String.Equals(arguments.PropertyName, _name))
                Following?.Update();
        }

        /// <summary>
        /// Reassign the event subscribing.
        /// </summary>
        protected override void OnInputUpdate(TInput previous, TInput current)
        {
            if (previous != null)
                previous.PropertyChanged -= ReactDataContextPropertyChanged;

            if (current != null)
                current.PropertyChanged += ReactDataContextPropertyChanged;

            Following?.Update();
        }

        protected override void InternalDispose(bool manual)
        {
            if (CachedInputValue != null)
                CachedInputValue.PropertyChanged -= ReactDataContextPropertyChanged;

            base.InternalDispose(manual);
        }
    }
}
