using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.DTO;
using Restaurant.Booking.Interfaces;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking;

public class Worker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly ITableBookingService _restaurant;
    private readonly IBus _bus;

    public Worker(ILogger<Worker> logger, ITableBookingService restaurant, IBus bus)
    {
        _logger = logger;
        _restaurant = restaurant;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Run(async () =>
            {
                int count = 0;
                while (true)
                {
                    CreateOrderAsync(count, stoppingToken);
#if DEBUG
                    Console.ReadLine();
#else
                    Thread.Sleep(5000);
#endif
                    count++;
                }
            },
            stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанная ошибка {Message}", ex.Message);
        }
    }

    private async Task CreateOrderAsync(int count, CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            var rnd = new Random();
            var seats = rnd.Next(1, 10);
            var bookingArrivalTime = rnd.Next(13, 16); // время прибытия указанное гостем при бронировании
            var actualArrivalTime = rnd.Next(5, 16); // фактическое время прибытия гостя
            var dish = MenuRepository.GetDishById(Random.Shared.Next(1, MenuRepository.Count));

            if (count % 4 == 0)
            {
                dish = MenuRepository.GetDishById(6);
            }                

            _logger.LogInformation("Резервирование столика на {Seats} мест., блюдо {Dish}, забронированное время {BookingArrivalTime}, фактическое время {ActualArrivalTime}",
                seats,
                dish?.Name ?? "",
                bookingArrivalTime,
                actualArrivalTime
            );

            var orderId = NewId.NextGuid();
            var clientId = Guid.NewGuid();

            await _bus.Publish(new BookingRequested()
            {
                OrderId = orderId,
                ClientId = clientId,
                Dish = dish,
                Seats = seats,
                BookingArrivalTime = bookingArrivalTime,
                ActualArrivalTime = actualArrivalTime
            } as IBookingRequested);
        },
        stoppingToken
        );
    }
}