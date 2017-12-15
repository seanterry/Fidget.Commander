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

namespace Fidget.Commander.Internal
{
    /// <summary>
    /// Factory for generating command adapters.
    /// </summary>
    /// <remarks>
    /// This is the stock factory that can work with dependency injection containers
    /// that can utilize IServiceProvider.
    /// </remarks>

    public class CommandAdapterFactory : ICommandAdapterFactory
    {
        /// <summary>
        /// Dependency resolver.
        /// </summary>

        readonly IServiceProvider resolver;

        /// <summary>
        /// Constructs a factory for generating command adapters.
        /// </summary>
        /// <param name="resolver">Depdendency resolver.</param>

        public CommandAdapterFactory( IServiceProvider resolver )
        {
            this.resolver = resolver ?? throw new ArgumentNullException( nameof( resolver ) );
        }

        /// <summary>
        /// Returns the appropriate adapter for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose adapter to return.</param>

        public ICommandAdapter CreateFor( ICommand command )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            var commandType = command.GetType();
            var adapterType = typeof( CommandAdapter<> ).MakeGenericType( commandType );

            return (ICommandAdapter)resolver
                .GetService( adapterType ) ?? throw new InvalidOperationException( $"Unable to create an adapter for {adapterType}" );
        }

        /// <summary>
        /// Returns the appropriate adapter for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose adapter to return.</param>

        public ICommandAdapter<TResult> CreateFor<TResult>( ICommand<TResult> command )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            var commandType = command.GetType();
            var resultType = typeof( TResult );
            var adapterType = typeof( CommandAdapter<,> ).MakeGenericType( commandType, resultType );

            return (ICommandAdapter<TResult>)resolver
                .GetService( adapterType ) ?? throw new InvalidOperationException( $"Unable to create an adapter for {adapterType}" );
        }
    }
}