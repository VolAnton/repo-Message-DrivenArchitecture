using Restaurant.Notifications.Interfaces;

namespace Restaurant.Notifications.Services;

public class Notifier : INotifier
{
    public void Notify(Guid orderId, Guid userId, string message)
    {
        Console.WriteLine($"OrderId {orderId}: Уважаемый клиент {userId}! {message}");
    }

}
