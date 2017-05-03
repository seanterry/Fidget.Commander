using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Tests of the stock command adapter factory.
    /// </summary>
    
    public class CommandAdapterFactoryTesting
    {
        /// <summary>
        /// Mock of the dependency resolver.
        /// </summary>
        
        protected Mock<IServiceProvider> MockResolver = new Mock<IServiceProvider>();

        /// <summary>
        /// Creates and returns an instance to test with the configured values.
        /// </summary>
        
        protected ICommandAdapterFactory CreateInstance() => new CommandAdapterFactory( MockResolver?.Object );

        /// <summary>
        /// Tests of the constructor.
        /// </summary>
        
        public class Constructor : CommandAdapterFactoryTesting
        {
            [Fact]
            public void Requires_resolver()
            {
                MockResolver = null;
                Assert.Throws<ArgumentNullException>( "resolver", () => CreateInstance() );
            }
        }

        /// <summary>
        /// Tests of the For method.
        /// </summary>
        
        public class For : CommandAdapterFactoryTesting
        {
            /// <summary>
            /// Test command type.
            /// </summary>
            
            public class TestCommand : ICommand<object> {}

            /// <summary>
            /// Command argument.
            /// </summary>
            
            ICommand<object> Command = new TestCommand();

            /// <summary>
            /// Calls the for method with configured values.
            /// </summary>
            
            ICommandAdapter<object> CallFor() => CreateInstance().For( Command );

            [Fact]
            public void Requires_command()
            {
                Command = null;
                Assert.Throws<ArgumentNullException>( "command", () => CallFor() );
            }

            [Fact]
            public void Throws_WhenNoAdapterReturnedFromResolver()
            {
                var adapterType = typeof( ICommandAdapter<TestCommand, object> );
                Assert.Throws<InvalidOperationException>( () => CallFor() );
                MockResolver.Verify( _ => _.GetService( adapterType ), Times.Once );
            }

            [Fact]
            public void Returns_AdapterFromResolver()
            {
                var adapterType = typeof(ICommandAdapter<TestCommand,object>);
                var mockAdapter = new Mock<ICommandAdapter<TestCommand,object>>();
                MockResolver.Setup( _=> _.GetService( adapterType ) ).Returns( mockAdapter.Object );

                var actual = CallFor();
                Assert.Equal( mockAdapter.Object, actual );
                Assert.IsAssignableFrom<ICommandAdapter<object>>( actual );
                Assert.IsAssignableFrom<ICommandAdapter<TestCommand,object>>( actual );

                MockResolver.Verify( _=> _.GetService( adapterType ), Times.Once );
            }
        }
    }
}