using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Defines a decorator for modifying the behavior of a command handler.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command whose handler to decorate.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>

    public interface ICommandDecorator<TCommand,TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Executes the decorator.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="continuation">Delegate that represents the next stage of command execution.</param>
        /// <returns>The command result.</returns>
        
        Task<TResult> Execute( TCommand command, CancellationToken cancellationToken, CommandDelegate<TCommand,TResult> continuation );
    }
}