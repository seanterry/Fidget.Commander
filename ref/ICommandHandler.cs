using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander
{
    /// <summary>
    /// Defines a handler for executing a command.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>

    public interface ICommandHandler<in TCommand,TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Handles execution of the command and returns a result.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The command result.</returns>
        
        Task<TResult> Handle( TCommand command, CancellationToken cancellationToken );
    }

    /// <summary>
    /// Defines a handler for executing a command that returns no result.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand,Unit> where TCommand : ICommand {}
}