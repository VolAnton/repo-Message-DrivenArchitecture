using System.Diagnostics;

namespace Restaurant.Booking
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Restaurant rest = new Restaurant();

            while (true)
            {
                await Task.Delay(1000 * 10);

                // считаем, что если уж позвонили, то столик забронировать хотим
                Console.WriteLine("Привет! Желаете забронировать столик?");

                Stopwatch stopWatch = new Stopwatch();

                stopWatch.Start(); // замерим потраченное нами время на бронирование,
                                   // ведь наше время - самое дорогое что у нас есть

                rest.BookFreeTableAsync(1); // забронируем с ответом по смс

                Console.WriteLine("Спасибо за Ваше обращение!"); // клиента всегда нужно порадовать благодарностью

                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;

                Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}"); // выведем потраченное нами время
            }
        }

    }
}