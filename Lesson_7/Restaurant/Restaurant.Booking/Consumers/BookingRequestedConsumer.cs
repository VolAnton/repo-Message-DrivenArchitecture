using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.DTO;
using Restaurant.Booking.Interfaces;
using Restaurant.Booking.Models;
using Restaurant.Messaging.Exceptions;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.Consumers;

public class BookingRequestedConsumer : IConsumer<IBookingRequested>
{
    private readonly ILogger<BookingRequestedConsumer> _logger;
    private readonly ITableBookingService _tableBookingService;
    private readonly IInMemoryRepository<BookingRequestModel> _db;

    public BookingRequestedConsumer(ILogger<BookingRequestedConsumer> logger, ITableBookingService tableBookingService, IInMemoryRepository<BookingRequestModel> db)
    {
        _logger = logger;
        _tableBookingService = tableBookingService;
        _db = db;
    }

    public async Task Consume(ConsumeContext<IBookingRequested> context)
    {
        _logger.LogInformation("Получено сообщение на бронирование столиков на {Seats} мест] для заказа OrderId = {OrderId}", context.Message.Seats, context.Message.OrderId);

        BookingRequestModel? bookingRequestModel = _db.Get().SingleOrDefault(x => x.OrderId == context.Message.OrderId);

        if (bookingRequestModel is not null && bookingRequestModel.Contains(context.MessageId.ToString()!))
        {
            _logger.LogError("Cообщение MessageId={MessageId} было уже обработано (OrderId = {OrderId} для клиента {ClientId})", context.MessageId, context.Message.OrderId, context.Message.ClientId);
            return;
        }

        _logger.LogInformation("Новое сообщение MessageId={MessageId} (OrderId = {OrderId} для клиента {ClientId})", context.Message.OrderId, context.Message.OrderId, context.Message.ClientId);

        bookingRequestModel = bookingRequestModel?.Update(context.Message, context.MessageId.ToString()!)
            ?? new BookingRequestModel(context.Message, context.MessageId.ToString()!);

        var result = await _tableBookingService.BookFreeTableAsync(bookingRequestModel.Seats);

        if (!result.Success)
        {
            _logger.LogWarning("Ошибка бронирования столиков заказ {OrderId} для клиента {ClientId}: {Error}", bookingRequestModel.OrderId, bookingRequestModel.ClientId.ToString(), result.Error);
            throw new BookingException($"Ошибка сервиса бронирования для заказа {bookingRequestModel.OrderId}");
        }

        _logger.LogInformation("Забронированы столики {TableNumbers} для клиента {ClientId} заказ №{OrderId}", string.Join(",", result.TableNumbers), bookingRequestModel.ClientId.ToString(), bookingRequestModel.OrderId);
        
        await _tableBookingService.AddOrder(bookingRequestModel.OrderId, result.TableNumbers, bookingRequestModel.Dish);
        
        await context.Publish(new TableBooked() { OrderId = bookingRequestModel.OrderId, ClientId = bookingRequestModel.ClientId, Dish = bookingRequestModel.Dish } as ITableBooked);
    }
}
