using System;
using System.Collections.Generic;
using System.IO;
using Business;

namespace Persistence
{
    public class FileManager : IPersist, IDisposable
    {
        private readonly string _path;
        private readonly int _maxSize;
        private FileStream Stream;
        private int FileCount;

        private const string FILENAME_FORMAT = "Records_{0}";
        private const string EXTENSION = ".dat";
        private const string PADDING = "D3";

        public FileManager(string path, int maxSize)
        {
            _path = path;
            _maxSize = maxSize;
            FileCount = 0;
        }

        public void Write(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                if (IsNewFile(Stream))
                {
                    Stream = CreateNewFile();
                }
                e.Write(Stream);
            }
        }

        private FileStream CreateNewFile()
        {
            FileCount++;
            var filepath = GetFilepath(FILENAME_FORMAT + EXTENSION, PADDING);

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            return new FileStream(filepath, FileMode.CreateNew);
        }

        private string GetFilepath(string filenameFormat, string padding)
        {
            return Path.Combine(_path, String.Format(filenameFormat, FileCount.ToString(padding)));
        }

        private bool IsNewFile(FileStream stream)
        {
            return stream == null || stream.Length > _maxSize;
        }

        public IEnumerable<IEvent> Read()
        {
            throw new NotImplementedException();
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
