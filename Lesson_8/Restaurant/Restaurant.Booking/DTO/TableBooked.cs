using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class TableBooked : ITableBooked
{
    public Guid OrderId { get; init; }

    public Guid ClientId { get; init; }

    public Dish? Dish { get; init; }

}
