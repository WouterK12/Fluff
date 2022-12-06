using Moq;
using RabbitMQ.Client;
using System.Text;

namespace Fluff.Test.Sender
{
    [TestClass]
    public class BasicSenderTest
    {
        private Mock<IModel> _channelMock;
        private Mock<IFluffContext> _contextMock;
        private BasicSender _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _channelMock = new Mock<IModel>(MockBehavior.Strict);
            _channelMock.Setup(c => c.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()));
            _channelMock.Setup(c => c.Dispose());

            _contextMock = new Mock<IFluffContext>(MockBehavior.Strict);
            _contextMock.Setup(c => c.ExchangeName).Returns(FluffOptionsDefaults.ExchangeName);
            _contextMock.Setup(c => c.QueueName).Returns(FluffOptionsDefaults.QueueName);
            _contextMock.Setup(c => c.CreateChannel()).Returns(_channelMock.Object);

            _sut = new BasicSender(_contextMock.Object);
        }

        [TestMethod]
        public void Send_EventMessage_BasicSender_CallsCreateChannel()
        {
            // Arrange
            var message = new EventMessage(FluffOptionsDefaults.QueueName, "Hello world!");

            // Act
            _sut.Send(message);

            // Assert
            _contextMock.Verify(c => c.CreateChannel(), Times.Once);
            _channelMock.Verify(c => c.Dispose(), Times.Once);
        }

        [TestMethod]
        public void Send_EventMessage_BasicSender_CallsBasicPublish()
        {
            // Arrange
            var message = new EventMessage(FluffOptionsDefaults.QueueName, "Hello world!");

            // Act
            _sut.Send(message);

            // Assert
            _channelMock.Verify(c => c.BasicPublish(FluffOptionsDefaults.ExchangeName, FluffOptionsDefaults.QueueName, false, null, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
        }

        [TestMethod]
        public void Send_EventMessage_BasicSender_ParsesBodyToUnicode()
        {
            // Arrange
            ReadOnlyMemory<byte> callBody = null;

            _channelMock.Setup(c => c.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                        .Callback((string _, string _, bool _, IBasicProperties _, ReadOnlyMemory<byte> body) =>
                        {
                            callBody = body;
                        });

            var message = new EventMessage(FluffOptionsDefaults.QueueName, "Hello world!");

            // Act
            _sut.Send(message);

            // Assert
            byte[] expectedBody = Encoding.Unicode.GetBytes(message.Body);
            CollectionAssert.AreEqual(expectedBody, callBody.ToArray());
        }
    }
}
