namespace Restaurant.Messaging.Interfaces;

public interface IInMemoryRepository<T> where T : class
{
    IEnumerable<T> Get();

    void Add(T entity);

}
