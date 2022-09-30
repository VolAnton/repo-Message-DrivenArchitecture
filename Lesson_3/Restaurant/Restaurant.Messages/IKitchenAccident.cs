namespace Restaurant.Messages
{
    public interface IKitchenAccident
    {
        public Guid OrderId { get; }

        public bool OrderCancel { get; }
    }
}
