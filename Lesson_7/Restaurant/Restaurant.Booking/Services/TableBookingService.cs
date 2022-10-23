using Microsoft.Extensions.Logging;
using Restaurant.Booking.Enums;
using Restaurant.Booking.Interfaces;
using Restaurant.Booking.Models;
using Restaurant.Messaging.Data;
using System.Collections.Concurrent;
using System.Data;

namespace Restaurant.Booking.Services;

public class TableBookingService : IDisposable, ITableBookingService
{
    private const int ClearBookingTimerPeriod = 30000;
    private const int SearchFreeTableTime = 300;
    private const int ClearBookingTime = 1000;
    private const int ClearAllBookingTime = 300;
    private const int AddOrderTime = 300;
    private const int ClearOrderTime = 300;
    private const int TablesCount = 5;
    private const int MinSeatsTable = 3;
    private const int MaxSeatsTable = 5;

    private readonly ILogger<TableBookingService> _logger;
    private readonly List<Table> _tables = new(TablesCount);
    private readonly System.Timers.Timer _timer;
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public TableBookingService(ILogger<TableBookingService> logger)
    {
        _logger = logger;

        InitTables();

        _timer = new System.Timers.Timer(ClearBookingTimerPeriod);
        _timer.Elapsed += async (s, e) => await RemoveBookingsAsync();
        _timer.Start();
    }

    private void InitTables()
    {
        for (int i = 0; i < TablesCount; i++)
        {
            _tables.Add(new Table(Random.Shared.Next(MinSeatsTable, MaxSeatsTable + 1)));
        }
    }

    public async Task AddOrder(Guid orderId, IEnumerable<int> tableNumbers, Dish? dish, CancellationToken cancel = default)
    {
        await Task.Delay(AddOrderTime, cancel);

        _orders.TryAdd(orderId, new Order() { OrderId = orderId, TableNumbers = tableNumbers, Dish = dish });
    }

    public async Task ClearOrder(Guid orderId, CancellationToken cancel = default)
    {
        if (_orders.TryGetValue(orderId, out var order))
        {
            var tableNumbers = order.TableNumbers;
            await semaphoreSlim.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                foreach (var table in tableNumbers!)
                {
                    RemoveBookingByNumberAsync(table);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }

            _orders.TryRemove(orderId, out _);
        }

        await Task.Delay(ClearOrderTime, cancel);
    }

    public async Task<BookingResult> BookFreeTableAsync(int seatsCount, CancellationToken cancel = default)
    {
        _logger.LogInformation("Добрый день. Подождите секунду, я подберу столик и подтвержу бронь. ");

        await semaphoreSlim.WaitAsync(cancel).ConfigureAwait(false);

        try
        {

            var tables = GetFreeTables(seatsCount);
            await Task.Delay(SearchFreeTableTime, cancel).ConfigureAwait(false);

            if (tables.Count > 0)
            {
                foreach (var item in tables)
                {
                    item.SetBooking(State.Booked);
                }
                
                var result = tables.OrderBy(item => item.Id).Select(item => item.Id).ToList();
                return result;
            }
            else
            {
                return $"[{DateTime.Now:hh:mm:ss}] Столиков нет";

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при бронировании столика на {SeatsCount}", seatsCount);
            return "Сервис временно недоступен. Приносим свои извинения. Вас уведомим  о решении проблемы";
        }
        finally
        {

            semaphoreSlim.Release();
        }
    }

    private List<Table> GetFreeTables(int seats)
    {
        Table? table;
        int seatsLeft = seats;
        var result = new List<Table>();

        while (seatsLeft > 0)
        {
            var freetables = _tables.Where(item => item.State == State.Free && !result.Contains(item)).ToArray();

            if (freetables.Length == 0)
                return new List<Table>();

            var maxSeatsLeft = freetables.Max(item => item.Seats);
            if (seatsLeft >= maxSeatsLeft)
                table = freetables?.OrderByDescending(item => item.Seats).FirstOrDefault();
            else
                table = freetables?.OrderBy(item => item.Seats).FirstOrDefault();

            if (table == null)
                return new List<Table>();

            result.Add(table);
            seatsLeft -= table.Seats;
        }

        return result;
    }

    public bool RemoveBookingByNumberAsync(int number, CancellationToken cancel = default)
    {
        _logger.LogInformation("Снятие брони со столика {Number}...", number);
        Task.Run(async () =>
        {
            await semaphoreSlim.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                await Task.Delay(ClearBookingTime, cancel).ConfigureAwait(false);
                var table = _tables.SingleOrDefault(item => item.Id == number && item.State == State.Booked);

                table?.SetBooking(State.Free);
                if (table is null)
                    _logger.LogInformation("Столик не найден или не забронирован");
                else
                    _logger.LogInformation("Бронь снята со столика {Table}", table.Id);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }, cancel);
        return true;
    }

    private async Task RemoveBookingsAsync(CancellationToken cancel = default)
    {
        _logger.LogInformation("Снятие брони со всех столов...");
        var bookedTablesNumbers = new List<int>();
        await semaphoreSlim.WaitAsync(cancel).ConfigureAwait(false);
        try
        {
            await Task.Delay(ClearAllBookingTime, cancel).ConfigureAwait(false);
            foreach (var item in _tables.Where(item => item.State == State.Booked))
            {
                item.SetBooking(State.Free);
                bookedTablesNumbers.Add(item.Id);
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
        _logger.LogInformation("Бронь снята со столов {BookedTablesNumbers}...", string.Join(",", bookedTablesNumbers));
    }

    public void Dispose()
    {
        _timer.Dispose();
        semaphoreSlim.Dispose();
    }
}
