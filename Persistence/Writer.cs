using System;
using System.Collections.Generic;
using System.IO;
using Business;

namespace Persistence
{
    public class Writer : IDisposable
    {
        private string Path;
        private int MaxSize;
        private int FileCount;
        private FileStream Stream;

        private const string FILENAME_FORMAT = "Records_{0}";
        private const string EXTENSION = ".dat";
        private const string PADDING = "D3";

        public void Write(IEnumerable<IEvent> events, string path, int maxSize)
        {
            Path = path;
            MaxSize = maxSize;

            foreach (var e in events)
            {
                if (IsNewFile(Stream))
                {
                    Stream = CreateNewFile();
                }
                Write(e, Stream);
            }
        }

        private void Write(IEvent e, FileStream stream)
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

        private FileStream CreateNewFile()
        {
            FileCount++;
            var filepath = GetFilepath(FILENAME_FORMAT + EXTENSION, PADDING);

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            return new FileStream(filepath, FileMode.CreateNew);
        }

        private string GetFilepath(string filenameFormat, string padding)
        {
            return System.IO.Path.Combine(Path, String.Format(filenameFormat, FileCount.ToString(padding)));
        }

        private bool IsNewFile(FileStream stream)
        {
            return stream == null || stream.Length > MaxSize;
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }
    }
}
