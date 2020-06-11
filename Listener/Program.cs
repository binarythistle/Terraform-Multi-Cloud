using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Diagnostics;

namespace Listener
{
    class Program
    {
        private static DateTime _startTime;
        private static TimeSpan _provisionDuration;
        private static string _provider;
        private static bool _providerSelection = false;

        static void Main(string[] args)
        {
            PrintInstructions();
            while (!_providerSelection)
            {
                string selection = Console.ReadLine();

                switch (selection.ToLower())
                {
                    case "azure":
                        _provider = "Azure";
                        _providerSelection = true;
                        break;
                    case "gcp":
                        _provider = "GCP";
                        _providerSelection = true;
                        break;
                    case "aws":
                        _provider = "AWS";
                        _providerSelection = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Incorrect selection...");
                        PrintInstructions();
                        break;
                }
            }


            InitialiseTerraform(_provider);

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "trigger",
                                  routingKey: "");
                Console.WriteLine();
                Console.Write("-> Waiting to ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Terraform ");
                Console.ResetColor();
                Console.Write("on ");
                Console.ForegroundColor = ReturnProviderColour(_provider);
                Console.Write(_provider);
                Console.ResetColor();

                Console.WriteLine();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (ModuleHandle, ea) =>
                {
                    _startTime = DateTime.Now;
                    //Process t = Process.Start("terraform", "apply -auto-approve");
                    //t.WaitForExit();
                    ApplyTerraform(_provider);
                    
                    _provisionDuration = DateTime.Now.Subtract(_startTime);
                    Console.WriteLine();
                    Console.ForegroundColor = ReturnProviderColour(_provider);
                    Console.Write($"{_provider} ");
                    Console.ResetColor();
                    Console.WriteLine($"Provisioning Complete. Total Duration: {_provisionDuration}");
                };

                channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

                Console.WriteLine();


                //Console.WriteLine("-> Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void ApplyTerraform(string provider)
        {
            var startInfo = new ProcessStartInfo("terraform");
            startInfo.Arguments = "apply -auto-approve";
            startInfo.WorkingDirectory = ($"./mainfiles/{provider.ToLower()}");
            Process p = Process.Start(startInfo);
            Console.WriteLine("-> terraform apply -auto-approve");
            Console.ForegroundColor = ReturnProviderColour(provider);
            //Console.WriteLine(provider);
            p.WaitForExit();
        }

        private static void InitialiseTerraform(string provider)
        {
            Console.WriteLine("-> terraform init");
            var startInfo = new ProcessStartInfo("terraform");
            startInfo.Arguments = "init";
            startInfo.WorkingDirectory = ($"./mainfiles/{provider.ToLower()}");
            Process p = Process.Start(startInfo);
            p.WaitForExit();
        }

        private static void PrintInstructions()
        {
            
            PrintProvider("Azure", "Microsoft Azure", ReturnProviderColour("Azure"));
            PrintProvider("AWS  ", "Amazon Web Services", ReturnProviderColour("AWS"));
            PrintProvider("GCP  ", "Google Cloud Platform", ReturnProviderColour("GCP"));
            Console.Write("\n\n");

        }

        private static void PrintProvider(string name, string description, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine();
            Console.Write($"  {name} ");
            Console.ResetColor();
            Console.Write($"\t {description}");
        }

        private static ConsoleColor ReturnProviderColour(string provider)
        {
            switch (provider.ToLower())
            {
                case "azure":
                    return ConsoleColor.Blue;

                case "gcp":
                    return ConsoleColor.DarkGreen;

                case "aws":
                    return ConsoleColor.DarkYellow;
                default:
                    return ConsoleColor.Gray;
            }
        }

    }
}
