namespace Fluff
{
    internal interface IRuntimeBuilder
    {
        IRuntimeBuilder DiscoverAndRegisterAllEventListeners();
        IRuntimeBuilder RegisterEventListener(Type eventListenerType);
        IFluffRouter Build();
    }
}