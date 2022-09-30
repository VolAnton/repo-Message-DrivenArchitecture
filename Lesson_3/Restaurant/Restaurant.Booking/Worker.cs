using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking
{
    public sealed class Worker : BackgroundService
    {
        private readonly IBus _bus;

        private readonly Restaurant _restaurant;

        public Worker(IBus bus, Restaurant restaurant)
        {
            _bus = bus;
            _restaurant = restaurant;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Random rndDish = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000 * 10, stoppingToken);

                Console.WriteLine("Привет! Желаете забронировать столик?");

                Guid orderId = NewId.NextGuid();

                bool? result = await _restaurant.BookFreeTableAsync(1, orderId);

                // Генерируем заказ случайного блюда
                Dish dish = (Dish)rndDish.Next(1, 5);

                //забронируем с ответом по смс
                await _bus.Publish<ITableBooked>(new TableBooked(orderId, NewId.NextGuid(), result ?? false, dish),
                    context => context.Durable = false, stoppingToken);
            }
        }
    }
}
