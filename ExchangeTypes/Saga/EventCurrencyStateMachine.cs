using Automatonymous;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ExchangeTypes.Saga
{
    public class EventCurrencyStateMachine : MassTransitStateMachine<CurrencyState>
    {
        private readonly ILogger<EventCurrencyStateMachine> _logger;

        public State RequestCurrencyRates { get; private set; }

        public State UpdateCurrencyInfo { get; private set; }

        public State ConverRate { get; private set; }

        public State SaveRates { get; private set; }

        public State Failed { get; set; }

        //  public State AcceptedCurrencyRate { get; private set; }

        /// <summary>
        /// Event for StartSaga
        /// </summary>
        public Event<UpdateCurrencyInfoEvent> UpdateCurrencyRate { get; private set; }

        //   public Event<UpdateCurrencyRateEvent> UpdateCurrencyRate { get; private set; }


        //Request for get currency for service
        public Event<GetActualCurrencyResponce> GetActualCurrency { get; set; }

        //Request for saving currency in DB
        public Event<UpdateCurrencyResponce> UpdateCurrency { get; set; }

        //Request for Convert currencies for service
        public Event<ConvertCurrencyResponce> GetConvertCurrencies { get; set; }

        //Request for saving rates in DB
        public Event<UpdateRatesResponce> UpdateRates { get; set; }

        public EventCurrencyStateMachine(ILogger<EventCurrencyStateMachine> logger)
        {
            _logger = logger;

            _logger.LogInformation("Start SAGA");

            //Event<UpdateCurrencyInfoEvent>(() => UpdateCurrencyRate , x => x.CorrelateById(m => m.Message.CorrelationId));

            InstanceState(x => x.CurrentState);

            Event(() => UpdateCurrencyRate, cc => cc
                            .CorrelateBy(state => state.CorrelationId, context => context.CorrelationId)
                            .SelectId(context => Guid.NewGuid()));
            Event(() => GetActualCurrency, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => UpdateCurrency, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => GetConvertCurrencies, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => UpdateRates, x => x.CorrelateById(context => context.Message.CorrelationId));

            //Start, request currency 
            During(Initial,
               When(UpdateCurrencyRate)
               .Then(context =>
               {
                   _logger.LogInformation($"Event1 {nameof(UpdateCurrencyRate)}");
                   context.Instance.CorrelationId = context.Data.CorrelationId;
                   _logger.LogInformation($"Event2 {nameof(UpdateCurrencyRate)}");
               })
               .Publish(ctx => new GetActualCurrencyRequest
               {
                   CorrelationId = ctx.CorrelationId.Value
               })
               .TransitionTo(RequestCurrencyRates)
               .Then(context => _logger.LogInformation($"Set state {nameof(RequestCurrencyRates)}, instance: {context.Instance.ToString()}"))
           );

            //Call saving currency in DB
            During(RequestCurrencyRates,
               When(GetActualCurrency)
               .Then(x => { _logger.LogInformation($"Event {nameof(GetActualCurrency)}"); })
               .Then(context =>
               {
                   context.Instance.CorrelationId = context.Data.CorrelationId;
               })
               .Publish(x => new UpdateCurrencyRequest { 
                   CorrelationId = x.Data.CorrelationId,  
                   Currencies = x.Data.Currencies })
               .TransitionTo(UpdateCurrencyInfo)
               );

            //Call convert rates
            During(UpdateCurrencyInfo,
                When(UpdateCurrency)
                .Then(context =>
                {
                    context.Instance.CorrelationId = context.Data.CorrelationId;
                })
                .Then(x => { _logger.LogInformation($"Event {nameof(UpdateCurrency)}"); })
                .Publish(x => new ConvertCurrencyRequest { 
                    CorrelationId = x.Data.CorrelationId, 
                    Currencies = x.Data.Currencies })
                .TransitionTo(ConverRate)
                );

            //Call save rates
            During(ConverRate,
                When(GetConvertCurrencies)
                .Then(context =>
                {
                    context.Instance.CorrelationId = context.Data.CorrelationId;
                })
                .Then(x => { _logger.LogInformation($"Event {nameof(GetConvertCurrencies)}"); })
                .Publish(x => new UpdateRatesRequest { 
                    CorrelationId = x.Data.CorrelationId, 
                    Currencies = x.Data.Currencies })
                .TransitionTo(SaveRates)
                );

            //Call save rates in DB
            During(SaveRates,
                When(UpdateRates)
                .Then(context =>
                {
                    context.Instance.CorrelationId = context.Data.CorrelationId;
                })
                .Then(x => _logger.LogInformation($"Complite {nameof(UpdateRates)}, CorrelationId: {x.Data.CorrelationId}"))
                .Finalize()
                );

            SetCompletedWhenFinalized();
        }
    }
}
