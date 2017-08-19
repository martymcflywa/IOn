using System;
using System.Collections.Generic;
using System.Linq;
using Business;

namespace EventSource
{
    /// <summary>
    /// Generates an IEnumerable IEvent stream.
    /// </summary>
    public class EventGenerator : IGenerator
    {
        private readonly Dictionary<EventKey, Func<int, object>> _eventRegister;
        private readonly Random _random;
        private EventKey[] KeysArray;

        public EventGenerator()
        {
            _eventRegister = new Dictionary<EventKey, Func<int, object>>();
            _random = new Random();
            RegisterGenerators();
        }

        private void RegisterGenerators()
        {
            RegisterEventGenerator(11, 1, GenerateCustomerCreatedEvent);
            RegisterEventGenerator(11, 16, GenerateCustomerCreatedEvent);
            RegisterEventGenerator(12, 83, GenerateRepaymentTakenEvent);
            RegisterEventGenerator(12, 84, GenerateRepaymentTakenEvent);
            RegisterEventGenerator(12, 85, GenerateRepaymentTakenEvent);
            RegisterEventGenerator(12, 87, GenerateRepaymentTakenEvent);
            RegisterEventGenerator(12, 89, GenerateRepaymentTakenEvent);
            RegisterEventGenerator(12, 92, GenerateRepaymentTakenEvent);
        }

        public void RegisterEventGenerator<TResult>(short aggregateTypeId, short messageTypeId, Func<int, TResult> eventGenerator)
        {
            _eventRegister.Add(new EventKey(aggregateTypeId, messageTypeId), e => eventGenerator(e));
        }

        public IEnumerable<IEvent> Get(int limit)
        {
            return Generate(limit);
        }

        private IEnumerable<IEvent> Generate(int limit)
        {
            var i = 1;
            while (i <= limit)
            {
                var handler = _eventRegister[RandomKey()];
                yield return (IEvent)handler(i++);
            }
        }

        private EventKey RandomKey()
        {
            if (KeysArray == null || !KeysArray.Any())
            {
                KeysArray = _eventRegister.Keys.ToArray();
            }
            return KeysArray[_random.Next(KeysArray.Count())];
        }

        private CustomerCreatedEvent GenerateCustomerCreatedEvent(int sequenceId)
        {
            return new CustomerCreatedEvent
            {
                SequenceId = sequenceId,
                AggregateTypeId = 11,
                MessageTypeId = 1,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
        }

        private RepaymentTakenEvent GenerateRepaymentTakenEvent(int sequenceId)
        {
            return new RepaymentTakenEvent
            {
                SequenceId = sequenceId,
                AggregateTypeId = 12,
                MessageTypeId = 89,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
        }
    }
}
