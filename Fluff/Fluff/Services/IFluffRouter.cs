namespace Fluff
{
    public interface IFluffRouter
    {
        IEnumerable<string> Topics { get; }

        void Route(EventMessage message);
    }
}