using Fidget.Commander.Dispatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Commander.Samples.StructureMap
{
    /// <summary>
    /// Controller that says hello.
    /// </summary>

    [Route( "" )]
    public class HelloController : ControllerBase
    {
        /// <summary>
        /// Fidget command dispatcher.
        /// </summary>

        readonly ICommandDispatcher Dispatcher;

        /// <summary>
        /// Constructs a controller that says hello.
        /// </summary>
        /// <param name="dispatcher">Fidget command dispatcher.</param>

        public HelloController( ICommandDispatcher dispatcher )
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException( nameof( dispatcher ) );
        }

        /// <summary>
        /// Command for creating a greeting.
        /// </summary>
        public class GreetCommand : ICommand<string> 
        {
            /// <summary>
            /// Gets or sets the name to include in the greeting.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the method used to invoke the command.
            /// </summary>
            public string Method { get; set; }
        }

        /// <summary>
        /// Handles the greet command.
        /// </summary>
        public class GreetCommandHandler : ICommandHandler<GreetCommand,string>
        {
            public Task<string> Handle( GreetCommand command, CancellationToken cancellationToken )
            {
                var greeting = "delete".Equals( command.Method, StringComparison.OrdinalIgnoreCase )
                    ? "Goodbye"
                    : "Hello";

                var name = string.IsNullOrWhiteSpace( command.Name )
                    ? "World"
                    : command.Name;
                
                return Task.FromResult( $"{greeting}, {name}!" );
            }
        }

        /// <summary>
        /// Decorates the greet command.
        /// </summary>
        public class GreetCommandDecorator : ICommandDecorator<GreetCommand, string>
        {
            public async Task<string> Execute( GreetCommand command, CancellationToken cancellationToken, CommandDelegate<GreetCommand, string> continuation ) =>
                $"I've been commanded to say '{await continuation( command, cancellationToken )}'!";
        }

        /// <summary>
        /// Says hello.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="name">Optional name to include in the message.</param>

        [HttpGet]
        [HttpDelete]
        public async Task<IActionResult> Greet( CancellationToken cancellationToken, string name = null )
        {
            var command = new GreetCommand
            {
                Name = name,
                Method = HttpContext.Request.Method,
            };

            return Ok( await Dispatcher.Execute( command, cancellationToken ) );
        }
    }
}