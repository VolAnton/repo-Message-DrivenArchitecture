using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Consumers
{
    public sealed class KitchenAccidentConsumer : IConsumer<IKitchenAccident>
    {
        public Task Consume(ConsumeContext<IKitchenAccident> context)
        {
            var result = context.Message.OrderCancel;
            var orderId = context.Message.OrderId;

            if (result)
            {
                Console.WriteLine($"Была отмена кухни для заказа OrderId = {orderId}." +
                    $"\nБронирование столика для этого заказа будет отменено.");

                Restaurant.RemoveBookingByOrderIdAsync(orderId);
            }

            return Task.CompletedTask;
        }

    }
}
