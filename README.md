# IOn

Attempt at Lee's mini challenge.

## Acceptance

1. Serialize multiple fixed length records as **bytes** then write to file
    - `Record(long SequenceId, short AggregateTypeId, short MessageTypeId, long Timestamp)`
2. Read multiple fixed length records from those files
    - `IEvent(long SequenceId, short AggregateTypeId, short MessageTypeId, long Timestamp)`
3. File writer must stop when some limit is reached
    - File size
    - Record count
4. Filenames should be sequential
    - Target
        - `Records_001_100.dat`
        - `Records_101_200.dat`
        - `Records_201_300.dat`
5. Validate against dataset
    - Known or deterministic

## TODO

- Use
    - Onion architecture
    - Event registration/handling