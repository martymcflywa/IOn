using System;
using System.Collections.Generic;
using System.IO;
using Business;

namespace Persistence
{
    public class Writer : IDisposable
    {
        private string _path;
        private int _maxSize;
        private int _fileCount;
        private FileStream _stream;

        private const string FilenameFormat = "Records_{0}";
        private const string Extension = ".dat";
        private const string Padding = "D3";

        public void Write(IEnumerable<IEvent> events, string path, int maxSize)
        {
            _path = path;
            _maxSize = maxSize;

            foreach (var e in events)
            {
                if (IsNewFile(_stream))
                {
                    _stream = CreateNewFile();
                }
                Write(e, _stream);
            }
        }

        private static void Write(IEvent e, Stream stream)
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
            _fileCount++;
            var filepath = GetFilepath(FilenameFormat + Extension, Padding);

            _stream?.Dispose();
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            return new FileStream(filepath, FileMode.CreateNew);
        }

        private string GetFilepath(string filenameFormat, string padding)
        {
            return System.IO.Path.Combine(_path, string.Format(filenameFormat, _fileCount.ToString(padding)));
        }

        private bool IsNewFile(Stream stream)
        {
            return stream == null || stream.Length > _maxSize;
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }
    }
}
