using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Commander.Dispatch
{
    /// <summary>
    /// Tests of the stock command dispatcher.
    /// </summary>

    public class CommandDispatcherTesting
    {
        /// <summary>
        /// Mock adapter factory.
        /// </summary>
        
        Mock<ICommandAdapterFactory> MockFactory = new Mock<ICommandAdapterFactory>();

        /// <summary>
        /// Creates and returns an instance to test.
        /// </summary>
        
        ICommandDispatcher CreateInstance() => new CommandDispatcher( MockFactory?.Object );
        
        public class Constructor : CommandDispatcherTesting
        {
            [Fact]
            public void Requires_factory()
            {
                MockFactory = null;
                Assert.Throws<ArgumentNullException>( "factory", () => CreateInstance() );
            }
        }

        public class Execute : CommandDispatcherTesting
        {
            /// <summary>
            /// Test command type.
            /// </summary>
        
            public class TestCommand : ICommand<object> {}

            /// <summary>
            /// Test command instance.
            /// </summary>
        
            TestCommand Command = new TestCommand();

            /// <summary>
            /// Cancellation token argument.
            /// </summary>
        
            CancellationToken CancellationToken = CancellationToken.None;

            /// <summary>
            /// Calls the execute method with configured values.
            /// </summary>
        
            async Task<object> CallExecute() => await CreateInstance().Execute( Command, CancellationToken );

            [Fact]
            public async Task Requires_command()
            {
                Command = null;
                await Assert.ThrowsAsync<ArgumentNullException>( "command", CallExecute );
            }

            [Fact]
            public async Task Cancels_WhenCancellationRequested()
            {
                CancellationToken = new CancellationToken( true );
                await Assert.ThrowsAsync<OperationCanceledException>( CallExecute );
            }

            [Fact]
            public async Task Throws_WhenNoAdapterReturnedFromFactory()
            {
                await Assert.ThrowsAsync<InvalidOperationException>( CallExecute );
                MockFactory.Verify( _=> _.For( Command ), Times.Once ); 
            }

            [Fact]
            public async Task Returns_ResultOfAdapterExecute()
            {
                var expected = new object();
                var mockAdapter = new Mock<ICommandAdapter<object>>();
                mockAdapter.Setup( _=> _.Execute( Command, CancellationToken ) ).ReturnsAsync( expected );
                MockFactory.Setup( _=> _.For( Command ) ).Returns( mockAdapter.Object );

                var actual = await CallExecute();
                Assert.Equal( expected, actual );

                MockFactory.Verify( _=> _.For( Command ), Times.Once );
                mockAdapter.Verify( _=> _.Execute( Command, CancellationToken ), Times.Once );
            }
        }
    }
}