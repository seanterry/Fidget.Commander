namespace Fidget.Commander
{
    /// <summary>
    /// Defines a command whose execution returns a result.
    /// </summary>
    /// <typeparam name="TResult">Type of the command result.</typeparam>
    /// <remarks>This is a marker interface.</remarks>

    public interface ICommand<out TResult> {}

    /// <summary>
    /// Defines a command whose execution returns no result.
    /// </summary>
    /// <remarks>This is a marker interface.</remarks>
    
    public interface ICommand : ICommand<Unit> {}
}