using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Kitchen.Models;

public class KitchenRequestModel
{
    public Guid OrderId { get; private set; }

    public Guid ClientId { get; private set; }

    public Dish? Dish { get; private set; }

    public KitchenRequestModel(ITableBooked tableBooked)
        : this(tableBooked.OrderId, tableBooked.ClientId, tableBooked.Dish) { }

    public KitchenRequestModel(Guid orderId, Guid clientId, Dish? dish)
    {
        OrderId = orderId;
        ClientId = clientId;
        Dish = dish;
    }
}
