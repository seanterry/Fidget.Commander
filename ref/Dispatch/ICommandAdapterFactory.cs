namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Defines a factory for generating command adapters.
    /// </summary>
    
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
