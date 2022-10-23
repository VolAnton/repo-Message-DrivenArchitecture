using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.Interfaces;
using Restaurant.Notifications.Interfaces;

namespace Restaurant.Notifications.Consumers;

public class NotifyConsumer : IConsumer<INotify>
{
    private readonly ILogger<NotifyConsumer> _logger;
    private readonly INotifier _notifier;

    public NotifyConsumer(ILogger<NotifyConsumer> logger, INotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<INotify> context)
    {
        _logger.LogInformation("Получение для заказа {OrderId} сообщения: ", context.Message.OrderId, context.Message.Message);

        _notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);

        return context.ConsumeCompleted;
    }
}
