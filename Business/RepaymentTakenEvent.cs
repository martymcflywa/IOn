namespace Business
{
    public class RepaymentTakenEvent : IEvent
    {
        public long SequenceId { get; set; }
        public short AggregateTypeId { get; set; }
        public short MessageTypeId { get; set; }
        public long Timestamp { get; set; }
    }
}
