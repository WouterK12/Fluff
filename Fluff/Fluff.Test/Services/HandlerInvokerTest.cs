using Fluff.Events;
using Fluff.Test.Mocks;
using Moq;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace Fluff.Test.Services
{
    [TestClass]
    public class HandlerInvokerTest
    {
        private ListenerMock _listenerMock = new();

        private Mock<IServiceProvider> _providerMock;

        private const string _topic = "Fluff.Test1.*";
        private static readonly EventMessage _emptyEventMessage = new(null!, "{}");


        [TestInitialize]
        public void TestInitialize()
        {
            _providerMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _providerMock.Setup(p => p.GetService(It.IsAny<Type>()))
                         .Returns(_listenerMock);
        }

        [TestMethod]
        public void Dispatch_HandlerInvoker_InvokesHandlerMethod()
        {
            // Arrange
            var handlerMethodMock = new Mock<MethodInfo>(MockBehavior.Strict);
            handlerMethodMock.Setup(m => m.Invoke(It.IsAny<object?>(), It.IsAny<BindingFlags>(), It.IsAny<Binder?>(), It.IsAny<object?[]?>(), It.IsAny<CultureInfo?>()))
                             .Returns(null);

            var sut = new HandlerInvoker(typeof(object), _topic, handlerMethodMock.Object, typeof(DomainEvent), _providerMock.Object);

            // Act
            sut.Dispatch(_emptyEventMessage);

            // Assert
            handlerMethodMock.Verify(m => m.Invoke(It.IsAny<object?>(), It.IsAny<BindingFlags>(), It.IsAny<Binder?>(), It.IsAny<object?[]?>(), It.IsAny<CultureInfo?>()), Times.Once);
        }

        [TestMethod]
        public void Dispatch_HandlerInvoker_InvokesHandlerMethod_WithParams()
        {
            // Arrange
            DomainEvent eventToSend = new();
            string eventBody = JsonSerializer.Serialize(eventToSend);
            EventMessage eventMessageToSend = new(_topic, eventBody);

            DomainEvent? callParam = null;

            var handlerMethodMock = new Mock<MethodInfo>(MockBehavior.Strict);
            handlerMethodMock.Setup(m => m.Invoke(It.IsAny<object?>(), It.IsAny<BindingFlags>(), It.IsAny<Binder?>(), It.IsAny<object?[]?>(), It.IsAny<CultureInfo?>()))
                             .Callback((object? obj, BindingFlags _, Binder? _, object?[]? parameters, CultureInfo? _) =>
                             {
                                 callParam = parameters?[0] as DomainEvent;
                             })
                             .Returns(null);

            var sut = new HandlerInvoker(typeof(ListenerMock), _topic, handlerMethodMock.Object, typeof(DomainEvent), _providerMock.Object);

            // Act
            sut.Dispatch(eventMessageToSend);

            // Assert
            Assert.AreEqual(eventToSend.Timestamp, callParam?.Timestamp);
            Assert.AreEqual(eventToSend.CorrelationId, callParam?.CorrelationId);
        }

        
        [TestMethod]
        public void Dispatch_HandlerInvoker_InvokesHandlerMethod_OnListenerInstance()
        {
            // Arrange
            ListenerMock? callInstance = null;

            var handlerMethodMock = new Mock<MethodInfo>(MockBehavior.Strict);
            handlerMethodMock.Setup(m => m.Invoke(It.IsAny<object?>(), It.IsAny<BindingFlags>(), It.IsAny<Binder?>(), It.IsAny<object?[]?>(), It.IsAny<CultureInfo?>()))
                             .Callback((object? obj, BindingFlags _, Binder? _, object?[]? _, CultureInfo? _) =>
                             {
                                 callInstance = obj as ListenerMock;
                             })
                             .Returns(null);

            var sut = new HandlerInvoker(typeof(ListenerMock), _topic, handlerMethodMock.Object, typeof(DomainEvent), _providerMock.Object);

            // Act
            sut.Dispatch(_emptyEventMessage);

            // Assert
            Assert.AreEqual(_listenerMock, callInstance);
        }
    }
}
