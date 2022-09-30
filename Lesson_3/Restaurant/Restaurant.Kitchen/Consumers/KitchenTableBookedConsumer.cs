using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Kitchen.Consumers
{
    internal class KitchenTableBookedConsumer : IConsumer<ITableBooked>
    {
        private readonly Manager _manager;

        public KitchenTableBookedConsumer(Manager manager)
        {
            _manager = manager;
        }

        public Task Consume(ConsumeContext<ITableBooked> context)
        {
            bool result = context.Message.Success;

            if (result)
            {
                Console.WriteLine($"Гость {context.Message.ClientId} оформил заказ {context.Message.OrderId} на блюдо {context.Message.PreOrder}");
                _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);
            }

            return Task.CompletedTask;
        }
    }
}
