using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Kitchen
{
    public sealed class Manager
    {
        private readonly IBus _bus;

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public void CheckKitchenReady(Guid orderId, Dish? dish)
        {
            bool dishIsReady = true;

            // Проверяем заказана ли пицца, если да, то публикуем соощение о том, что кухня не готова (передаем его в MassTransit).
            if (dish == Dish.Pizza)
            {
                Console.WriteLine($"Блюдо {dish} в стоп листе для заказа {orderId}");
                dishIsReady = false;
            }

            _bus.Publish<IKitchenReady>(new KitchenReady(orderId, dishIsReady));
            _bus.Publish<IKitchenAccident>(new KitchenAccident(orderId, !dishIsReady));
        }
    }
}
