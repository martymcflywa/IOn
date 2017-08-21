using System;
using System.Collections.Generic;
using System.IO;
using Business;

namespace Persistence
{
    public class FileManager : IPersist
    {
        public void Write(IEnumerable<IEvent> events, string path, int maxSize)
        {
            using (var writer = new Writer())
            {
                writer.Write(events, path, maxSize);
            }
        }

        public IEnumerable<IEvent> Read(string path)
        {
            using (var reader = new Reader())
            {
                return reader.Read(path);
            }
        }
    }
}
