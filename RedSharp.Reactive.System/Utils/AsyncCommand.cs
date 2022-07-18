using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Utils
{
    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with no parameters that works by delegates.
    /// </remarks>
    public class AsyncCommand : AsyncCommandBase
    {
        private Action<CancellationToken> _execute;
        private Func<bool> _check;

        public AsyncCommand(Action<CancellationToken> execute, Func<bool> check = null)
        {
            ArgumentsGuard.ThrowIfNull(execute);

            _execute = execute;
            _check = check;
        }

        protected override void InternalExecute(CancellationToken token)
        {
            _execute.Invoke(token);
        }

        protected override bool InternalCheck()
        {
            return _check?.Invoke() ?? true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with a single parameter that works by delegates.
    /// </remarks>
    public class AsyncCommand<T1> : AsyncCommandBase, IAsyncCommand<T1>
    {
        private Action<T1, CancellationToken> _execute;
        private Func<T1, bool> _check;

        private T1 _firstArgument;

        public AsyncCommand(Action<T1, CancellationToken> execute, Func<T1, bool> check = null)
        {
            ArgumentsGuard.ThrowIfNull(execute);

            _execute = execute;
            _check = check;
        }

        public T1 FirstArgument
        {
            get => _firstArgument;
            set
            {
                if (SetAndRaise(ref _firstArgument, value))
                    ForseCheck();
            }
        }

        protected override void InternalExecute(CancellationToken token)
        {
            _execute.Invoke(FirstArgument, token);
        }

        protected override bool InternalCheck()
        {
            return _check?.Invoke(FirstArgument) ?? true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with two parameters that works by delegates.
    /// </remarks>
    public class AsyncCommand<T1, T2> : AsyncCommandBase, IAsyncCommand<T1, T2>
    {
        private Action<T1, T2, CancellationToken> _execute;
        private Func<T1, T2, bool> _check;

        private T1 _firstArgument;
        private T2 _secondArgument;

        public AsyncCommand(Action<T1, T2, CancellationToken> execute, Func<T1, T2, bool> check = null)
        {
            ArgumentsGuard.ThrowIfNull(execute);

            _execute = execute;
            _check = check;
        }

        public T1 FirstArgument
        {
            get => _firstArgument;
            set
            {
                if (SetAndRaise(ref _firstArgument, value))
                    ForseCheck();
            }
        }

        public T2 SecondArgument
        {
            get => _secondArgument;
            set
            {
                if (SetAndRaise(ref _secondArgument, value))
                    ForseCheck();
            }
        }

        protected override void InternalExecute(CancellationToken token)
        {
            _execute.Invoke(FirstArgument, SecondArgument, token);
        }

        protected override bool InternalCheck()
        {
            return _check?.Invoke(FirstArgument, SecondArgument) ?? true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with three parameters that works by delegates.
    /// </remarks>
    public class AsyncCommand<T1, T2, T3> : AsyncCommandBase, IAsyncCommand<T1, T2, T3>
    {
        private Action<T1, T2, T3, CancellationToken> _execute;
        private Func<T1, T2, T3, bool> _check;

        private T1 _firstArgument;
        private T2 _secondArgument;
        private T3 _thirdArgument;

        public AsyncCommand(Action<T1, T2, T3, CancellationToken> execute, Func<T1, T2, T3, bool> check = null)
        {
            ArgumentsGuard.ThrowIfNull(execute);

            _execute = execute;
            _check = check;
        }

        public T1 FirstArgument
        {
            get => _firstArgument;
            set
            {
                if (SetAndRaise(ref _firstArgument, value))
                    ForseCheck();
            }
        }

        public T2 SecondArgument
        {
            get => _secondArgument;
            set
            {
                if (SetAndRaise(ref _secondArgument, value))
                    ForseCheck();
            }
        }

        public T3 ThirdArgument
        {
            get => _thirdArgument;
            set
            {
                if (SetAndRaise(ref _thirdArgument, value))
                    ForseCheck();
            }
        }

        protected override void InternalExecute(CancellationToken token)
        {
            _execute.Invoke(FirstArgument, SecondArgument, ThirdArgument, token);
        }

        protected override bool InternalCheck()
        {
            return _check?.Invoke(FirstArgument, SecondArgument, ThirdArgument) ?? true;
        }
    }
}
