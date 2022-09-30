using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Notification.Consumers
{
    public sealed class KitchenReadyConsumer : IConsumer<IKitchenReady>
    {
        private readonly Notifier _notifier;

        public KitchenReadyConsumer(Notifier notifier)
        {
            _notifier = notifier;
        }

        public Task Consume(ConsumeContext<IKitchenReady> context)
        {
            var result = context.Message.Ready;

            if (!result)
            {
                Console.WriteLine($"Отмена кухни, блюдо в стоп листе: OrderId = {context.Message.OrderId}");

            }

            _notifier.Accept(context.Message.OrderId, result ? Accepted.Kitchen : Accepted.Rejected);

            return Task.CompletedTask;
        }
    }
}
