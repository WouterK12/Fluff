namespace Fluff
{
    internal interface IHandlerInvoker
    {
        string Topic { get; }

        void Dispatch(EventMessage message);
    }
}