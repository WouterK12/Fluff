using Microsoft.Extensions.Logging;
using Moq;

namespace Fluff.Test.Services
{
    [TestClass]
    public class FluffHostedServiceTest
    {
        private Mock<IBasicReceiver> _basicReceiverMock;
        private Mock<IFluffRouter> _routerMock;

        private CancellationToken _token;
        private FluffHostedService _sut;

        private static string[] _topics = new string[] { "Fluff.Topic1.*", "Fluff.Topic2.*" };

        [TestInitialize]
        public void TestInitialize()
        {
            _basicReceiverMock = new Mock<IBasicReceiver>(MockBehavior.Strict);
            _basicReceiverMock.Setup(b => b.SetupQueue(It.IsAny<IEnumerable<string>>()));
            _basicReceiverMock.Setup(b => b.StartReceiving(It.IsAny<Action<EventMessage>>()));
            _basicReceiverMock.Setup(b => b.Dispose());

            _routerMock = new Mock<IFluffRouter>(MockBehavior.Strict);
            _routerMock.Setup(r => r.Topics).Returns(_topics);

            var loggerMock = new Mock<ILogger<FluffHostedService>>();

            _token = new CancellationToken();
            _sut = new FluffHostedService(_basicReceiverMock.Object, _routerMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void StartAsync_FluffHostedService_StartsReceiver()
        {
            // Act
            _sut.StartAsync(_token);

            // Assert
            _basicReceiverMock.Verify(b => b.SetupQueue(_topics), Times.Once);
            _basicReceiverMock.Verify(b => b.StartReceiving(It.IsAny<Action<EventMessage>>()), Times.Once);
        }

        [TestMethod]
        public void StopAsync_FluffHostedService_DisposesReceiver()
        {
            // Act
            _sut.StopAsync(_token);

            // Assert
            _basicReceiverMock.Verify(b => b.Dispose(), Times.Once);
        }
    }
}
