using Converter.Core;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Request;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Converter.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IConverterService, ConverterService>()
                            .AddTransient<ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce>, CurrentRateHandler>();

                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();
                        x.AddDelayedMessageScheduler();
                        x.AddConsumer<ConvertCurrencyConsumer>();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseInMemoryOutbox();

                            cfg.UseDelayedMessageScheduler();
                            cfg.Host(hostContext.Configuration.GetSection("Rabbit").Value);

                            cfg.UseMessageRetry(r =>
                            {
                                r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                            });
                            cfg.ConfigureEndpoints(context);
                           
                        });

                    });
                    services.AddMassTransitHostedService(true);

                });
    }
}
