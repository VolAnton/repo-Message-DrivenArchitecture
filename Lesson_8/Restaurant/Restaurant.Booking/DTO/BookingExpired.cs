using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class BookingExpired : IBookingExpired
{
    private readonly RestaurantState _state;

    public Guid OrderId => _state.OrderId;

    public BookingExpired(RestaurantState state)
    {
        _state = state;
    }

}
