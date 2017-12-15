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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Commander.Internal
{
    public class CommandAdapter_TCommand_TResult_Tests
    {
        public class TestCommand : ICommand<object> {}

        Mock<ICommandHandler<TestCommand,object>> mockHandler = new Mock<ICommandHandler<TestCommand,object>>();

        ICommandHandler<TestCommand,object> handler => mockHandler?.Object;
        List<ICommandDecorator<TestCommand,object>> decorators = new List<ICommandDecorator<TestCommand,object>>();
        ICommandAdapter<object> instance => new CommandAdapter<TestCommand,object>( handler, decorators );

        public class Constructor : CommandAdapter_TCommand_TResult_Tests
        {
            [Fact]
            public void requires_handler()
            {
                mockHandler = null;
                Assert.Throws<ArgumentNullException>( nameof( handler ), () => instance );
            }

            [Fact]
            public void requires_decorators()
            {
                decorators = null;
                Assert.Throws<ArgumentNullException>( nameof( decorators ), () => instance );
            }
        }

        public class Execute : CommandAdapter_TCommand_TResult_Tests
        {
            ICommand<object> command = new TestCommand();
            CancellationToken cancellationToken = CancellationToken.None;

            async Task<object> invoke() => await instance.Execute( command, cancellationToken );

            [Fact]
            public async Task requires_command()
            {
                command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( command ), invoke );
            }

            class WrongCommand : ICommand<object> {}

            [Fact]
            public async Task requires_correct_command_type()
            {
                command = new WrongCommand();
                await Assert.ThrowsAsync<ArgumentException>( nameof( command ), invoke );
            }

            [Fact]
            public async Task aborts_when_requested()
            {
                cancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( invoke );

                mockHandler.Verify( _ => _.Handle( (TestCommand)command, cancellationToken ), Times.Never );
            }

            // easier than mock setup
            class FakeDecorator : ICommandDecorator<TestCommand,object>
            {
                public Action<TestCommand> callback;

                public async Task<object> Execute( TestCommand command, CancellationToken cancellationToken, CommandDelegate<TestCommand,object> continuation )
                {
                    callback.Invoke( command );
                    return await continuation.Invoke( command, cancellationToken );
                }
            }

            [Theory]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( 3 )]
            public async Task calls_handler_and_decorators_returns_result( int expectedDecorators )
            {
                var actualDecorators = 0;

                for ( var i = 0; i < expectedDecorators; i++ )
                {
                    decorators.Add( new FakeDecorator
                    {
                        callback = actualCommand =>
                        {
                            Assert.Equal( command, actualCommand );
                            actualDecorators++;
                        }
                    } );
                }

                var expected = new object();
                mockHandler.Setup( _=> _.Handle( (TestCommand)command, cancellationToken ) ).ReturnsAsync( expected );
                var actual = await invoke();

                Assert.Equal( expected, actual );
                Assert.Equal( expectedDecorators, actualDecorators );
                mockHandler.Verify( _ => _.Handle( (TestCommand)command, cancellationToken ), Times.Once );
            }
        }
    }
}
