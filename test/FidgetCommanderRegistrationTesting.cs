using Fidget.Commander.Dispatch;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Commander
{
    /// <summary>
    /// Tests of dependency injection registration.
    /// </summary>

    public class FidgetCommanderRegistrationTesting
    {
        IServiceCollection services = new ServiceCollection();
        IServiceCollection invoke() => services.AddFidgetCommander();
        
        [Fact]
        public void requires_services()
        {
            services = null;
            Assert.Throws<ArgumentNullException>( nameof(services), ()=> invoke() );
        }

        public class TestCommand : ICommand<Unit> {}

        public class TestHandler : ICommandHandler<TestCommand,Unit>
        {
            public Task<Unit> Handle( TestCommand command, CancellationToken cancellationToken ) => Task.FromResult( Unit.Default );
        }

        [Fact]
        public void registers_dispatcher()
        {
            var actual = invoke()
                .BuildServiceProvider()
                .GetRequiredService<ICommandDispatcher>();

            Assert.IsType<CommandDispatcher>( actual );
        }

        [Fact]
        public void registers_adapterFactory()
        {
            var actual = invoke()
                .BuildServiceProvider()
                .GetRequiredService<ICommandAdapterFactory>();

            Assert.IsType<CommandAdapterFactory>( actual );
        }

        [Fact]
        public void registers_adapter()
        {
            // add handlers from assembly
            var services = invoke().Scan( scanner => scanner.FromAssemblyOf<TestCommand>()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandHandler<,> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandDecorator<,> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime() 
            );

            var factory = services
                .BuildServiceProvider()
                .GetRequiredService<ICommandAdapterFactory>();

            var command = new TestCommand();
            var actual = factory.For( command );

            Assert.IsType<CommandAdapter<TestCommand,Unit>>( actual );
        }
    }
}