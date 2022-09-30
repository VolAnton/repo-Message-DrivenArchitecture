using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public sealed class KitchenAccident : IKitchenAccident
    {
        public Guid OrderId { get; }

        public bool OrderCancel { get; }


        public KitchenAccident(Guid orderId, bool orderCancel)
        {
            OrderId = orderId;
            OrderCancel = orderCancel;
        }

    }
}
