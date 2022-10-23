using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class Notify : INotify
{
    public Guid OrderId { get; init; }

    public Guid ClientId { get; init; }

    public string Message { get; init; } = string.Empty;

}
