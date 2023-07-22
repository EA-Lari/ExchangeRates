using Automatonymous;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ExchangeTypes.Saga
{
    public class CurrencyStateMachine : MassTransitStateMachine<CurrencyState>
    {
        private readonly ILogger<CurrencyStateMachine> _logger;
        public CurrencyStateMachine(ILogger<CurrencyStateMachine> logger)
        {
            _logger = logger;

            _logger.LogInformation("Start SAGA");

            Event<UpdateCurrencyInfoEvent>(() => UpdateCurrencyRate , x => x.CorrelateById(m => m.Message.CorrelationId));

            InstanceState(x => x.CurrentState);

            Request(() => UpdateCurrency);

            Request(() => GetActualCurrency);

            Request(() => GetConvertCurrencies);

            Request(() => UpdateRates);

            //Start, request currency 
            Initially(
                When(UpdateCurrencyRate)
                .Then(x =>
                {
                    _logger.LogInformation($"Get {nameof(UpdateCurrencyInfoEvent)}");
                    //Сохраняем идентификатор запроса и его адрес при старте саги чтобы потом на него ответить
                    if (!x.TryGetPayload(out SagaConsumeContext<CurrencyState, UpdateCurrencyInfoEvent> payload))
                        throw new Exception($"Unable to retrieve required payload for callback data {nameof(UpdateCurrencyInfoEvent)}.");
                    x.Instance.RequestId = payload.RequestId;
                    x.Instance.ResponseAddress = payload.ResponseAddress;
                    _logger.LogInformation($"End {nameof(UpdateCurrencyInfoEvent)}");
                })
                .Request(GetActualCurrency,
                         x => x.Init<GetActualCurrencyRequest>(new { CorrelationId = x.CorrelationId }))
                .TransitionTo(RequestCurrencyRates)
                ) ;

            //Call saving currency in DB
            During(RequestCurrencyRates,
               When(GetActualCurrency.Completed)
               .Then(x => { _logger.LogInformation($"Get {nameof(GetActualCurrencyResponce)}"); })
               .Request(UpdateCurrency,
                        x => x.Init<UpdateCurrencyRequest>(new UpdateCurrencyRequest { CorrelationId = x.Data.CorrelationId,  Currencies = x.Data.Currencies }))
               .TransitionTo(UpdateCurrencyInfo),

               When(GetActualCurrency.Faulted)
               .Then(x => _logger.LogError($"Error, step {nameof(GetActualCurrency)}: {string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
               .TransitionTo(Failed),

               When(GetActualCurrency.TimeoutExpired)
               .Then(x => _logger.LogError($"Error, step {nameof(GetActualCurrency)}:Timeout Expired On Get Money"))
               .TransitionTo(Failed)
               
               );

            //Call convert rates
            During(UpdateCurrencyInfo,
                When(UpdateCurrency.Completed)
                .Request(GetConvertCurrencies,
                         x => x.Init<ConvertCurrencyRequest>(new ConvertCurrencyRequest { CorrelationId = x.Data.CorrelationId, Currencies = x.Data.Currencies }))
                .TransitionTo(ConverRate),
                
                When(UpdateCurrency.Faulted)
                .Then(x => _logger.LogError($"Error, step {nameof(UpdateCurrency)}: { string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
                .TransitionTo(Failed),

                When(UpdateCurrency.TimeoutExpired)
                .Then(x =>_logger.LogError($"Error, step {nameof(UpdateCurrency)}:Timeout Expired On Get Money"))
                .TransitionTo(Failed)
                
                );

            //Call save rates
            During(ConverRate,
                When(GetConvertCurrencies.Completed)
                .Request(UpdateRates,
                         x => x.Init<UpdateRatesRequest>(new UpdateRatesRequest { CorrelationId = x.Data.CorrelationId, Currencies = x.Data.Currencies }))
                .TransitionTo(SaveRates),

                When(GetConvertCurrencies.Faulted)
                .Then(x => _logger.LogError($"Error, step {nameof(GetConvertCurrencies)}: {string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
                .TransitionTo(Failed),

                When(GetConvertCurrencies.TimeoutExpired)
                .Then(x => _logger.LogError($"Error, step {nameof(GetConvertCurrencies)}:Timeout Expired On Get Money"))
                .TransitionTo(Failed)
                
                );

            //Call save rates in DB
            During(SaveRates,
                When(UpdateRates.Completed)
                .Then(x => _logger.LogInformation($"Complite {nameof(UpdateRates)}, CorrelationId: {x.Data.CorrelationId}")),

                When(UpdateRates.Faulted)
                .Then(x => _logger.LogError($"Error, step {nameof(UpdateRates)}: {string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
                .TransitionTo(Failed),

                When(UpdateRates.TimeoutExpired)
                .Then(x => _logger.LogError($"Error, step {nameof(UpdateRates)}:Timeout Expired On Get Money"))
                .TransitionTo(Failed)
                );
        }

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
        public Request<CurrencyState, GetActualCurrencyRequest, GetActualCurrencyResponce> GetActualCurrency { get; set; }

        //Request for saving currency in DB
        public Request<CurrencyState, UpdateCurrencyRequest, UpdateCurrencyResponce> UpdateCurrency { get; set; }

        //Request for Convert currencies for service
        public Request<CurrencyState, ConvertCurrencyRequest, ConvertCurrencyResponce> GetConvertCurrencies { get; set; }

        //Request for saving rates in DB
        public Request<CurrencyState, UpdateRatesRequest, UpdateRatesResponce> UpdateRates { get; set; }
    }
}
