# Fidget.Commander

This is a simple dispatcher for executing commands in a CQRS-ish manner. In Fidget, however, everything is a command. In my day job as both a database administrator and API developer, I've found that commands frequently end up needing to return a value, and queries frequently require auditing.

Consequently, Fidget has a unified interface for defining commands, handlers, and decorators.

## Basics
Once you've added the `Fidget.Commander` package to your project, you'll want to familiarize yourself with the following interfaces:
- `ICommand<TResult>` is a marker interface for the command itself.
- `ICommandHandler<TCommand,TResult>` defines a handler for executing the command.
- `ICommandDecorator<TCommand,TResult>` allows you to modify a handler's behavior without modifying the handler itself.
- `ICommandDispatcher` is implemented for you, and true to its name, it dispatches commands to be executed by their handlers.

As of version 3, commands that return no result do not need a TResult argument.

To get started, you'll need to tell your favorite DI container how to find the implementations of those interfaces.

### Microsoft.Extensions.DependencyInjection + Scrutor
Here's a simple registration example using the standard dependency injection container in ASP.NET Core. It is augmented using Scrutor to scan the assembly for handlers and decorators that we would otherwise have to register individually.

```csharp
    public void ConfigureServices( IServiceCollection services )
    {
        // register the commander
        services.AddFidgetCommander()
            // note: this example uses Scrutor for assembly scanning
            .Scan( scanner => scanner.FromAssemblyOf<Startup>()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandHandler<,> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandHandler<> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandDecorator<,> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandDecorator<> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );
                
        services.AddMvc();
    }
```

### StructureMap.AspNetCore
Here's a registration example using StructureMap.AspNetCore:
```csharp
    public void ConfigureServices( IServiceCollection services )
    {
        services.AddFidgetCommander();
        services.AddMvc();
    }

    public void ConfigureContainer( Registry registry )
    {
        // add all your handlers and decorators
        registry.Scan( scanner =>
        {
            scanner.AssemblyContainingType<Startup>();
            scanner.ConnectImplementationsToTypesClosing( typeof( ICommandHandler<,> ) );
            scanner.ConnectImplementationsToTypesClosing( typeof( ICommandHandler<> ) );
            scanner.ConnectImplementationsToTypesClosing( typeof( ICommandDecorator<,> ) );
            scanner.ConnectImplementationsToTypesClosing( typeof( ICommandDecorator<> ) );
        });
    }
```

`CommandAdapterFactory` uses the `IServiceProvider` interface that most of the common DI containers support. For those that don't, a simple wrapper or a custom implementation of `ICommandAdapterFactory` can be used.

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
