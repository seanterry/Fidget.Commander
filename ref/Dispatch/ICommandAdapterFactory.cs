namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Factory for generating command adapters.
    /// </summary>
    /// <remarks>This is an extensibility point for dependency injection.</remarks>

    public interface ICommandAdapterFactory
    {
        /// <summary>
        /// Returns the appropriate adapter for the given command.
        /// </summary>
        /// <typeparam name="TResult">Type of the command result.</typeparam>
        /// <param name="command">Command whose adapter to return.</param>
        
        ICommandAdapter<TResult> For<TResult>( ICommand<TResult> command );
    }
}
