namespace Fluff
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlerAttribute : Attribute
    {
        public string Topic { get; }

        public HandlerAttribute(string topic)
        {
            Topic = topic;
        }
    }
}
