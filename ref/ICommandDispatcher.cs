using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander
{
    /// <summary>
    /// Defines a dispatcher for resolving and executing the handlers for commands.
    /// </summary>

    public interface ICommandDispatcher
    {
        /// <summary>
        /// Locates and executes the handler for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose handler to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The command result.</returns>
        
        Task<TResult> Execute<TResult>( ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken) );
    }
}