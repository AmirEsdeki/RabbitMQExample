using Common;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName };

            using var connection = factory.CreateConnection();

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("=======================================");
                Console.WriteLine(" Enter [d] to publish direct message.");
                Console.WriteLine(" Enter [t] to publish task message.");
                Console.WriteLine(" Enter [f] to publish fanout message.");
                Console.WriteLine(" Enter [e] to exit.");

                string userEntry = Console.ReadLine();

                if (userEntry == "d")
                {
                    Console.WriteLine("");
                    Console.WriteLine(" Enter your message...");
                    var message = Console.ReadLine();

                    SendDirectMessage(connection, message);
                }
                else if (userEntry == "t")
                {
                    var messages =
                        new string[]
                        { "message No.1",
                            "message No.2",
                            "message No.3",
                            "message No.4",
                            "message No.5" };

                    SendTaskMessage(connection, messages);
                }
                else if (userEntry == "f")
                {
                    Console.WriteLine("");
                    Console.WriteLine(" Enter your message...");
                    var message = Console.ReadLine();

                    SendFanOutMessage(connection, message);
                }
                else if (userEntry == "e")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(" Just Enter [d], [t], [f] or [e].");
                }
            }
        }

        private static void SendTaskMessage(IConnection connection, string[] messages)
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: RabbitMQConstants.QueueNameForTaskPublish,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                //to persist messages in disk
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: RabbitMQConstants.QueueNameForTaskPublish,
                                         basicProperties: properties,
                                         body: body);

                    Console.WriteLine(" You have Sent '{0}'", message);
                }

                Console.WriteLine("");
            }
        }

        private static void SendFanOutMessage(IConnection connection, string message)
        {
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: RabbitMQConstants.FanOutExchangeName, type: ExchangeType.Fanout);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: RabbitMQConstants.FanOutExchangeName,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine("");
                Console.WriteLine(" You have Sent '{0}' to every listening client", message);
            }
        }

        private static void SendDirectMessage(IConnection connection, string message)
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: RabbitMQConstants.QueueNameForDirectPublish,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: RabbitMQConstants.QueueNameForDirectPublish,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine("");
                Console.WriteLine(" You have Sent '{0}' directly to the listener", message);
            }
        }
    }
}
