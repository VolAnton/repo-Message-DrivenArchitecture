using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public sealed class Producer
    {
        private readonly string _queueName;
        private readonly string _hostName;

        public Producer(string queueName, string hostName)
        {
            _queueName = queueName;            
            _hostName = hostName;
        }

        public void Send(string message)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _hostName
            };

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(
                "direct_exchange",
                "direct",
                false,
                false,
                null
            );

            byte[] body = Encoding.UTF8.GetBytes(message); // формируем тело сообщения для отправки

            channel.BasicPublish(exchange: "direct_exchange",
                routingKey: _queueName,
                basicProperties: null,
                body: body); // отправляем сообщение
        }

    }
}
