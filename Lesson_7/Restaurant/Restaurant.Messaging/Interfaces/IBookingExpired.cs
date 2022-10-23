namespace Restaurant.Messaging.Interfaces;

public interface IBookingExpired
{
    Guid OrderId { get; }

}
