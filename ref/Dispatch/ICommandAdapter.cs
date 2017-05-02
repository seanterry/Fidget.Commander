using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Defines an adapter for a command handler.
    /// </summary>
    /// <typeparam name="TResult">Type of the command result.</typeparam>

    public interface ICommandAdapter<TResult>
    {
        /// <summary>
        /// Translates the given command to its concrete type and invokes its execution.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The command result.</returns>
        
        Task<TResult> Execute( ICommand<TResult> command, CancellationToken cancellationToken );
    }

    /// <summary>
    /// Defines an adapter for a command handler.
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of the command.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>
    
    public interface ICommandAdapter<in TCommand, TResult> : ICommandAdapter<TResult> where TCommand : ICommand<TResult> {}
}