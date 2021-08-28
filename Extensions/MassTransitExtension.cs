using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MassTransit;

namespace MassTransitSample.Extensions
{
    /// <summary>
    /// MassTransitExtension
    /// </summary>
    public static class MassTransitExtension
    {
        /// <summary>
        /// AddRabbitMQ configs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitSection = configuration.GetSection("MassTransit");
            var url = massTransitSection.GetValue<string>("Uri");
            var userName = massTransitSection.GetValue<string>("Username");
            var password = massTransitSection.GetValue<string>("Password");

            if (massTransitSection == null || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Section 'mass-transit' configuration settings are not found in appsettings.json");
            }

            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host
                    (
                        new Uri(url),
                        hostdata =>
                        {
                            hostdata.Username(userName);
                            hostdata.Password(password);
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransitHostedService(true);
        }
    }
}
