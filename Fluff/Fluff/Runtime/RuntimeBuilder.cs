using Fluff.Events;
using Fluff.Exceptions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Fluff
{
    internal class RuntimeBuilder : IRuntimeBuilder
    {
        private readonly ITypeFinder _typeFinder;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RuntimeBuilder> _logger;
        private readonly List<IHandlerInvoker> _handlers;

        public RuntimeBuilder(ITypeFinder typeFinder, IServiceProvider serviceProvider, ILogger<RuntimeBuilder> logger)
        {
            _typeFinder = typeFinder;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _handlers = new List<IHandlerInvoker>();
        }

        public IRuntimeBuilder DiscoverAndRegisterAllEventListeners()
        {
            IEnumerable<Type> allTypes = _typeFinder.FindAllTypes();
            IEnumerable<Type> listenerTypes = allTypes.Where(t => t.GetCustomAttribute<EventListenerAttribute>() != null);

            foreach (Type listener in listenerTypes)
            {
                RegisterEventListener(listener);
            }

            return this;
        }

        public IRuntimeBuilder RegisterEventListener(Type eventListenerType)
        {
            _logger.LogTrace("Registering EventListener {0}", eventListenerType.FullName);

            foreach (MethodInfo method in eventListenerType.GetMethods())
            {
                HandlerAttribute? handlerAttribute = method.GetCustomAttribute<HandlerAttribute>();

                if (handlerAttribute == null)
                    continue;

                ParameterInfo? parameter = method.GetParameters().FirstOrDefault();

                if (parameter == null || !parameter.ParameterType.IsAssignableTo(typeof(DomainEvent)))
                    throw new FluffException(FluffExceptionMessages.HandlerMustHaveOneParameter);

                _logger.LogTrace("Creating HandlerInvoker for EventListener {0}", eventListenerType.FullName);

                HandlerInvoker invoker = new(eventListenerType, handlerAttribute.Topic, method, parameter.ParameterType, _serviceProvider);
                _handlers.Add(invoker);
            }

            return this;
        }

        public IFluffRouter Build()
        {
            return new FluffRouter(_handlers);
        }
    }
}
