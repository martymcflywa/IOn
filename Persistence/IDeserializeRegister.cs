using System;
using System.Collections.Generic;
using Business;

namespace Persistence
{
    public interface IDeserializeRegister
    {
        void RegisterDeserializeHandler<TResult>(short aggregateTypeId, short messageTypeId, Func<long, short, short, long, TResult> deserializeHandler);
        IEnumerable<IEvent> Deserialize(string path);
    }
}
