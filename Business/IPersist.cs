using System.Collections.Generic;

namespace Business
{
    public interface IPersist
    {
        IEnumerable<IEvent> Read();
        void Write(IEnumerable<IEvent> events);
    }
}
