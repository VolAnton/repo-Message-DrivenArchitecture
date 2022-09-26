using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;





namespace RestaurantApp
{
    public class Program
    {
        public static void BackGroundRemoveBookings(object obj)
        {
            Restaurant restaurant = (Restaurant)obj;
            restaurant.RemoveBookingsAsync();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Restaurant rest = new Restaurant();

            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(BackGroundRemoveBookings);
            // создаем таймер
            Timer timer = new Timer(tm, rest, 1000 * 20, 1000 * 20);

            while (true)
            {
                Console.WriteLine("Привет! Что желаете?" +
                    "\n1 - забронировать столик" +
                    "\n2 - снять бронь стола");

                if (!(int.TryParse(Console.ReadLine(), out int choise1) && choise1 is (1 or 2)))
                {
                    Console.WriteLine($"Введите, пожалуйста 1 или 2"); // проверка валидного ввода                    
                    continue;
                }

                if (choise1 == 1)
                {
                    Console.WriteLine("Как именно желаете забронировать столик?" +
                        "\n1 - мы уведомим Вас по СМС (асинхронно)" +
                        "\n2 - подождите на линии, мы Вас оповестим (синхронно)"); // приглашение ко вводу

                    if (!(int.TryParse(Console.ReadLine(), out int choise2) && choise2 is (1 or 2)))
                    {
                        Console.WriteLine($"Введите, пожалуйста 1 или 2"); // проверка валидного ввода
                        continue;
                    }

                    Stopwatch stopWatch = new Stopwatch();

                    stopWatch.Start(); // Замерим время на бронирование

                    if (choise2 == 1)
                    {
                        rest.BookFreeTableAsync(1); // бронь по СМС
                    }
                    else if (choise2 == 2)
                    {
                        rest.BookFreeTable(1); // бронь по телефону
                    }

                    Console.WriteLine($"Спасибо за Ваше обращение!");
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
                }
                else if (choise1 == 2)
                {
                    Console.WriteLine("Укажите номер стола, с которого следует снять бронь стола");

                    //int number = Int32.Parse(Console.ReadLine());
                    if (!int.TryParse(Console.ReadLine(), out int number))
                    {
                        Console.WriteLine($"Введите, пожалуйста число"); // проверка валидного ввода
                        continue;
                    }


                    Console.WriteLine("Как именно желаете снять бронь стола?" +
                        "\n1 - мы уведомим Вас по СМС (асинхронно)" +
                        "\n2 - подождите на линии, мы Вас оповестим (синхронно)"); // приглашение ко вводу

                    if (!(int.TryParse(Console.ReadLine(), out int choise3) && choise3 is (1 or 2)))
                    {
                        Console.WriteLine($"Введите, пожалуйста 1 или 2"); // проверка валидного ввода
                        continue;
                    }

                    Stopwatch stopWatch = new Stopwatch();

                    stopWatch.Start(); // Замерим время на бронирование

                    if (choise3 == 1)
                    {
                        rest.RemoveBookingByNumberAsync(number); // бронь по СМС
                    }
                    else if (choise3 == 2)
                    {
                        rest.RemoveBookingByNumber(number); // бронь по телефону
                    }

                    Console.WriteLine($"Спасибо за Ваше обращение!");
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
                }
            }

        }

    }
}
