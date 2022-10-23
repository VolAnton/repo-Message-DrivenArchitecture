using Restaurant.Booking.Models;
using Restaurant.Messaging.Data;

namespace Restaurant.Booking.Interfaces;

public interface ITableBookingService
{
    Task<BookingResult> BookFreeTableAsync(int seatsCount, CancellationToken cancel = default);

    bool RemoveBookingByNumberAsync(int number, CancellationToken cancel = default);

    void Dispose();

    Task AddOrder(Guid orderId, IEnumerable<int> tableNumbers, Dish? dish, CancellationToken cancel = default);

    Task ClearOrder(Guid orderId, CancellationToken cancel = default);

}
