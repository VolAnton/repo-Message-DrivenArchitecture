using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.Models;

public class BookingRequestModel
{
    private readonly ICollection<string> _messageIds = new List<string>();

    public Guid OrderId { get; private set; }

    public Guid ClientId { get; private set; }

    public Dish? Dish { get; private set; }

    public int Seats { get; private set; }

    public BookingRequestModel(IBookingRequested bookingRequested, string id)
        : this(bookingRequested.OrderId, bookingRequested.ClientId, bookingRequested.Dish, bookingRequested.Seats, id) { }

    public BookingRequestModel(Guid orderId, Guid clientId, Dish? dish, int seats, string id)
    {
        _messageIds.Add(id);

        OrderId = orderId;
        ClientId = clientId;
        Dish = dish;
        Seats = seats;
    }

    public BookingRequestModel Update(IBookingRequested bookingRequested, string id)
    {
        _messageIds.Add(id);

        OrderId = bookingRequested.OrderId;
        ClientId = bookingRequested.ClientId;
        Dish = bookingRequested.Dish;
        Seats = bookingRequested.Seats;

        return this;
    }

    public bool Contains(string messageId)
    {
        return _messageIds.Contains(messageId);
    }
}
