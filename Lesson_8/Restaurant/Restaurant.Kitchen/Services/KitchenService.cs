using Microsoft.Extensions.Logging;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Exceptions;

namespace Restaurant.Kitchen.Services;

public class KitchenService : IKitchenService
{
    private readonly List<int> _stopList;

    private readonly ILogger<KitchenService> _logger;

    public KitchenService(ILogger<KitchenService> logger)
    {
        _logger = logger;
        _stopList = new List<int> { 2 };
    }

    public async Task<bool> CheckKitchenReadyAsync(Guid orderId, Dish dish)
    {        
        var checkTime = 300;

        if (MenuRepository.GetDishById(dish.Id) is null)
            throw new KitchenException("Блюдо не существует в меню");

        if (dish.Id == 6)
            throw new KitchenException("Не принимаем предзаказ с лазаньей");

        _logger.LogInformation("Проверка блюда {DishId} {DishName} в стоп-листе за {CheckTime} сек", dish.Id, dish.Name, checkTime);
        await Task.Delay(checkTime);
        return !_stopList.Contains(dish.Id);
    }

    public async Task CancelKitchenAsync(Guid orderId)
    {
        _logger.LogInformation("Отмена заказа {Order} кухней", orderId);

        await Task.Delay(100);
    }
}
