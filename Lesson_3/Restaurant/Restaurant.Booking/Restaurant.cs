using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking
{
    public sealed class Restaurant
    {
        private static readonly List<Table> _tables = new List<Table>();

        public Restaurant()
        {
            for (ushort i = 1; i <= 10; i++)
            {
                _tables.Add(new Table(i, NewId.NextGuid()));
            }
        }

        public async Task<bool?> BookFreeTableAsync(int countOfPersons, Guid guid)
        {
            Console.WriteLine("Спасибо за Ваше обращение, я подберу столик и подтвержу вашу бронь, " +
                              "Вам придет уведомление");

            await Task.Delay(1000 * 5); //у нас нерасторопные менеджеры, 5 секунд они находятся в поисках стола

            Table? table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons
                                                        && t.State == State.Free);
            table?.SetGuid(guid);

            return table?.SetState(State.Booked);
        }

        public static void RemoveBookingByOrderIdAsync(Guid orderId)
        {
            Console.WriteLine($"Подождите, отмена бронирования столика для заказа {orderId}.");

            Task.Run(async () =>
            {
                await Task.Delay(1000 * 5); // 5 секунд происходит поиск стола
                Table? table = _tables.FirstOrDefault(t => t.OrderId == orderId && t.State == State.Booked);
                table?.SetState(State.Free);

                Console.WriteLine(table is null
                    ? $"УВЕДОМЛЕНИЕ: Столик не найден или не забронирован"
                    : $"УВЕДОМЛЕНИЕ: Готово! Бронь снята со столика для заказа {orderId}");
            });
        }

    }
}
