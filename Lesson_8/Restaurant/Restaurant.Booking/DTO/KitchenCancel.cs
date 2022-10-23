using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class KitchenCancel : IKitchenCancelRequested
{
    public Guid OrderId { get; init; }

}
