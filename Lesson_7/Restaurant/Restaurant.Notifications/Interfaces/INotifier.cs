namespace Restaurant.Notifications.Interfaces;

public interface INotifier
{
    void Notify(Guid orderId, Guid userId, string message);

}
