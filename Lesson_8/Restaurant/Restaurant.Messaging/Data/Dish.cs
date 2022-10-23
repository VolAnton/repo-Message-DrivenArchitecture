
namespace Restaurant.Messaging.Data;

public class Dish
{
    public int Id { get; }

    public string Name { get; }

    public Dish(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
