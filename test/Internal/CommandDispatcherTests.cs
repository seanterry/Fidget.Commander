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

using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Commander.Internal
{
    public class CommandDispatcherTests
    {
        Mock<ICommandAdapterFactory> mockFactory = new Mock<ICommandAdapterFactory>();

        ICommandAdapterFactory factory => mockFactory?.Object;
        ICommandDispatcher instance => new CommandDispatcher( factory );

        public class Constructor : CommandDispatcherTests
        {
            [Fact]
            public void requires_factory()
            {
                mockFactory = null;
                Assert.Throws<ArgumentNullException>( nameof(factory), ()=>instance );
            }
        }

        public class Execute : CommandDispatcherTests
        {
            public class TestCommand : ICommand {}

            ICommand command = new TestCommand();
            CancellationToken cancellationToken = CancellationToken.None;

            async Task invoke() => await instance.Execute( command, cancellationToken );

            [Fact]
            public async Task requires_command()
            {
                command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(command), ()=>invoke() );
            }

            [Fact]
            public async Task cancels_when_requested()
            {
                cancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( ()=>invoke() );
            }

            [Fact]
            public async Task executes_adapter()
            {
                var mockAdapter = new Mock<ICommandAdapter>();
                mockFactory.Setup( _=> _.CreateFor( command ) ).Returns( mockAdapter.Object );

                await invoke();
                
                mockFactory.Verify( _=> _.CreateFor( command ), Times.Once );
                mockAdapter.Verify( _=> _.Execute( command, cancellationToken ), Times.Once );
            }
        }

        public class Execute_TResult : CommandDispatcherTests
        {
            public class TestCommand : ICommand<object> { }

            ICommand<object> command = new TestCommand();
            CancellationToken cancellationToken = CancellationToken.None;

            async Task<object> invoke() => await instance.Execute( command, cancellationToken );

            [Fact]
            public async Task requires_command()
            {
                command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( command ), () => invoke() );
            }

            [Fact]
            public async Task cancels_when_requested()
            {
                cancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( () => invoke() );
            }

            [Fact]
            public async Task executes_adapter_returns_result()
            {
                var expected = new object();
                var mockAdapter = new Mock<ICommandAdapter<object>>();
                mockAdapter.Setup( _=> _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );
                mockFactory.Setup( _ => _.CreateFor( command ) ).Returns( mockAdapter.Object );

                var actual = await invoke();

                Assert.Equal( expected, actual );
                mockFactory.Verify( _ => _.CreateFor( command ), Times.Once );
                mockAdapter.Verify( _ => _.Execute( command, cancellationToken ), Times.Once );
            }
        }
    }
}