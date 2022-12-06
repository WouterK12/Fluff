using Fluff.Events;
using Moq;
using System.Text.Json;

namespace Fluff.Test.Publisher
{
    [TestClass]
    public class FluffPublisherTest
    {
        private FluffPublisher _sut;

        private Mock<IBasicSender> _basicSenderMock;

        private const string _topic = "Fluff.Test1.*";

        [TestInitialize]
        public void TestInitialize()
        {
            _basicSenderMock = new Mock<IBasicSender>(MockBehavior.Strict);
            _basicSenderMock.Setup(b => b.Send(It.IsAny<EventMessage>()));

            _sut = new FluffPublisher(_basicSenderMock.Object);
        }

        [TestMethod]
        public void Publish_FluffPublisher_CallsSenderSend()
        {
            // Arrange
            var domainEvent = new DomainEvent();

            // Act
            _sut.Publish(_topic, domainEvent);

            // Assert
            _basicSenderMock.Verify(b => b.Send(It.IsAny<EventMessage>()), Times.Once);
        }

        [TestMethod]
        public void Publish_FluffPublisher_SerializesEvent()
        {
            // Arrange
            var domainEvent = new DomainEvent();

            // Act
            _sut.Publish(_topic, domainEvent);

            // Assert
            string expectedBody = JsonSerializer.Serialize(domainEvent);
            EventMessage expectedMessage = new EventMessage(_topic, expectedBody);

            _basicSenderMock.Verify(b => b.Send(expectedMessage), Times.Once);
        }
    }
}
