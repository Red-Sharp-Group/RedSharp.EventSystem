using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Abstracts
{
    /// <summary>
    /// Default implementation of <see cref="IReactiveEntity"/>
    /// </summary>
    public class ReactiveEntityBase : IReactiveEntity
    {
        private bool _hasErros;
        private Lazy<Dictionary<String, HashSet<String>>> _errors;
        private Lazy<Dictionary<String, Dictionary<String, Delegate>>> _rules;

        public ReactiveEntityBase()
        {
            _errors = new Lazy<Dictionary<String, HashSet<String>>>();
            _rules = new Lazy<Dictionary<String, Dictionary<String, Delegate>>>();
        }

        /// <summary>
        /// Occurs on the beginning of property changing right before it is actually changed.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Returns true if any of properties has a validation error.
        /// </summary>
        public bool HasErrors
        {
            get => _hasErros;
            set => SetAndRaise(ref _hasErros, value);
        }
        
        /// <inheritdoc cref="GetErrors(string)"/>
        IEnumerable INotifyDataErrorInfo.GetErrors(String propertyName) => GetErrors(propertyName);

        /// <summary>
        /// Returns a list of errors identifiers for the input property name.
        /// <br/>You can use this list to find and show the correspond error messages to a user.
        /// </summary>
        /// <exception cref="ArgumentException">If the property name if null or empty.</exception>
        public IEnumerable<String> GetErrors(String propertyName)
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(propertyName);

            if (_errors.Value.TryGetValue(propertyName, out HashSet<String> result))
                return result;
            else
                return Array.Empty<String>();
        }

        /// <summary>
        /// The generic method that raises all needed events, to not double the code.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue<TItem>(ref TItem field, TItem value, String name)
        {
            RaisePropertyChanging(name);

            field = value;

            RaisePropertyChanged(name);

            ValidateProperty(field, name);
        }

        /// <summary>
        /// Invokes a <see cref="PropertyChanging"/> for the input property name.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// </remarks>
        protected void RaisePropertyChanging([CallerMemberName] String name = null)
        {
            if (PropertyChanging == null)
                return;

            var eventArguments = new PropertyChangingEventArgs(name);

            try
            {
                PropertyChanging.Invoke(this, eventArguments);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Invokes a <see cref="PropertyChanged"/> for the input property name.
        /// </summary>
        /// <remarks>
        /// Raises in the try {..} catch {..} statements.
        /// </remarks>
        protected void RaisePropertyChanged([CallerMemberName] String name = null)
        {
            if (PropertyChanged == null)
                return;

            var eventArguments = new PropertyChangedEventArgs(name);

            try
            {
                PropertyChanged.Invoke(this, eventArguments);
            }
            catch(Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Validates the input value by rules that must be filled by a user.
        /// <br/>All validation infrastructure has a <see cref="Lazy{T}"/> initialization, 
        /// so if the user hasn't filled rules dictionary invocation of this method will be the cheapest as possible.
        /// <br/>Invokes a <see cref="ErrorsChanged"/> for the input property name.
        /// </summary>
        /// <remarks>
        /// Raises the event in the try {..} catch {..} statements.
        /// </remarks>
        protected void ValidateProperty<TItem>(TItem value, [CallerMemberName] String name = null)
        {
            Dictionary<String, Delegate> propertyRules = null;

            if (!_rules.IsValueCreated || !_rules.Value.TryGetValue(name, out propertyRules))
                return;

            bool hadErrors = false;

            HashSet<String> propertyErrors = null;

            if (_errors.IsValueCreated && _errors.Value.TryGetValue(name, out propertyErrors))
                hadErrors = true;

            bool wasChanged = false;

            foreach (var item in propertyRules)
            {
                //I assume that the user will not set the rule that can throw an exception,
                //or if it does it will be a user problem ¯\_(ツ)_/¯
                if (!((Predicate<TItem>)item.Value).Invoke(value))
                {
                    if (propertyErrors == null)
                        propertyErrors = new HashSet<String>();

                    wasChanged |= propertyErrors.Add(item.Key);
                }
                else if (hadErrors)
                {
                    wasChanged |= propertyErrors.Remove(item.Key);
                }
            }

            if (!hadErrors && propertyErrors != null)
            {
                _errors.Value[name] = propertyErrors;
            }
            else if (hadErrors && propertyErrors.Count == 0)
            {
                _errors.Value.Remove(name);
            }

            HasErrors = _errors.IsValueCreated && _errors.Value.Count > 0;

            if (ErrorsChanged != null && wasChanged)
            {
                var eventArguments = new DataErrorsChangedEventArgs(name);

                try
                {
                    ErrorsChanged.Invoke(this, eventArguments);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception.Message);
                    Trace.WriteLine(exception.StackTrace);
                }
            }
        }

        /// <summary>
        /// Generic method for lazy people (like me) that will check equality 
        /// of the given field and value and will raise all needed events by it self.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetAndRaise<TItem>(ref TItem field, TItem value, [CallerMemberName] String name = null)
        {
            if (EqualityComparer<TItem>.Default.Equals(field, value))
                return false;

            SetValue(ref field, value, name);

            return true;
        }

        /// <summary>
        /// More specific implementation of the <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> 
        /// for the <see cref="float"/> values, it can also receive a precision value for more accurate checking.
        /// <br/> As it is for <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> this method will 
        /// check equality of the given field and value and will raise all needed events by it self.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetAndRaise(ref float field, float value, float precision, [CallerMemberName] String name = null)
        {
            if (Math.Abs(field - value) < precision)
                return false;

            SetValue(ref field, value, name);

            return true;
        }

        /// <summary>
        /// More specific implementation of the <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> 
        /// for the <see cref="double"/> values, it can also receive a precision value for more accurate checking.
        /// <br/> As it is for <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> this method will 
        /// check equality of the given field and value and will raise all needed events by it self.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetAndRaise(ref double field, double value, double precision, [CallerMemberName] String name = null)
        {
            if (Math.Abs(field - value) < precision)
                return false;

            SetValue(ref field, value, name);

            return true;
        }
        
        /// <summary>
        /// More specific implementation of the <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> 
        /// for the <see cref="String"/> values (you can pass a <see cref="StringComparison"/> to make a different comparison).
        /// <br/> As it is for <see cref="SetAndRaise{TItem}(ref TItem, TItem, string)"/> this method will 
        /// check equality of the given field and value and will raise all needed events by it self.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetAndRaise(ref String field, String value, StringComparison comparison, [CallerMemberName] String name = null)
        {
            if (String.Equals(field, value, comparison))
                return false;

            SetValue(ref field, value, name);

            return true;
        }

        /// <summary>
        /// The method that will create a validation rule for the property name.
        /// </summary>
        /// <remarks>
        /// This method will create a rules dictionary and other validation stuff, so you literally open a Pandora box.
        /// <br/>Currently you cannot remove a rule but you can change it by passing another predicate for the same name and identifier.
        /// <br/>The identifier it is a value that you will receive by <see cref="GetErrors(string)"/> for the property 
        /// (you can pass an actual error message but I don't recommend to do this, because the identifier is used as a key).
        /// </remarks>
        protected void AssignValidationRule<TItem>(String name, String identifier, Predicate<TItem> predicate)
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            ArgumentsGuard.ThrowIfNullOrEmpty(identifier);
            ArgumentsGuard.ThrowIfNull(predicate);

            if (!_rules.Value.TryGetValue(name, out Dictionary<string, Delegate> propertyRules))
            {
                propertyRules = new Dictionary<string, Delegate>();

                _rules.Value[name] = propertyRules;
            }

            propertyRules[identifier] = predicate;
        }
    }
}
