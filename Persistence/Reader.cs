using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Business;

namespace Persistence
{
    public class Reader : IDeserializeRegister
    {
        private readonly Dictionary<EventKey, Func<long, short, short, long, object>> _deserializeRegister;

        public Reader()
        {
            _deserializeRegister = new Dictionary<EventKey, Func<long, short, short, long, object>>();
            RegisterDeserializers();
        }

        private void RegisterDeserializers()
        {
            RegisterDeserializeHandler(11, 1, DeserializeCustomerCreatedEvent);
            RegisterDeserializeHandler(11, 16, DeserializeCustomerCreatedEvent);
            RegisterDeserializeHandler(12, 83, DeserializeRepaymentTakenEvent);
            RegisterDeserializeHandler(12, 84, DeserializeRepaymentTakenEvent);
            RegisterDeserializeHandler(12, 85, DeserializeRepaymentTakenEvent);
            RegisterDeserializeHandler(12, 87, DeserializeRepaymentTakenEvent);
            RegisterDeserializeHandler(12, 89, DeserializeRepaymentTakenEvent);
            RegisterDeserializeHandler(12, 92, DeserializeRepaymentTakenEvent);
        }

        public void RegisterDeserializeHandler<TResult>(short aggregateTypeId, short messageTypeId, Func<long, short, short, long, TResult> deserializeHandler)
        {
            _deserializeRegister.Add(new EventKey(aggregateTypeId, messageTypeId), (s, a, m, t) => deserializeHandler(s, a, m, t));
        }

        public IEnumerable<IEvent> Read(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            var files = Directory.GetFiles(path);
            if (files.Length < 0)
            {
                throw new FileNotFoundException(path);
            }

            foreach (var file in files)
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var bytesLeft = fs.Length;
                    while (bytesLeft > 0)
                    {
                        var sequenceIdBytes = new byte[8];
                        bytesLeft -= fs.Read(sequenceIdBytes, 0, sequenceIdBytes.Length);
                        var sequenceId = BitConverter.ToInt64(sequenceIdBytes, 0);

                        var aggregateTypeIdBytes = new byte[2];
                        bytesLeft -= fs.Read(aggregateTypeIdBytes, 0, aggregateTypeIdBytes.Length);
                        var aggregateTypeId = BitConverter.ToInt16(aggregateTypeIdBytes, 0);

                        var messageTypeIdBytes = new byte[2];
                        bytesLeft -= fs.Read(messageTypeIdBytes, 0, messageTypeIdBytes.Length);
                        var messageTypeId = BitConverter.ToInt16(messageTypeIdBytes, 0);

                        var timestampBytes = new byte[8];
                        bytesLeft -= fs.Read(timestampBytes, 0, timestampBytes.Length);
                        var timestamp = BitConverter.ToInt64(timestampBytes, 0);

                        var deserialize = _deserializeRegister[new EventKey(aggregateTypeId, messageTypeId)];
                        yield return (IEvent)deserialize(sequenceId, aggregateTypeId, messageTypeId, timestamp);
                    }
                }
            }
        }

        private IEvent DeserializeCustomerCreatedEvent(long sequenceId, short aggregateTypeId, short messageTypeId, long timestamp)
        {
            return new CustomerCreatedEvent
            {
                SequenceId = sequenceId,
                AggregateTypeId = aggregateTypeId,
                MessageTypeId = messageTypeId,
                Timestamp = timestamp
            };
        }

        private IEvent DeserializeRepaymentTakenEvent(long sequenceId, short aggregateTypeId, short messageTypeId, long timestamp)
        {
            return new RepaymentTakenEvent
            {
                SequenceId = sequenceId,
                AggregateTypeId = aggregateTypeId,
                MessageTypeId = messageTypeId,
                Timestamp = timestamp
            };
        }
    }
}
