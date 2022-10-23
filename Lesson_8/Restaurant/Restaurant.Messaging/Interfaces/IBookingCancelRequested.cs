using Restaurant.Messaging.Data;

namespace Restaurant.Messaging.Interfaces;

public interface IBookingCancelRequested
{
    Guid OrderId { get; }

}
