namespace Business
{
    public class CustomerCreatedEvent : IEvent
    {
        public long SequenceId { get; set; }
        public short AggregateTypeId { get; set; }
        public short MessageTypeId { get; set; }
        public long Timestamp { get; set; }
    }
}
