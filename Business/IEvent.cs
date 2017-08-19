namespace Business
{
    public interface IEvent
    {
        long SequenceId { get; set; }
        short AggregateTypeId { get; set; }
        short MessageTypeId { get; set; }
        long Timestamp { get; set; }
    }
}
