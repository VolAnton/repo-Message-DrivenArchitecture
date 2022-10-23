using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Interfaces;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.Consumers;

public class BookingCancelRequested : IConsumer<IBookingCancelRequested>
{
    private readonly ILogger<BookingCancelRequested> _logger;
    private readonly ITableBookingService _tableBookingService;

    public BookingCancelRequested(ILogger<BookingCancelRequested> logger, ITableBookingService tableBookingService)
    {
        _logger = logger;
        _tableBookingService = tableBookingService;
    }

    public async Task Consume(ConsumeContext<IBookingCancelRequested> context)
    {
        _logger.LogInformation("Получено сообщение об отмене кухни: OrderId = {OrderId}", context.Message.OrderId);
        await _tableBookingService.ClearOrder(context.Message.OrderId);
    }
}
