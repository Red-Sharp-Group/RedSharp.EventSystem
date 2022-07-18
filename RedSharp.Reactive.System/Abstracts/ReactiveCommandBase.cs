using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using RedSharp.Reactive.Sys.Interfaces.Entities;

namespace RedSharp.Reactive.Sys.Abstracts
{
    /// <summary>
    /// Default implementation of the <see cref="IReactiveCommand"/>
    /// </summary>
    public abstract class ReactiveCommandBase : ReactiveEntityBase, IReactiveCommand
    {
        private bool _inExecution;
        private bool _canExecute;

        public event EventHandler CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute
        {
            get => _canExecute;
            private set
            {
                if (SetAndRaise(ref _canExecute, value)) 
                    RaiseCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public bool InExecution
        {
            get => _inExecution;
            private set
            {
                if (SetAndRaise(ref _inExecution, value))
                    RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Bridge to the <see cref="ICommand"/> object,
        /// translates the call to the <see cref="Execute"/> method, the parameter will be ignored.
        /// </summary>
        void ICommand.Execute(object parameter) => Execute();

        /// <summary>
        /// Actual executing of the command.
        /// </summary>
        /// <remarks>
        /// Has to be sync. Will be invoked in the try {..} catch {..} statement.
        /// <br/>Forces check twice, so I do not recommend to put something heavy to the <see cref="ForseCheck"/> method.
        /// </remarks>
        public virtual void Execute()
        {
            if (!ForseCheck())
                return;

            InExecution = true;

            try
            {
                InternalExecute();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            InExecution = false;

            ForseCheck();
        }

        /// <summary>
        /// Bridge to the <see cref="ICommand"/> object, 
        /// translates the call to the <see cref="ForseCheck"/> method, the parameter will be ignored.
        /// </summary>
        bool ICommand.CanExecute(object parameter) => ForseCheck();

        /// <summary>
        /// Forces check operation for the command.
        /// </summary>
        /// <remarks>
        /// Will be invoked in the try {..} catch {..} statement.
        /// </remarks>
        public virtual bool ForseCheck()
        {
            try
            {
                CanExecute = !InExecution && InternalCheck();
            }
            catch (Exception exception)
            {
                CanExecute = false;

                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            return CanExecute;
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> in the try {..} catch {..} statement.
        /// </summary>
        private void RaiseCanExecuteChanged()
        {
            try
            {
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Has to be implemented for the actual command execution.
        /// </summary>
        protected abstract void InternalExecute();

        /// <summary>
        /// Has to be implemented to make it possible to allow or disallow of the command execution.
        /// </summary>
        protected abstract bool InternalCheck();
    }
}
