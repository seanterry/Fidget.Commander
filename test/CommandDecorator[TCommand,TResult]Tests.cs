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

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Commander
{
    public class CommandDecorator_TCommand_TResult_Tests
    {
        class TestCommand : ICommand<object> {}
        class Decorator : CommandDecorator<TestCommand,object>
        {
            protected override async Task<object> Decorate( TestCommand command, CancellationToken cancellationToken, CommandDelegate<TestCommand, object> continuation ) =>
                await continuation.Invoke( command, cancellationToken );
        }

        ICommandDecorator<TestCommand,object> instance => new Decorator();

        public class Execute: CommandDecorator_TCommand_TResult_Tests
        {
            TestCommand command = new TestCommand();
            CancellationToken cancellationToken = CancellationToken.None;
            CommandDelegate<TestCommand,object> continuation = ( cmd, cancellationToken ) => throw new NotImplementedException();
            async Task<object> invoke() => await instance.Execute( command, cancellationToken, continuation );

            [Fact]
            public async Task requires_command()
            {
                command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(command), invoke );
            }

            [Fact]
            public async Task requires_continuation()
            {
                continuation = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(continuation), invoke );
            }

            [Fact]
            public async Task cancels_when_requested()
            {
                cancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( invoke );
            }

            [Fact]
            public async Task returns_decorate_result()
            {
                var expected = new object();
                var executions = 0;
                continuation = async ( command, cancellationToken ) =>
                {
                    Assert.Equal( this.command, command );
                    executions++;
                    return expected;
                };

                var actual = await invoke();
                Assert.Equal( 1, executions );
                Assert.Equal( expected, actual );
            }
        }
    }
}
