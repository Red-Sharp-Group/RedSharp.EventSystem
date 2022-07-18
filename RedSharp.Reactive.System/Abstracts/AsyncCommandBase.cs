using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Abstracts
{
    /// <summary>
    /// Default implementation of the <see cref="IAsyncCommand"/>
    /// </summary>
    public abstract class AsyncCommandBase : ReactiveEntityBase, IAsyncCommand
    {
        //Firstly I wanted to extend the ReactiveCommandBase but this command is far more complicated.
                
        /// <summary>
        /// Special command that is used as a <see cref="CancellationTokenSource"/> keeper.
        /// </summary>
        private class InternalCancelCommand : ReactiveCommandBase
        {
            private AsyncCommandBase _owner;

            public InternalCancelCommand(AsyncCommandBase owner)
            {
                _owner = owner;

                TokenSource = new CancellationTokenSource();
            }

            /// <summary>
            /// Yep, this token source is used for the cancellation process.
            /// <br/>Currently, I assume that every async command can be canceled.
            /// </summary>
            public CancellationTokenSource TokenSource { get; private set; }

            /// <summary>
            /// Recreates a <see cref="TokenSource"/>, 
            /// simply because this is the only way it can be reset properly.
            /// </summary>
            public void Reset()
            {
                if (!TokenSource.IsCancellationRequested)
                    return;

                TokenSource.Dispose();
                TokenSource = new CancellationTokenSource();
            }

            /// <summary>
            /// Is allowed to execute only when the command-owner 
            /// is executing and cancellation hasn't invoked yet.
            /// </summary>
            protected override bool InternalCheck() => _owner.InExecution && !TokenSource.IsCancellationRequested;

            /// <summary>
            /// Performs cancel on the <see cref="TokenSource"/>
            /// </summary>
            protected override void InternalExecute() => TokenSource.Cancel();
        }

        private bool _inExecution;
        private bool _canExecute;

        private InternalCancelCommand _cancelCommand;

        public AsyncCommandBase()
        {
            _cancelCommand = new InternalCancelCommand(this);
        }

        public event EventHandler CanExecuteChanged;

        /// <inheritdoc/>
        public IReactiveCommand CancelCommand => _cancelCommand;

        /// <inheritdoc/>
        public bool CanExecute
        {
            get => _canExecute;
            set
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
                    _cancelCommand.ForseCheck();
            }
        }

        /// <summary>
        /// Bridge to the <see cref="ICommand"/> object,
        /// translates the call to the <see cref="Execute"/> method, the parameter will be ignored.
        /// <br/>According to the interface standard it calls the async version of the <see cref="Execute"/> 
        /// operation without waiting of the end of it.
        /// </summary>
        void ICommand.Execute(object parameter) => ExecuteAsync();

        /// <summary>
        /// Synchronous executing of the command.
        /// </summary>
        /// <remarks>
        /// Will be invoked in the try {..} catch {..} statement.
        /// <br/>Forces check twice, so I do not recommend to put something heavy to the <see cref="ForseCheck"/> method.
        /// </remarks>
        public virtual void Execute()
        {
            if (!ForseCheck())
                return;

            InExecution = true;

            try
            {
                StartExecution();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            InExecution = false;

            ForseCheck();

            _cancelCommand.ForseCheck();
        }

        /// <summary>
        /// Asynchronous executing of the command.
        /// </summary>
        /// <remarks>
        /// Will be invoked in the try {..} catch {..} statement.
        /// <br/>Forces check twice, so I do not recommend to put something heavy to the <see cref="ForseCheck"/> method.
        /// </remarks>
        public virtual async Task ExecuteAsync()
        {
            if (!ForseCheck())
                return;

            InExecution = true;

            try
            {
                await Task.Factory.StartNew(StartExecution);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }

            InExecution = false;

            ForseCheck();

            _cancelCommand.ForseCheck();
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
        /// Wrapper, to not make an anonymous delegate every time.
        /// </summary>
        private void StartExecution() => InternalExecute(_cancelCommand.TokenSource.Token);

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
        /// <remarks>
        /// This variant has a <see cref="CancellationToken"/> that has to be used 
        /// if you want to make your command actually cancelable.
        /// </remarks>
        protected abstract void InternalExecute(CancellationToken token);

        /// <summary>
        /// Has to be implemented to make it possible to allow or disallow of the command execution.
        /// </summary>
        protected abstract bool InternalCheck();
    }
}
