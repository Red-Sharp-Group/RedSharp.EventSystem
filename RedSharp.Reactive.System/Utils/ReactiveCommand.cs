using System;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Sys.Interfaces.Entities;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Sys.Utils
{
    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with no parameters that works by delegates.
    /// </remarks>
    public class ReactiveCommand : ReactiveCommandBase
    {
        private Action _execute;
        private Func<bool> _check;

        public ReactiveCommand(Action execute, Func<bool> check = null)
        {
            ArgumentsGuard.ThrowIfNull(execute);

            _execute = execute;
            _check = check;
        }

        protected override void InternalExecute() =>  _execute.Invoke();

        protected override bool InternalCheck() => _check?.Invoke() ?? true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with a single parameter that works by delegates.
    /// </remarks>
    public class ReactiveCommand<T1> : ReactiveCommandBase, IReactiveCommand<T1>
    {
        private Action<T1> _execute;
        private Func<T1, bool> _check;

        private T1 _firstArgument;

        public ReactiveCommand(Action<T1> execute, Func<T1, bool> check = null)
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

        protected override void InternalExecute() => _execute.Invoke(FirstArgument);

        protected override bool InternalCheck() => _check?.Invoke(FirstArgument) ?? true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with two parameters that works by delegates.
    /// </remarks>
    public class ReactiveCommand<T1, T2> : ReactiveCommandBase, IReactiveCommand<T1, T2>
    {
        private Action<T1, T2> _execute;
        private Func<T1, T2, bool> _check;

        private T1 _firstArgument;
        private T2 _secondArgument;

        public ReactiveCommand(Action<T1, T2> execute, Func<T1, T2, bool> check = null)
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

        protected override void InternalExecute() => _execute.Invoke(FirstArgument, SecondArgument);

        protected override bool InternalCheck() =>  _check?.Invoke(FirstArgument, SecondArgument) ?? true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Basic implementation with three parameters that works by delegates.
    /// </remarks>
    public class ReactiveCommand<T1, T2, T3> : ReactiveCommandBase, IReactiveCommand<T1, T2, T3>
    {
        private Action<T1, T2, T3> _execute;
        private Func<T1, T2, T3, bool> _check;

        private T1 _firstArgument;
        private T2 _secondArgument;
        private T3 _thirdArgument;

        public ReactiveCommand(Action<T1, T2, T3> execute, Func<T1, T2, T3, bool> check = null)
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

        protected override void InternalExecute() => _execute.Invoke(FirstArgument, SecondArgument, ThirdArgument);

        protected override bool InternalCheck() =>  _check?.Invoke(FirstArgument, SecondArgument, ThirdArgument) ?? true;
    }
}
