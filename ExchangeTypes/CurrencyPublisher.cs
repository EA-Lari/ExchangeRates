using MassTransit;
using System;
using System.Threading.Tasks;

namespace ExchangeTypes
{
    public class CurrencyPublisher
    {
        private readonly IBus _publishEndpoint;

        public CurrencyPublisher(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish<T>(T @event)
        {
            var endpoint = await _publishEndpoint.GetSendEndpoint(new Uri("queue:event-listener"));
            await endpoint.Send(@event);
        }
    }
}
