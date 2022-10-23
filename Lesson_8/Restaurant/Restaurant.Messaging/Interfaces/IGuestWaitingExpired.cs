namespace Restaurant.Messaging.Interfaces;

public interface IGuestWaitingExpired
{
    Guid OrderId { get; }

}
