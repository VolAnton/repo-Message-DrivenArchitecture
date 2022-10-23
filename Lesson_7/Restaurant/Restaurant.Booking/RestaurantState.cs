using MassTransit;

namespace Restaurant.Booking;

public class RestaurantState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public int CurrentState { get; set; }

    public Guid OrderId { get; set; }

    public Guid ClientId { get; set; }

    public int ReadyEventStatus { get; set; }

    public Guid? BookingExpirationId { get; set; }

    public Guid? GuestAwaitingId { get; set; }

    public Guid? GuestArrivalId { get; set; }

    // Время прибытия указанное гостем при бронировании
    public int BookingArrivalTime { get; set; }

    // Фактическое время прибытия гостя
    public int ActualArrivalTime { get; set; }
}
