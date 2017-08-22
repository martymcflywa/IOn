using System;
using System.IO;
using System.Linq;
using EventSource;
using Persistence;
using Xunit;

namespace PersistenceTest
{
    public class FileManagerTest
    {
        [Fact]
        public void Write()
        {
            var eventRecordLength = 20;
            var limit = 500;
            var maxSize = 1000;
            var generator = new EventGenerator();
            var events = generator.Get(limit);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test");
            var fileManager = new FileManager();
            fileManager.Write(events, path, maxSize);

            var actualFileCount = 0;
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                Assert.True(file.Length <= maxSize);
                Assert.Equal(".dat", fileInfo.Extension);
                actualFileCount++;
            }
            var expectedFileCount = Math.Ceiling((double)(limit * eventRecordLength) / maxSize);
            Assert.Equal(expectedFileCount, actualFileCount);
            Teardown(path);
        }

        [Fact]
        public void Read()
        {
            var limit = 5;
            var maxSize = 1000;
            var generator = new EventGenerator();
            var expected = generator.Get(limit).ToList();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test");
            var fileManager = new FileManager();
            fileManager.Write(expected, path, maxSize);

            var actual = fileManager.Read(path);
            Assert.Equal(expected.Count(), actual.Count());

            var expectedArray = expected.ToArray();
            var actualArray = actual.ToArray();
            for (var i = 0; i < expectedArray.Length; i++)
            {
                Assert.Equal(expectedArray[i].SequenceId, actualArray[i].SequenceId);
                Assert.Equal(expectedArray[i].AggregateTypeId, actualArray[i].AggregateTypeId);
                Assert.Equal(expectedArray[i].MessageTypeId, actualArray[i].MessageTypeId);
                Assert.Equal(expectedArray[i].Timestamp, actualArray[i].Timestamp);
            }
            Teardown(path);
        }

        private void Teardown(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(path);
        }
    }
}
