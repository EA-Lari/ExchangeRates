using Automatonymous;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeTypes.Saga
{
    public class CurrencyStateMachine : MassTransitStateMachine<CurrencyState>
    {
        private readonly ILogger<CurrencyStateMachine> _logger;
        public CurrencyStateMachine(ILogger<CurrencyStateMachine> logger)
        {
            _logger = logger;

            Event(() => UpdateCurrencyInfo , x => x.CorrelateById(m => m.Message.CorrelationId));

            InstanceState(x => x.CurrentState);

            Request(() => GetActualCurrency);

            Request(() => GetConvertCurrencies);

            Initially(
                When(UpdateCurrencyInfo)
                .Then(x =>
                {
                    //Сохраняем идентификатор запроса и его адрес при старте саги чтобы потом на него ответить
                    if (!x.TryGetPayload(out SagaConsumeContext<CurrencyState, UpdateCurrencyInfoEvent> payload))
                        throw new Exception($"Unable to retrieve required payload for callback data {nameof(UpdateCurrencyInfoEvent)}.");
                    x.Instance.RequestId = payload.RequestId;
                    x.Instance.ResponseAddress = payload.ResponseAddress;
                })
                .Request(GetActualCurrency,
                         x => x.Init<GetActualCurrencyRequest>(new { CorrelationId = x.Data.CorrelationId }))
                .TransitionTo(RequestCurrencyInfo)
                ) ;

            During(RequestCurrencyInfo,
                When(GetActualCurrency.Completed)
                .Request(GetConvertCurrencies,
                         x => x.Init<GetActualCurrencyRequest>(new { CorrelationId = x.Data.CorrelationId, Currencies = x.Data.Currencies }))
                .TransitionTo(ConvertCurrencyInfo),
                
                When(GetActualCurrency.Faulted)
                .Then(x => _logger.LogError($"Error, step {nameof(GetActualCurrency)}: { string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
                .TransitionTo(Failed),

                When(GetActualCurrency.TimeoutExpired)
                .Then(x =>_logger.LogError($"Error, step {nameof(GetActualCurrency)}:Timeout Expired On Get Money"))
                .TransitionTo(Failed)
                );

            During(ConvertCurrencyInfo,
                When(GetConvertCurrencies.Completed)
                .Then(x => _logger.LogInformation($"Complite {nameof(GetConvertCurrencies)}, curriencies: {string.Join(",", x.Data.Currencies.Select(y=> y.Name))}"))
                .TransitionTo(ConvertCurrencyInfo),

                When(GetConvertCurrencies.Faulted)
                .Then(x => _logger.LogError($"Error, step {nameof(GetConvertCurrencies)}: {string.Join(";\n", x.Data.Exceptions.Select(x => x.Message))}"))
                .TransitionTo(Failed),

                When(GetConvertCurrencies.TimeoutExpired)
                .Then(x => _logger.LogError($"Error, step {nameof(GetConvertCurrencies)}:Timeout Expired On Get Money"))
                .TransitionTo(Failed)
                );

            //Initially(
            //When(UpdateCurrencyInfo)
            //    .TransitionTo(AcceptedCurrencyInfo));

            //During(AcceptedCurrencyInfo,
            //    When(UpdateCurrencyRate)
            //        .TransitionTo(AcceptedCurrencyRate));

            //During(AcceptedCurrencyRate,
            //    When(UpdateCurrencyRate)
            //        .TransitionTo(AcceptedCurrencyRate));
            //During(AcceptedCurrencyInfo,
            //When(UpdateCurrencyRate)
            //    .Then(x => x.Instance = x.Data));
        }

        public State RequestCurrencyInfo { get; private set; }

        public State ConvertCurrencyInfo { get; private set; }

        public State SaveCurrencyInfo { get; private set; }

        public State Failed { get; set; }

        //  public State AcceptedCurrencyRate { get; private set; }

        /// <summary>
        /// Event for StartSaga
        /// </summary>
        public Event<UpdateCurrencyInfoEvent> UpdateCurrencyInfo { get; private set; }

        //   public Event<UpdateCurrencyRateEvent> UpdateCurrencyRate { get; private set; }

        //Request for get currency for service
        public Request<CurrencyState, GetActualCurrencyRequest, GetActualCurrencyResponce> GetActualCurrency { get; set; }

        //Request for Convert currencies for service
        public Request<CurrencyState, GetActualCurrencyRequest, GetActualCurrencyResponce> GetConvertCurrencies { get; set; }
    }
}
