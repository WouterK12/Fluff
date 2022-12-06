using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;

namespace Fluff
{
    internal class HandlerInvoker : IHandlerInvoker
    {
        public Type EventListenerType { get; }
        public string Topic { get; }
        public MethodInfo HandlerMethod { get; }
        public Type ParameterType { get; }

        private readonly IServiceProvider _serviceProvider;

        public HandlerInvoker(Type eventListenerType, string topic, MethodInfo handlerMethod, Type parameterType, IServiceProvider serviceProvider)
        {
            EventListenerType = eventListenerType;
            Topic = topic;
            HandlerMethod = handlerMethod;
            ParameterType = parameterType;

            _serviceProvider = serviceProvider;
        }

        public void Dispatch(EventMessage message)
        {
            object instance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, EventListenerType);

            object eventObj = JsonSerializer.Deserialize(message.Body, ParameterType)!;
            object[] parameters = new object[] { eventObj };

            HandlerMethod.Invoke(instance, parameters);
        }
    }
}
