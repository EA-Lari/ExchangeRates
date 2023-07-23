using Converter.Core;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

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
                     services.AddTransient<ConverterService>()
                        .AddTransient<ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce>, CurrentRateHandler>()
                        .AddTransient<CurrencyPublisher>();
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
                             //cfg.ReceiveEndpoint("service", e =>
                             //{
                             //    e.ConfigureConsumer<ConvertCurrencyConsumer>(context);
                             //});
                         });

                     });
                     services.AddMassTransitHostedService(true);

                 });
    }
}
