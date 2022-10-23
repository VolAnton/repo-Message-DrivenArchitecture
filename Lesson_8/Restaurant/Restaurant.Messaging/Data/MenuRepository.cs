namespace Restaurant.Messaging.Data;

public static class MenuRepository
{
    private static readonly List<Dish> _menu = new()
    {
        new Dish(1, "Рыба"),
        new Dish(2, "Мясо"),
        new Dish(3, "Суп"),
        new Dish(4, "Пицца"),
        new Dish(5, "Картофель"),
        new Dish(6, "Лазанья"),
    };

    public static int Count => _menu.Count;

    public static Dish? GetDishById(int id)
    {
        var result = _menu.SingleOrDefault(item => item.Id == id);

        return result;
    }
}
