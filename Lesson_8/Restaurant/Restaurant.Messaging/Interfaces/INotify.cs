namespace Restaurant.Messaging.Interfaces;

public interface INotify
{
    Guid OrderId { get; }

    Guid ClientId { get; }

    string Message { get; }

}
