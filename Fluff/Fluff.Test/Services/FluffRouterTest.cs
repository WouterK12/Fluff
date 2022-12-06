using Moq;

namespace Fluff.Test.Services
{
    [TestClass]
    public class FluffRouterTest
    {
        private static readonly string[] _topics = new string[] { "Fluff.Test1.*", "Fluff.Test2.*" };

        private static readonly IHandlerInvoker[] _handlerInvokers = new IHandlerInvoker[]
        {
            new HandlerInvoker(null, _topics[0], null, null, null),
            new HandlerInvoker(null, _topics[1], null, null, null)
        };

        [TestMethod]
        public void Constructor_FluffRouter_SetsProperties()
        {
            // Act
            var result = new FluffRouter(_handlerInvokers);

            // Assert
            CollectionAssert.AreEqual(_handlerInvokers, result.Handlers.ToArray());

            Assert.AreEqual(2, result.Topics.Count());
            Assert.IsTrue(result.Topics.Any(t => t == _topics[0]));
            Assert.IsTrue(result.Topics.Any(t => t == _topics[1]));
        }

        [TestMethod]
        public void Route_FluffRouter_CallsDispatchWithEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandlerInvoker>(MockBehavior.Strict);
            handlerMock.Setup(h => h.Dispatch(It.IsAny<EventMessage>()));
            handlerMock.Setup(h => h.Topic).Returns(_topics[1]);

            var handleInvokers = new IHandlerInvoker[]
            {
                new HandlerInvoker(null, _topics[0], null, null, null),
                handlerMock.Object
            };
            var sut = new FluffRouter(handleInvokers);

            EventMessage eventToRoute = new(_topics[1], null!);

            // Act
            sut.Route(eventToRoute);

            // Assert
            handlerMock.Verify(h => h.Dispatch(eventToRoute), Times.Once);
        }

        [TestMethod]
        public void Route_TopicTwoHandlers_FluffRouter_CallsDispatchOnBothHandlers()
        {
            // Arrange
            string topicToSend = _topics[1];

            var handlerMock1 = new Mock<IHandlerInvoker>(MockBehavior.Strict);
            handlerMock1.Setup(h => h.Dispatch(It.IsAny<EventMessage>()));
            handlerMock1.Setup(h => h.Topic).Returns(topicToSend);

            var handlerMock2 = new Mock<IHandlerInvoker>(MockBehavior.Strict);
            handlerMock2.Setup(h => h.Dispatch(It.IsAny<EventMessage>()));
            handlerMock2.Setup(h => h.Topic).Returns(topicToSend);

            var handlerInvokers = new IHandlerInvoker[]
            {
                new HandlerInvoker(null, _topics[0], null, null, null),
                handlerMock1.Object,
                handlerMock2.Object
            };
            var sut = new FluffRouter(handlerInvokers);

            EventMessage eventToRoute = new(topicToSend, null!);

            // Act
            sut.Route(eventToRoute);

            // Assert
            handlerMock1.Verify(h => h.Dispatch(eventToRoute), Times.Once);
            handlerMock2.Verify(h => h.Dispatch(eventToRoute), Times.Once);
        }

        // edge case: route an event with a topic that is not present in the router's handlers
        [TestMethod]
        public void Route_TopicNoHandler_FluffRouter_DoesNotThrowException()
        {
            // Arrange
            var sut = new FluffRouter(_handlerInvokers);
            string topic = "Topic with no HandleInvoker!";
            EventMessage eventToRoute = new(topic, null!);

            // Act
            try
            {
                sut.Route(eventToRoute);
            }
            catch (Exception)
            {
                Assert.Fail("Should not throw an Exception");
            }
        }
    }
}
