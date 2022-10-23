using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Kitchen.DTO;

public class KitchenReject : IKitchenReject
{
    public Guid OrderId { get; init; }

}
