using Restaurant.Booking.Enums;

namespace Restaurant.Booking.Models;

public class Table
{
    private static int counter = 1;

    public int Id { get; init; }

    public int Seats { get; init; }

    public State State { get; private set; }

    public Table(int seats)
    {
        Id = counter++;
        Seats = seats;
        State = State.Free;
    }

    public void SetBooking(State state)
    {
        State = state;
    }
}
