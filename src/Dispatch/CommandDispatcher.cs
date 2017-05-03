using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Dispatcher for resolving and executing the handlers for commands.
    /// </summary>

    public class CommandDispatcher : ICommandDispatcher
    {
        /// <summary>
        /// Command adapter factory.
        /// </summary>
        
        readonly ICommandAdapterFactory Factory;

        /// <summary>
        /// Constructs a dispatcher for resolving and executing the handlers for commands.
        /// </summary>
        /// <param name="factory">Command adapter factory.</param>
        
        public CommandDispatcher( ICommandAdapterFactory factory )
        {
            Factory = factory ?? throw new ArgumentNullException( nameof(factory) );
        }

        /// <summary>
        /// Locates and executes the handler for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose handler to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The command result.</returns>

        public async Task<TResult> Execute<TResult>( ICommand<TResult> command, CancellationToken cancellationToken )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            cancellationToken.ThrowIfCancellationRequested();

            var adapter = Factory.For( command ) ?? 
                throw new InvalidOperationException( $"No adapter returned for the command {command.GetType()}" );

            return await adapter.Execute( command, cancellationToken );
        }
    }
}