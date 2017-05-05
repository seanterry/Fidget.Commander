# Fidget.Commander

This is a simple dispatcher for executing commands in a CQRS-ish manner. In Fidget, however, everything is a command. In my day job as both a database administrator and API developer, I've found that commands frequently end up needing to return a value, and queries frequently require auditing.

Consequently, Fidged has a unified interface for defining a commands, handlers, and decorators.

## Basics
Once you've added the `Fidget.Commander` package to your project, you'll want to familiarize yourself with the following interfaces:
- `ICommand<TResult>` is a marker interface for the command itself.
- `ICommandHandler<TCommand,TResult>` defines a handler for executing the command.
- `ICommandDecorator<TCommand,TResult>` allows you to modify a handler's behavior without modifying the handler itself.
- `ICommandDispatcher` is implemented for you, and true to its name, it dispatches commands to be executed by their handlers.

To get started, you'll need to tell your favorite DI container how to find the implementations of those interfaces.

### StructureMap
Here's a simple registration example using `StructureMap.AspNetCore`, which is available in the code samples.
```csharp
    public class Startup
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>

        public static void Main( string[] args )
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseStructureMap()
                .Build();

            host.Run();
        }

        /// <summary>
        /// Configures application services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        /// <summary>
        /// Configures application dependency injection.
        /// </summary>
        /// <param name="registry">StructureMap registry.</param>

        public void ConfigureContainer( Registry registry )
        {
            // add all your handlers and decorators
            registry.Scan( scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.ConnectImplementationsToTypesClosing( typeof( ICommandHandler<,> ) );
                scanner.ConnectImplementationsToTypesClosing( typeof( ICommandDecorator<,> ) );
            });
            
            // add the types that Fidget needs
            registry.For( typeof(ICommandAdapter<,>) ).Use( typeof(CommandAdapter<,>) );
            registry.For<ICommandAdapterFactory>().Use<CommandAdapterFactory>();
            registry.For<ICommandDispatcher>().Use<CommandDispatcher>();
        }

        /// <summary>
        /// Configures the HTTP pipeline.
        /// </summary>

        public void Configure( IApplicationBuilder app )
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
```

`CommandAdapterFactory' uses the `IServiceProvider` interface that most of the common DI containers. For those that don't, a simple wrapper or a custom implementation of `ICommandAdapterFactory` can be used.

### Usage
Here's a simple controller, command, handler, and decorator example. It returns a hello or goodbye greeting based on HTTP method, and a decorator to add some attitude:
```csharp
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
```