using Restaurant.Messaging.Data;

namespace Restaurant.Kitchen.Interfaces;

public interface IKitchenService
{
    Task CancelKitchenAsync(Guid orderId);

    Task<bool> CheckKitchenReadyAsync(Guid orderId, Dish dish);

}
