using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class BookingCancel : IBookingCancelRequested
{
    public Guid OrderId { get; init; }

}
