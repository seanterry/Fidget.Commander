using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Adapter for a command handler.
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of the command to handle.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>
    
    public class CommandAdapter<TCommand,TResult> : ICommandAdapter<TCommand,TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Handler that executes the command.
        /// </summary>
        
        readonly ICommandHandler<TCommand,TResult> Handler;

        /// <summary>
        /// Collection of decorators for modifying command execution.
        /// </summary>
        
        readonly IEnumerable<ICommandDecorator<TCommand,TResult>> Decorators;

        /// <summary>
        /// Constructs an adapter for a command handler.
        /// </summary>
        /// <param name="handler">Handler that executes the command.</param>
        /// <param name="decorators">Decorators for modifying command behavior.</param>

        public CommandAdapter( ICommandHandler<TCommand,TResult> handler, IEnumerable<ICommandDecorator<TCommand,TResult>> decorators ) 
        {
            Handler = handler ?? throw new ArgumentNullException( nameof(handler) );
            Decorators = decorators ?? throw new ArgumentNullException( nameof(decorators) );
        }

        /// <summary>
        /// Translates the given command to its concrete type and invokes its execution.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The command result.</returns>

        public async Task<TResult> Execute( ICommand<TResult> command, CancellationToken cancellationToken )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            if ( command is TCommand concrete )
            {
                cancellationToken.ThrowIfCancellationRequested();

                var handle = new CommandDelegate<TCommand, TResult>( Handler.Handle );

                if ( Decorators.Any() )
                {
                    handle = Decorators
                        .Reverse()
                        .Aggregate( handle, ( continuation, decorator ) => ( cmd, token ) => decorator.Execute( cmd, token, continuation ) );
                }
                
                return await handle.Invoke( concrete, cancellationToken );
            }

            throw new ArgumentException( $"Expected command of type {typeof(TCommand)}; Received {command.GetType()}", nameof(command) );
        }
    }
}