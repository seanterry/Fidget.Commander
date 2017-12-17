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
using System.Text;
using Xunit;

namespace Fidget.Commander.Internal
{
    public class CommandAdapterFactoryTests
    {
        Mock<IServiceProvider> mockResolver = new Mock<IServiceProvider>();

        IServiceProvider resolver => mockResolver?.Object;
        ICommandAdapterFactory instance => new CommandAdapterFactory( resolver );

        public class Constructor : CommandAdapterFactoryTests
        {
            [Fact]
            public void requires_resolver()
            {
                mockResolver = null;
                Assert.Throws<ArgumentNullException>( nameof(resolver), ()=>instance );
            }
        }

        public class CreateFor_TResult : CommandAdapterFactoryTests
        {
            public class TestCommand : ICommand<object> { }

            ICommand<object> command = new TestCommand();
            ICommandAdapter<object> invoke() => instance.CreateFor( command );

            [Fact]
            public void requires_command()
            {
                command = null;
                Assert.Throws<ArgumentNullException>( nameof( command ), () => invoke() );
            }

            [Fact]
            public void returns_adapter()
            {
                var handler = new Mock<ICommandHandler<TestCommand,object>>().Object;
                var decorators = new List<ICommandDecorator<TestCommand,object>>();
                var expected = new CommandAdapter<TestCommand,object>( handler, decorators );
                mockResolver.Setup( _ => _.GetService( typeof( CommandAdapter<TestCommand,object> ) ) ).Returns( expected );

                var actual = invoke();
                Assert.Equal( expected, actual );
            }
        }
    }
}