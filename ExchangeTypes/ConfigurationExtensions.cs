using ExchangeTypes.Events;
using MassTransit;
using MassTransit.RabbitMqTransport;
using RabbitMQ.Client;

namespace Contracts
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureMessageTopology(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
            configurator.ConfigureEndpoints(context);
            configurator.Host("localhost");
            configurator.Message<UpdateCurrencyInfoEvent>(x => x.SetEntityName("content.received"));
            configurator.Send<UpdateCurrencyInfoEvent>(x =>
            {
                //x.UseCorrelationId(c => c.Id);
                //x.UseRoutingKeyFormatter(c => c.Message.NodeId);
            });

            configurator.Publish<UpdateCurrencyInfoEvent>(x =>
            {
                x.ExchangeType = ExchangeType.Direct;
            });
        }
    }
}