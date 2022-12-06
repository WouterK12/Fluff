using RabbitMQ.Client;
using Moq;

namespace Fluff.Test.Context
{
    [TestClass]
    public class FluffContextTest
    {
        private FluffContext _sut;

        private Mock<IModel> _channelMock;
        private Mock<IConnection> _connectionMock;
        private Mock<IConnectionFactory> _factoryMock;

        private Mock<IFluffOptions> _optionsMock;
        private IFluffOptions Options => _optionsMock.Object;

        [TestInitialize]
        public void TestInitialize()
        {
            _channelMock = new Mock<IModel>(MockBehavior.Strict);
            _channelMock.Setup(c => c.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), null));
            _channelMock.Setup(c => c.Dispose());

            _connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);
            _connectionMock.Setup(c => c.Dispose());

            _factoryMock = new Mock<IConnectionFactory>(MockBehavior.Strict);
            _factoryMock.Setup(f => f.CreateConnection()).Returns(_connectionMock.Object);

            _optionsMock = new Mock<IFluffOptions>(MockBehavior.Strict);
            _optionsMock.SetupProperty(o => o.ExchangeName, FluffOptionsDefaults.ExchangeName);
            _optionsMock.SetupProperty(o => o.ExchangeType, FluffOptionsDefaults.ExchangeType);
            _optionsMock.SetupProperty(o => o.ExchangeIsDurable, FluffOptionsDefaults.ExchangeIsDurable);
            _optionsMock.SetupProperty(o => o.QueueName, FluffOptionsDefaults.QueueName);
            _optionsMock.Setup(o => o.CreateFactory()).Returns(_factoryMock.Object);

            _sut = new FluffContext(_optionsMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _sut.Dispose();
        }

        [TestMethod]
        public void Constructor_FluffContext_SetsProperties()
        {
            // Act
            var result = new FluffContext(Options);

            // Assert
            Assert.AreEqual(Options.ExchangeName, result.ExchangeName);
            Assert.AreEqual(Options.QueueName, result.QueueName);
        }

        [TestMethod]
        public void Connection_FluffContext_IsNotNull()
        {
            // Act
            using IConnection connection = _sut.Connection;

            // Assert
            Assert.IsNotNull(connection);
        }

        [TestMethod]
        public void CreateChannel_FluffContext_ReturnsChannel()
        {
            // Act
            using IModel channel = _sut.CreateChannel();

            // Assert
            Assert.AreEqual(_channelMock.Object, channel);
        }

        [TestMethod]
        public void Connection_FluffContext_CallsExchangeDeclare()
        {
            // Act
            using IConnection connection = _sut.Connection;

            // Assert
            _optionsMock.Verify(o => o.CreateFactory(), Times.Once);
            _factoryMock.Verify(f => f.CreateConnection(), Times.Once);
            _connectionMock.Verify(c => c.CreateModel(), Times.Once);
            _channelMock.Verify(c => c.ExchangeDeclare(Options.ExchangeName, Options.ExchangeType, Options.ExchangeIsDurable, false, null), Times.Once);
            _channelMock.Verify(c => c.Dispose(), Times.Once);
        }

        [TestMethod]
        public void CreateChannel_FluffContext_CallsCreateModelTwice()
        {
            // Act
            using IModel channel = _sut.CreateChannel();

            // Assert
            _connectionMock.Verify(c => c.CreateModel(), Times.Exactly(2)); // create exchange and create channel
        }

        [TestMethod]
        public void Connection_Twice_FluffContext_CreateConnection_Once()
        {
            // Act
            using IConnection connection1 = _sut.Connection;
            using IConnection connection2 = _sut.Connection;

            // Assert
            _factoryMock.Verify(f => f.CreateConnection(), Times.Once);
        }

        [TestMethod]
        public void CreateChannel_Twice_FluffContext_CreateConnection_Once()
        {
            // Act
            using IModel channel1 = _sut.CreateChannel();
            using IModel channel2 = _sut.CreateChannel();

            // Assert
            _factoryMock.Verify(f => f.CreateConnection(), Times.Once);
        }

        [TestMethod]
        public void Dispose_FluffContext_DisposesConnection()
        {
            // Arrange
            using IConnection connection = _sut.Connection;

            // Act
            _sut.Dispose();

            // Assert
            _connectionMock.Verify(c => c.Dispose(), Times.Once);
        }

        [TestMethod]
        public void Dispose_FluffContext_NoConnection_DoesNotDispose()
        {
            // Act
            _sut.Dispose();

            // Assert
            _connectionMock.Verify(c => c.Dispose(), Times.Never);
        }
    }
}