using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;

namespace Restaurant.Booking.DTO;

public class GuestArrived : IGuestArrived
{
    private readonly RestaurantState _state;

    public Guid OrderId => _state.OrderId;

    public GuestArrived(RestaurantState state)
    {
        _state = state;
    }

}
