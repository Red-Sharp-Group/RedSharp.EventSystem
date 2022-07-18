using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// This is a little bit another implementation of the <see cref="ICommand"/>
    /// <br/>First of all it is hide usual method with parameter, 
    /// plus this is a <see cref="IReactiveEntity"/>, so all properties can be observed.
    /// </summary>
    public interface IReactiveCommand : ICommand, IReactiveEntity
    {
        /// <summary>
        /// Shows that the command can be executed.
        /// </summary>
        /// <remarks>
        /// Will be became false if command is in executing process.
        /// </remarks>
        bool CanExecute { get; }

        /// <summary>
        /// Shows that the command currently in the process.
        /// </summary>
        bool InExecution { get; }

        /// <summary>
        /// Starts a command execution.
        /// </summary>
        void Execute();
    }

    /// <inheritdoc cref="IReactiveCommand"/>
    /// <remarks>
    /// The variant with the only one argument.
    /// </remarks>
    public interface IReactiveCommand<TArgument1> : IReactiveCommand
    {
        TArgument1 FirstArgument { get; set; }
    }

    /// <inheritdoc cref="IReactiveCommand"/>
    /// <remarks>
    /// The variant with two arguments.
    /// </remarks>
    public interface IReactiveCommand<TArgument1, TArgument2> : IReactiveCommand<TArgument1>
    {
        TArgument2 SecondArgument { get; set; }
    }

    /// <inheritdoc cref="IReactiveCommand"/>
    /// <remarks>
    /// The variant with three arguments.
    /// </remarks>
    public interface IReactiveCommand<TArgument1, TArgument2, TArgument3> : IReactiveCommand<TArgument1, TArgument2>
    {
        TArgument3 ThirdArgument { get; set; }
    }
}
