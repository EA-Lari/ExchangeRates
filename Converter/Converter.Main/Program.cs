using Converter.Core;
using ExchangeTypes;
using ExchangeTypes.Consumers;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                         //x.AddConsumers(Assembly.GetEntryAssembly());
                         x.AddConsumer<ConvertCurrencyConsumer>();
                         x.UsingRabbitMq((context, cfg) =>
                         {
                             cfg.Message<ConvertCurrencyRateEvent>(x => x.SetEntityName("ConvertCurrencyRate"));
                             cfg.Message<UpdateCurrencyRateEvent>(x => x.SetEntityName("UpdateCurrencyRate"));
                             cfg.Host(hostContext.Configuration.GetSection("Rabbit").Value);
                             //cfg.ReceiveEndpoint("event-listener", e =>
                             //{
                             //    e.ConfigureConsumer<BaseCurrencyConsumer<ConvertCurrencyRateEvent>>(context);
                             //});
                         });

                     });
                     services.AddMassTransitHostedService(true);
                 });
    }
}
