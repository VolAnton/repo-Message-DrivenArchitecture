using Restaurant.Messaging.Interfaces;
using System.Collections.Concurrent;

namespace Restaurant.Messaging.Repositories;

public class InMemoryRepository<T> : IInMemoryRepository<T> where T : class
{
    private readonly ConcurrentBag<T> _db = new();

    public void Add(T entity)
    {
        _db.Add(entity);
    }

    public IEnumerable<T> Get()
    {
        return _db;
    }
}
