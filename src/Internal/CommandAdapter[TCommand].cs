/*  Copyright 2017 Sean Terry

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Internal
{
    /// <summary>
    /// Adapter for executing a command.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to execute.</typeparam>

    public class CommandAdapter<TCommand> : ICommandAdapter where TCommand : ICommand
    {
        /// <summary>
        /// Handler that executes the command.
        /// </summary>
        
        readonly ICommandHandler<TCommand> handler;

        /// <summary>
        /// Decorators for modifying command behavior.
        /// </summary>

        readonly IEnumerable<ICommandDecorator<TCommand>> decorators;

        /// <summary>
        /// Constructs an adapter for executing a command.
        /// </summary>
        /// <param name="handler">Handler that executes the command.</param>
        /// <param name="decorators">Decorators for modifying command behavior.</param>
        
        public CommandAdapter( ICommandHandler<TCommand> handler, IEnumerable<ICommandDecorator<TCommand>> decorators )
        {
            this.handler = handler ?? throw new ArgumentNullException( nameof(handler) );
            this.decorators = decorators ?? throw new ArgumentNullException( nameof(decorators) );
        }

        /// <summary>
        /// Translates the given command to its concrete type and invokes its execution.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        
        public async Task Execute( ICommand command, CancellationToken cancellationToken )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );
            if ( !( command is TCommand concrete ) ) throw new ArgumentException( $"Expected command of type {typeof( TCommand )}; Received {command.GetType()}", nameof( command ) );

            cancellationToken.ThrowIfCancellationRequested();

            var handle = new CommandDelegate<TCommand>( handler.Handle );

            if ( decorators.Any() )
            {
                handle = decorators
                    .Reverse()
                    .Aggregate( handle, ( continuation, decorator ) => ( cmd, token ) => decorator.Execute( cmd, token, continuation ) );
            }

            await handle.Invoke( concrete, cancellationToken );
        }
    }
}