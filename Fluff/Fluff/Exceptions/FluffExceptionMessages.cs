namespace Fluff.Exceptions
{
    internal readonly struct FluffExceptionMessages
    {
        public const string NoTopics = "Expected one or more topics, but no topics where provided.";
        public const string NoQueue = "The queue has not been set up.";
        public const string QueueAlreadySetup = "The queue has already been set up.";
        public const string AlreadyReceiving = "The receiver is already receiving.";
        public const string HandlerMustHaveOneParameter = "Every method with a Handler attribute must have exactly one parameter assignable to Fluff.Events.DomainEvent";
    }
}
