using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace DistributedConsumerNo2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: RabbitMQConstants.QueueNameForTaskPublish,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            //to dispatch messages fairly between instances.
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine(" Waiting for messages...");

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("");
                Console.WriteLine(" Message received => '{0}' , working on it...", message);

                Thread.Sleep(5000);

                Console.WriteLine(" Done");
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: RabbitMQConstants.QueueNameForTaskPublish,
                                 autoAck: false,
                                 consumer: consumer);


            Console.WriteLine(" Press [enter] to exit.");
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
