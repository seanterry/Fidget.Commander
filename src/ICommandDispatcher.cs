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