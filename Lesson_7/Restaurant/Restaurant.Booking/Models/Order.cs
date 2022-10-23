using Restaurant.Booking.Enums;
using Restaurant.Messaging.Data;

namespace Restaurant.Booking.Models;

public class Order
{
    public Guid OrderId { get; init; }

    public IEnumerable<int>? TableNumbers{ get; init; } 

    public Dish? Dish { get; init; }

}
