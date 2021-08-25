using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;

namespace MassTransitSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();

                        //x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host
                            (
                                new Uri(hostContext.Configuration["RabbitMQ:Uri"]),
                                hostdata =>
                                {
                                    hostdata.Username(hostContext.Configuration["RabbitMQ:Username"]);
                                    hostdata.Password(hostContext.Configuration["RabbitMQ:Password"]);
                                }
                            );

                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddMassTransitHostedService(true);

                    services.AddHostedService<Worker>();
                });
    }
}
