using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace DirectConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: RabbitMQConstants.QueueNameForDirectPublish,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine(" Waiting for messages...");

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("");
                Console.WriteLine(" Message received => '{0}'", message);
            };

            channel.BasicConsume(queue: RabbitMQConstants.QueueNameForDirectPublish,
                                 autoAck: true,
                                 consumer: consumer);


            Console.WriteLine(" Press [enter] to exit.");
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
