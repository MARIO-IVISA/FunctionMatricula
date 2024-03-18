using FunctionPreMatricula;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;

class Program
{
    static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                var rabbitMQConnectionString = "amqp://guest:guest@rabbitmq-service:5672/";
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitMQConnectionString)
                };
                services.AddSingleton(factory);

                services.AddSingleton<Function1>();
            })
            .ConfigureFunctionsWorkerDefaults()
            .Build();

        host.Run();
    }
}
