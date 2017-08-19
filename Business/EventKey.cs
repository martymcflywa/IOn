using System;
namespace Business
{
    public class EventKey : IEquatable<EventKey>
    {
        public short AggregateTypeId { get; }
        public short MessageTypeId { get; }

        public EventKey(short aggregateTypeId, short messageTypeId)
        {
            AggregateTypeId = aggregateTypeId;
            MessageTypeId = messageTypeId;
        }

        public override int GetHashCode()
        {
            return (AggregateTypeId + MessageTypeId).GetHashCode();
        }

        public bool Equals(EventKey other)
        {
            return other != null &&
                   other.AggregateTypeId == AggregateTypeId &&
                   other.MessageTypeId == MessageTypeId;
        }
    }
}
