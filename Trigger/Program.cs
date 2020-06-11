using System;
using RabbitMQ.Client;
using System.Text;

namespace Trigger
{
    class Program
    {
        static void Main(string[] args)
        {

            PrintInstructions();
            Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);



                

                var body = Encoding.UTF8.GetBytes("Start the Race");
                channel.BasicPublish(exchange: "trigger",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);
                Console.WriteLine($"-> Triggered Race Start!");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

       

        private static void PrintInstructions()
        {
            Console.ResetColor();
            Console.WriteLine("What do you want to do:\n");
            Console.WriteLine("Press [y] to start the race.");
            Console.WriteLine("Press any other key to start the race.\n");
        }
    }
}
