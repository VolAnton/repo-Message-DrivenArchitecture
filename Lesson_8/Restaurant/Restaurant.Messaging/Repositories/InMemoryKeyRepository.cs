using Microsoft.Extensions.Logging;
using Restaurant.Messaging.Interfaces;
using System.Collections.Concurrent;

namespace Restaurant.Messaging.Repositories;

public class InMemoryKeyRepository<T> : IInMemoryKeyRepository<T> where T : class
{
    private const int clearBookingTimerPeriod = 30000;
    private readonly ConcurrentDictionary<string, T> _db = new();
    private readonly ILogger<InMemoryKeyRepository<T>> _logger;

    public InMemoryKeyRepository(ILogger<InMemoryKeyRepository<T>> logger)
    {
        _logger = logger;
    }

    public void Add(T entity, string key)
    {
        _logger.LogInformation("Добавление сущности в InMemoryKeyRepository с Id = {Key}", key);

        if (_db.TryAdd(key, entity))
        {
            var timer = new System.Timers.Timer(clearBookingTimerPeriod);
            timer.Elapsed += (s, e) =>
            {
                _logger.LogInformation("Удаление сущности из InMemoryKeyRepository с Id = {Key}", key);
                _db.TryRemove(key, out _);
            };
            timer.Start();
        }
    }

    public bool Contains(string key)
    {
        return _db.ContainsKey(key);
    }
}
