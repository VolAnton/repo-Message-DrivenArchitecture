using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Kitchen.DTO;

public class KitchenReady : IKitchenReady
{
    public Guid OrderId { get; init; }

}
