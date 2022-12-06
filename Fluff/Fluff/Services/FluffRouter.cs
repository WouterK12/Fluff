namespace Fluff
{
    internal class FluffRouter : IFluffRouter
    {
        public IEnumerable<string> Topics { get; }

        public IEnumerable<IHandlerInvoker> Handlers { get; }

        public FluffRouter(IEnumerable<IHandlerInvoker> handlers)
        {
            Handlers = handlers;

            Topics = Handlers.Select(h => h.Topic);
        }

        public void Route(EventMessage message)
        {
            IEnumerable<IHandlerInvoker> handlersWithTopic = Handlers.Where(h => h.Topic == message.Topic);

            foreach (IHandlerInvoker handler in handlersWithTopic)
            {
                handler.Dispatch(message);
            }
        }
    }
}
