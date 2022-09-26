using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class Restaurant
    {
        private readonly List<Table> _tables = new();

        private readonly object _lock = new object();

        public Restaurant()
        {
            lock (_lock)
            {
                for (ushort i = 1; i <= 10; i++)
                {
                    _tables.Add(new Table(i));
                }
            }
        }

        public void BookFreeTable(int countOfPersons)
        {
            lock (_lock)
            {
                Console.WriteLine($"Добрый день. Подождите секунду, я подберу столик и подтвержу вашу бронь, оставайтесь на линии");

                Thread.Sleep(1000 * 5); // 5 секунд происходит поиск стола
                Table? table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == State.Free);
                table?.SetState(State.Booked);

                Console.WriteLine(table is null
                    ? $"К сожалению сейчас все столики заняты."
                    : $"Готово! Ваш столик {table.Id}.");
            }
        }

        public void BookFreeTableAsync(int countOfPersons)
        {
            lock (_lock)
            {
                Console.WriteLine($"Добрый день. Подождите секунду, я подберу столик и подтвержу вашу бронь, Вам придет уведомление");

                Task.Run(async () =>
                {
                    await Task.Delay(1000 * 5); // 5 секунд происходит поиск стола
                    Table? table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == State.Free);
                    table?.SetState(State.Booked);

                    Console.WriteLine(table is null
                        ? $"УВЕДОМЛЕНИЕ: К сожалению сейчас все столики заняты"
                        : $"УВЕДОМЛЕНИЕ: Готово! Ваш столик {table.Id}");
                });
            }
        }

        public void RemoveBookingByNumber(int number)
        {
            lock (_lock)
            {
                Console.WriteLine($"Добрый день. Подождите секунду, я сниму бронь со столика {number}. Оставайтесь на линии");

                Thread.Sleep(1000 * 5); // 5 секунд происходит поиск стола
                Table? table = _tables.FirstOrDefault(t => t.Id == number && t.State == State.Booked);
                table?.SetState(State.Free);

                Console.WriteLine(table is null
                    ? $"Столик не найден или не забронирован."
                    : $"Готово! Бронь снята со столика {table.Id}.");

            }
        }

        public void RemoveBookingByNumberAsync(int number)
        {
            lock (_lock)
            {
                Console.WriteLine($"Добрый день. Подождите секунду, я сниму бронь со столика {number}, Вам придет уведомление");

                Task.Run(async () =>
                {
                    await Task.Delay(1000 * 5); // 5 секунд происходит поиск стола
                    Table? table = _tables.FirstOrDefault(t => t.Id == number && t.State == State.Booked);
                    table?.SetState(State.Free);

                    Console.WriteLine(table is null
                        ? $"УВЕДОМЛЕНИЕ: Столик не найден или не забронирован"
                        : $"УВЕДОМЛЕНИЕ: Готово! Бронь снята со столика {table.Id}");


                });
            }
        }
        
        public async Task RemoveBookingsAsync()
        {
            lock (_lock)
            {
                Task.Run(async () =>
                {
                    Console.WriteLine($"Снятие брони со всех столов...");
                    List<int> bookedTablesNumbers = new List<int>();

                    foreach (Table item in _tables.Where(item => item.State == State.Booked))
                    {
                        item.SetState(State.Free);
                        bookedTablesNumbers.Add(item.Id);
                    }

                    Console.WriteLine($"Бронь снята со столов {string.Join(",", bookedTablesNumbers)}...");
                });
            }
        }

    }
}
