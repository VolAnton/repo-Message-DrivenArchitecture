using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Kitchen.DTO;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Kitchen.Models;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Kitchen.Consumers;

public class KitchenRequestedConsumer : IConsumer<ITableBooked>
{
    private readonly ILogger<KitchenRequestedConsumer> _logger;
    private readonly IKitchenService _kitchen;
    private readonly IInMemoryKeyRepository<KitchenRequestModel> _db;

    public KitchenRequestedConsumer(ILogger<KitchenRequestedConsumer> logger, IKitchenService kitchen, IInMemoryKeyRepository<KitchenRequestModel> db)
    {
        _logger = logger;
        _kitchen = kitchen;
        _db = db;
    }

    public async Task Consume(ConsumeContext<ITableBooked> context)
    {
        _logger.LogInformation("Получение сообщения TableBooked от сервиса бронирования: OrderId = {OrderId}", context.Message.OrderId);

        var model = new KitchenRequestModel(context.Message);
        var key = $"{context.MessageId.ToString()}_{model.OrderId.ToString()}";

        if (_db.Contains(key))
        {
            _logger.LogError("Cообщение MessageId={MessageId} было уже обработано (OrderId = {OrderId} для клиента {ClientId})", context.MessageId, model.OrderId, model.ClientId);
            return;
        }

        if (context.Message.Dish is null)
        {
            _logger.LogInformation("Поле <Блюдо> должно быть заполнено: OrderId = {OrderId}", model.OrderId);
            throw new NullReferenceException("Поле <Блюдо> должно быть заполнено");
        }

        var kitchenIsReady = await _kitchen.CheckKitchenReadyAsync(context.Message.OrderId, context.Message.Dish);

        if (kitchenIsReady)
        {
            await context.Publish(new KitchenReady() { OrderId = model.OrderId } as IKitchenReady);
            _logger.LogInformation("Подтверждение кухни: OrderId = {OrderId}", model.OrderId);
        }
        else
        {
            await context.Publish(new KitchenReject() { OrderId = model.OrderId } as IKitchenReject);
            _logger.LogInformation("Отмена кухни, блюдо в стоп листе: OrderId = {OrderId}", model.OrderId);
        }

        _db.Add(model, key);
    }
}
