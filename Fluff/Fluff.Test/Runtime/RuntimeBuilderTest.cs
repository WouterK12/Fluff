using Fluff.Events;
using Fluff.Exceptions;
using Fluff.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace Fluff.Test.Runtime
{
    [TestClass]
    public class RuntimeBuilderTest
    {
        private Mock<ITypeFinder> _typeFinderMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<Type> _listenerTypeMock;
        private HandlerAttribute _handlerAttributeMock;

        private RuntimeBuilder _sut;

        private readonly static IEnumerable<Type> _allTypes = new List<Type>() { typeof(ListenerMock), typeof(ListenerMock), typeof(object) };

        [TestInitialize]
        public void TestInitialize()
        {
            _typeFinderMock = new Mock<ITypeFinder>(MockBehavior.Strict);
            _typeFinderMock.Setup(t => t.FindAllTypes())
                           .Returns(_allTypes);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _listenerTypeMock = new Mock<Type>(MockBehavior.Strict);
            _listenerTypeMock.Setup(t => t.MemberType)
                             .Returns(It.IsAny<MemberTypes>());
            _listenerTypeMock.Setup(s => s.FullName)
                             .Returns(string.Empty);
            _handlerAttributeMock = new HandlerAttribute("Fluff.Test1.*");

            var loggerMock = new Mock<ILogger<RuntimeBuilder>>();

            _sut = new RuntimeBuilder(_typeFinderMock.Object, _serviceProviderMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void DiscoverAndRegisterAllEventListeners_RuntimeBuilder_CallsTypeFinder()
        {
            // Act
            _sut.DiscoverAndRegisterAllEventListeners();

            // Assert
            _typeFinderMock.Verify(t => t.FindAllTypes(), Times.Once);
        }

        [TestMethod]
        public void DiscoverAndRegisterAllEventListeners_RuntimeBuilder_CallsRegisterEventListenerTwice()
        {
            // Arrange
            _listenerTypeMock.Setup(t => t.GetMethods(It.IsAny<BindingFlags>()))
                             .Returns(new MethodInfo[] { });
            _listenerTypeMock.Setup(t => t.GetCustomAttributes(typeof(EventListenerAttribute), It.IsAny<bool>()))
                             .Returns(new EventListenerAttribute[] { new EventListenerAttribute() });

            var allTypes = new List<Type>() { _listenerTypeMock.Object, _listenerTypeMock.Object, typeof(object) };

            _typeFinderMock.Setup(t => t.FindAllTypes())
                           .Returns(allTypes);

            // Act
            _sut.DiscoverAndRegisterAllEventListeners();

            // Assert
            _listenerTypeMock.Verify(t => t.GetMethods(It.IsAny<BindingFlags>()), Times.Exactly(2));
        }

        [TestMethod]
        public void DiscoverAndRegisterAllEventListeners_RuntimeBuilder_ReturnsRuntimeBuilder()
        {
            // Act
            var result = _sut.DiscoverAndRegisterAllEventListeners();

            // Assert
            Assert.AreEqual(_sut, result);
        }

        [TestMethod]
        public void RegisterEventListener_RuntimeBuilder_CallsOnMethods()
        {
            // Arrange
            Mock<MethodInfo> handlerMethodMock = GetHandlerMethodMock(_handlerAttributeMock);
            Mock<MethodInfo> noHandlerMethodMock = GetHandlerMethodMock(null!);
            var methodMocks = new MethodInfo[] { handlerMethodMock.Object, handlerMethodMock.Object, noHandlerMethodMock.Object };

            _listenerTypeMock.Setup(t => t.GetMethods(It.IsAny<BindingFlags>()))
                             .Returns(methodMocks);

            // Act
            _sut.RegisterEventListener(_listenerTypeMock.Object);

            // Assert
            _listenerTypeMock.Verify(t => t.GetMethods(It.IsAny<BindingFlags>()), Times.Once);
            handlerMethodMock.Verify(m => m.GetCustomAttributes(typeof(HandlerAttribute), It.IsAny<bool>()), Times.Exactly(2));
            handlerMethodMock.Verify(m => m.GetParameters(), Times.Exactly(2));
            noHandlerMethodMock.Verify(m => m.GetCustomAttributes(typeof(HandlerAttribute), It.IsAny<bool>()), Times.Once);
            noHandlerMethodMock.Verify(m => m.GetParameters(), Times.Never);
        }

        [TestMethod]
        public void RegisterEventListener_MethodNoParams_RuntimeBuilder_ThrowsFluffException()
        {
            // Arrange
            var methodMock = GetHandlerMethodMock(_handlerAttributeMock);
            methodMock.Setup(m => m.GetParameters())
                      .Returns(new ParameterInfo[] { });

            _listenerTypeMock.Setup(t => t.GetMethods(It.IsAny<BindingFlags>()))
                    .Returns(new MethodInfo[] { methodMock.Object });

            // Act
            Action act = () =>
            {
                _sut.RegisterEventListener(_listenerTypeMock.Object);
            };

            // Assert
            FluffException ex = Assert.ThrowsException<FluffException>(act);
            Assert.AreEqual(FluffExceptionMessages.HandlerMustHaveOneParameter, ex.Message);
        }

        [TestMethod]
        public void RegisterEventListener_MethodWithTooManyParams_RuntimeBuilder_ThrowsFluffException()
        {
            // Arrange
            var methodMock = GetHandlerMethodMock(_handlerAttributeMock);
            methodMock.Setup(m => m.GetParameters())
                      .Returns(new ParameterInfo[] { null, null });

            _listenerTypeMock.Setup(t => t.GetMethods(It.IsAny<BindingFlags>()))
                    .Returns(new MethodInfo[] { methodMock.Object });

            // Act
            Action act = () =>
            {
                _sut.RegisterEventListener(_listenerTypeMock.Object);
            };

            // Assert
            FluffException ex = Assert.ThrowsException<FluffException>(act);
            Assert.AreEqual(FluffExceptionMessages.HandlerMustHaveOneParameter, ex.Message);
        }

        [TestMethod]
        public void RegisterEventListener_MethodParamNotAssignableToDomainEvent_RuntimeBuilder_ThrowsFluffException()
        {
            // Arrange
            var methodMock = GetHandlerMethodMock(_handlerAttributeMock);

            Type typeNotAssignableToDomainEvent = typeof(string);

            var parameterMock = new Mock<ParameterInfo>(MockBehavior.Strict);
            parameterMock.Setup(p => p.ParameterType)
                         .Returns(typeNotAssignableToDomainEvent);

            methodMock.Setup(m => m.GetParameters())
                      .Returns(new ParameterInfo[] { parameterMock.Object });

            _listenerTypeMock.Setup(t => t.GetMethods(It.IsAny<BindingFlags>()))
                    .Returns(new MethodInfo[] { methodMock.Object });

            // Act
            Action act = () =>
            {
                _sut.RegisterEventListener(_listenerTypeMock.Object);
            };

            // Assert
            FluffException ex = Assert.ThrowsException<FluffException>(act);
            Assert.AreEqual(FluffExceptionMessages.HandlerMustHaveOneParameter, ex.Message);
        }

        private Mock<MethodInfo> GetHandlerMethodMock(HandlerAttribute handlerAttribute)
        {
            var methodMock = new Mock<MethodInfo>(MockBehavior.Strict);
            methodMock.Setup(m => m.GetCustomAttributes(typeof(HandlerAttribute), It.IsAny<bool>()))
                      .Returns(new[] { handlerAttribute });

            var parameterInfoMock = new Mock<ParameterInfo>(MockBehavior.Strict);
            parameterInfoMock.Setup(p => p.ParameterType)
                             .Returns(typeof(DomainEvent));

            methodMock.Setup(m => m.GetParameters())
                      .Returns(new ParameterInfo[] { parameterInfoMock.Object });
            methodMock.Setup(m => m.MemberType)
                      .Returns(It.IsAny<MemberTypes>());

            return methodMock;
        }

        [TestMethod]
        public void RegisterEventListener_RuntimeBuilder_ReturnsRuntimeBuilder()
        {
            // Act
            var result = _sut.RegisterEventListener(typeof(ListenerMock));

            // Assert
            Assert.AreEqual(_sut, result);
        }

        [TestMethod]
        public void Build_RuntimeBuilder_ReturnsFluffRouter()
        {
            // Arrange
            _sut.RegisterEventListener(typeof(ListenerMock));
            _sut.RegisterEventListener(typeof(ListenerMock));
            _sut.RegisterEventListener(typeof(object));

            // Act
            var result = _sut.Build();

            // Assert
            FluffRouter router = (FluffRouter)result;
            Assert.AreEqual(2, router.Handlers.Count());
            Assert.IsTrue(router.Handlers.Any(h => h.Topic == ListenerMock.Topic));
        }
    }
}
