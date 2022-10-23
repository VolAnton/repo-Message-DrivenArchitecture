using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Kitchen.Consumers;

public class KitchenCancelRequested : IConsumer<IKitchenCancelRequested>
{
    private readonly ILogger<KitchenCancelRequested> _logger;
    private readonly IKitchenService _kitchen;

    public KitchenCancelRequested(ILogger<KitchenCancelRequested> logger, IKitchenService kitchen)
    {
        _logger = logger;
        _kitchen = kitchen;
    }

    public async Task Consume(ConsumeContext<IKitchenCancelRequested> context)
    {
        _logger.LogInformation("Получено сообщение об отмене кухни: OrderId = {OrderId}", context.Message.OrderId);

        await _kitchen.CancelKitchenAsync(context.Message.OrderId);
    }
}
