using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Defines a delegate type that represents command execution.
    /// </summary>
    /// <typeparam name="TCommand">Type of the executing command.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>
    /// <param name="command">Command being executed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The command result.</returns>

    public delegate Task<TResult> CommandDelegate<TCommand,TResult>( TCommand command, CancellationToken cancellationToken ) where TCommand : ICommand<TResult>;
}