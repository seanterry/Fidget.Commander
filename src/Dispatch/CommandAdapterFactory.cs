using System;
using System.Collections.Generic;
using System.Text;

namespace Fidget.Commander.Dispatch
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
        
        readonly IServiceProvider Resolver;

        /// <summary>
        /// Constructs a factory for generating command adapters.
        /// </summary>
        /// <param name="resolver">Depdendency resolver.</param>
        
        public CommandAdapterFactory( IServiceProvider resolver )
        {
            Resolver = resolver ?? throw new ArgumentNullException( nameof(resolver) );
        }

        /// <summary>
        /// Returns the appropriate adapter for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose adapter to return.</param>

        public ICommandAdapter<TResult> For<TResult>( ICommand<TResult> command )
        {
            if ( command == null ) throw new ArgumentNullException( nameof( command ) );

            var commandType = command.GetType();
            var resultType = typeof(TResult);
            var adapterType = typeof(ICommandAdapter<,>).MakeGenericType( commandType, resultType );

            return (ICommandAdapter<TResult>) Resolver
                .GetService( adapterType ) ?? throw new InvalidOperationException( $"Unable to create an adapter for {adapterType}" );
        }
    }
}