using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class GuestWaitingExpired : IGuestWaitingExpired
{
    private readonly RestaurantState _state;

    public Guid OrderId => _state.OrderId;

    public GuestWaitingExpired(RestaurantState state)
    {
        _state = state;
    }

}
