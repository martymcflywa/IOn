using System;
using System.Collections.Generic;
using System.IO;
using Business;

namespace Persistence
{
    public class Reader : IDeserializeRegister, IDisposable
    {
        private FileStream Stream;
        private readonly Dictionary<EventKey, Func<long, short, short, long, object>> _deserializeRegister;
        private int Cursor;

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

        public IEnumerable<IEvent> Deserialize(string path)
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

            Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Cursor = 0;
            foreach (var file in files)
            {
                Cursor = 0;
                yield return Read(file, Stream);
            }
        }

        private IEvent Read(string file, FileStream stream)
        {
            // TODO: set file to stream, do i need to init stream again here?
            var sequenceIdBytes = new byte[8];
            stream.Read(sequenceIdBytes, Cursor, sequenceIdBytes.Length);
            var sequenceId = BitConverter.ToInt64(sequenceIdBytes, 0);
            Cursor += sequenceIdBytes.Length;

            var aggregateTypeIdBytes = new byte[2];
            stream.Read(aggregateTypeIdBytes, Cursor, aggregateTypeIdBytes.Length);
            var aggregateTypeId = BitConverter.ToInt16(sequenceIdBytes, 0);
            Cursor += aggregateTypeIdBytes.Length;

            var messageTypeIdBytes = new byte[2];
            stream.Read(messageTypeIdBytes, Cursor, messageTypeIdBytes.Length);
            var messageTypeId = BitConverter.ToInt16(messageTypeIdBytes, 0);
            Cursor += messageTypeIdBytes.Length;

            var timestampBytes = new byte[8];
            stream.Read(timestampBytes, Cursor, timestampBytes.Length);
            var timestamp = BitConverter.ToInt64(timestampBytes, 0);

            var deserialize = _deserializeRegister[new EventKey(aggregateTypeId, messageTypeId)];
            return (IEvent)deserialize(sequenceId, aggregateTypeId, messageTypeId, timestamp);
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

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }
    }
}
