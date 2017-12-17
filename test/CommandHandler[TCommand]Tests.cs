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
    public class CommandHandler_TCommand_Tests
    {
        class TestCommand : ICommand { }
        class Handler : CommandHandler<TestCommand>
        {
            public Action<TestCommand> callback;
            protected override async Task Execute( TestCommand command, CancellationToken cancellationToken ) => callback.Invoke( command );
        }

        Action<TestCommand> callback;
        ICommandHandler<TestCommand> instance => new Handler { callback = callback };

        public class Handle : CommandHandler_TCommand_Tests
        {
            TestCommand command = new TestCommand();
            CancellationToken cancellationToken = CancellationToken.None;

            async Task<Unit> invoke() => await instance.Handle( command, cancellationToken );

            [Fact]
            public async Task requires_command()
            {
                command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( command ), invoke );
            }

            [Fact]
            public async Task cancels_when_requested()
            {
                cancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( invoke );
            }

            [Fact]
            public async Task returns_override_result()
            {
                var executions = 0;
                callback = cmd =>
                {
                    Assert.Equal( command, cmd );
                    executions++;
                };

                var actual = await invoke();
                Assert.Equal( 1, executions );
                Assert.Equal( Unit.Default, actual );
            }
        }
    }
}