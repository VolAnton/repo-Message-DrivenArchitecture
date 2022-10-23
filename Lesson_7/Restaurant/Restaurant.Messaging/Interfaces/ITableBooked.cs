using Restaurant.Messaging.Data;

namespace Restaurant.Messaging.Interfaces;

public interface ITableBooked
{
    Guid OrderId { get; }

    Guid ClientId { get; }

    Dish? Dish { get; }

}
