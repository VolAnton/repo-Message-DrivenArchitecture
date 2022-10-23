
namespace Restaurant.Booking.Models;

public struct BookingResult
{
    public bool Success => string.IsNullOrEmpty(Error) && TableNumbers.Count > 0;

    public string Error { get; init; } = string.Empty;

    public ICollection<int> TableNumbers { get; init; } = new List<int>();

    public BookingResult(List<int> tableNumbers) => TableNumbers = tableNumbers;

    public BookingResult(string error) => Error = error;

    public static implicit operator BookingResult(List<int> tableNumbers) => new (tableNumbers);

    public static implicit operator BookingResult(Exception ex) => new (ex.Message);

    public static implicit operator BookingResult(String error) => new (error);

    public static implicit operator List<int>(BookingResult bookinResult) => (List<int>)bookinResult.TableNumbers;

    public static implicit operator string(BookingResult bookinResult) => bookinResult.Error;

}
