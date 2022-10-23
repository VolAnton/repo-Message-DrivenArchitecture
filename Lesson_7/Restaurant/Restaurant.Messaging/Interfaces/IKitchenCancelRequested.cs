using Restaurant.Messaging.Data;

namespace Restaurant.Messaging.Interfaces;

public interface IKitchenCancelRequested
{
    Guid OrderId { get; }

}
