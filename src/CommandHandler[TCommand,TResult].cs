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
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander
{
    /// <summary>
    /// Convenience type for implementing a command handler.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to execute.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>

    public abstract class CommandHandler<TCommand,TResult> : ICommandHandler<TCommand,TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Executes the command when overridden in a derived type.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        
        protected abstract Task<TResult> Execute( TCommand command, CancellationToken cancellationToken );

        /// <summary>
        /// Handles execution of the command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        
        public async Task<TResult> Handle( TCommand command, CancellationToken cancellationToken )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            cancellationToken.ThrowIfCancellationRequested();
            return await Execute( command, cancellationToken );
        }
    }
}