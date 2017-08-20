using System;
using System.IO;
using Business;

namespace Persistence
{
    public static class Writer
    {
        public static void Write(this IEvent e, FileStream stream)
        {
            var sequenceIdBytes = BitConverter.GetBytes(e.SequenceId);
            stream.Write(sequenceIdBytes, 0, sequenceIdBytes.Length);

            var aggregateTypeIdBytes = BitConverter.GetBytes(e.AggregateTypeId);
            stream.Write(aggregateTypeIdBytes, 0, aggregateTypeIdBytes.Length);

            var messageTypeIdBytes = BitConverter.GetBytes(e.MessageTypeId);
            stream.Write(messageTypeIdBytes, 0, messageTypeIdBytes.Length);

            var timestampBytes = BitConverter.GetBytes(e.Timestamp);
            stream.Write(timestampBytes, 0, timestampBytes.Length);

            stream.Flush();
        }
    }
}
