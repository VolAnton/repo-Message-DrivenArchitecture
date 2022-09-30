using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public sealed class TableBooked : ITableBooked
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public Dish? PreOrder { get; }

        public bool Success { get; }

        public TableBooked(Guid orderId, Guid clientId, bool success, Dish? preOrder = null)
        {
            OrderId = orderId;
            ClientId = clientId;
            Success = success;
            PreOrder = preOrder;
        }

    }
}
