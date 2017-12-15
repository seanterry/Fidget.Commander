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

using Fidget.Commander.Internal;
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

        public class TestVoidCommand : ICommand {}

        public class TestVoidHandler : ICommandHandler<TestVoidCommand>
        {
            public Task Handle( TestVoidCommand command, CancellationToken cancellationToken ) => Task.CompletedTask;
        }

        public class TestResultCommand : ICommand<object> {}

        public class TestResultHandler : ICommandHandler<TestResultCommand,object>
        {
            public object Result { get; set; }
            public Task<object> Handle( TestResultCommand command, CancellationToken cancellationToken ) => Task.FromResult( Result );
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
        public void registers_void_adapter()
        {
            // add handlers from assembly
            var services = invoke().Scan( scanner => scanner.FromAssemblyOf<TestVoidCommand>()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandHandler<> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses( _ => _.AssignableTo( typeof( ICommandDecorator<> ) ) )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );

            var factory = services
                .BuildServiceProvider()
                .GetRequiredService<ICommandAdapterFactory>();

            var command = new TestVoidCommand();
            var actual = factory.CreateFor( command );

            Assert.IsType<CommandAdapter<TestVoidCommand>>( actual );
        }

        [Fact]
        public void registers_result_adapter()
        {
            // add handlers from assembly
            var services = invoke().Scan( scanner => scanner.FromAssemblyOf<TestResultCommand>()
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

            var command = new TestResultCommand();
            var actual = factory.CreateFor( command );

            Assert.IsType<CommandAdapter<TestResultCommand,object>>( actual );
        }
    }
}