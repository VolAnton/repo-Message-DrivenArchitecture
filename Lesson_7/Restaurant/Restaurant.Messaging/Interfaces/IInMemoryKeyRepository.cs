namespace Restaurant.Messaging.Interfaces;

public interface IInMemoryKeyRepository<T> where T : class
{
    void Add(T entity, string key);

    bool Contains(string key);

}
