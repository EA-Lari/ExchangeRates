using Automatonymous;
using System;

namespace ExchangeTypes.Saga
{
    public class CurrencyState : SagaStateMachineInstance
    {
        public Guid? RequestId { get; set; }

        public Uri ResponseAddress { get; set; }

        public string CurrentState { get; set; }
        public Guid CorrelationId { get ; set ; }
    }
}
