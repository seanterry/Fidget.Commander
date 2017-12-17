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
    /// Convenience type for implementing a command decorator.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to decorate.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>

    public abstract class CommandDecorator<TCommand, TResult> : ICommandDecorator<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Performs command decoration.
        /// </summary>
        /// <param name="command">Command to decorate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="continuation">Delegate that represents the next stage of the command pipeline.</param>
        
        protected abstract Task<TResult> Decorate( TCommand command, CancellationToken cancellationToken, CommandDelegate<TCommand,TResult> continuation );

        /// <summary>
        /// Executes the command decoration.
        /// </summary>
        /// <param name="command">Command to decorate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="continuation">Delegate that represents the next stage of the command pipeline.</param>
        
        public async Task<TResult> Execute( TCommand command, CancellationToken cancellationToken, CommandDelegate<TCommand, TResult> continuation )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );
            if ( continuation == null ) throw new ArgumentNullException( nameof( continuation ) );

            cancellationToken.ThrowIfCancellationRequested();

            return await Decorate( command, cancellationToken, continuation );
        }
    }
}