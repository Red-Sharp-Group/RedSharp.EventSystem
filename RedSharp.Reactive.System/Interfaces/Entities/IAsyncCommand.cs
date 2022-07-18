using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// <inheritdoc cref="IReactiveCommand"/>
    /// <br/>This is a variant of <see cref="IReactiveCommand"/> 
    /// where <see cref="ICommand.Execute(object?)"/> is performed in async way.
    /// <br/>Also it has a several "regular" methods, ususal <see cref="IReactiveCommand.Execute"/> 
    /// will execute command in a sync way and <see cref="IAsyncCommand.ExecuteAsync"/> in async way 
    /// with possibility to wait it by using a result <see cref="Task"/>.
    /// </summary>
    public interface IAsyncCommand : IReactiveCommand
    {
        /// <summary>
        /// Special command that activates an internal <see cref="CancellationTokenSource"/>
        /// in a goal to stop command executing.
        /// </summary>
        IReactiveCommand CancelCommand { get; }

        /// <summary>
        /// Executes command asynchronous.
        /// </summary>
        Task ExecuteAsync();
    }

    /// <inheritdoc cref="IAsyncCommand"/>
    /// <remarks>
    /// The variant with the only one argument.
    /// </remarks>
    public interface IAsyncCommand<TArgument1> : IAsyncCommand
    {
        TArgument1 FirstArgument { get; set; }
    }

    /// <inheritdoc cref="IAsyncCommand"/>
    /// <remarks>
    /// The variant with two arguments.
    /// </remarks>
    public interface IAsyncCommand<TArgument1, TArgument2> : IAsyncCommand<TArgument1>
    {
        TArgument2 SecondArgument { get; set; }
    }

    /// <inheritdoc cref="IAsyncCommand"/>
    /// <remarks>
    /// The variant with three arguments.
    /// </remarks>
    public interface IAsyncCommand<TArgument1, TArgument2, TArgument3> : IAsyncCommand<TArgument1, TArgument2>
    {
        TArgument3 ThirdArgument { get; set; }
    }
}
