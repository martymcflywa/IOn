using System.Collections.Generic;

namespace Business
{
    public interface IPersist
    {
        void Write(IEnumerable<IEvent> events, string path, int maxSize);
        IEnumerable<IEvent> Read(string path);
    }
}
