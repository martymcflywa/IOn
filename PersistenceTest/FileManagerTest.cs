using System;
using System.IO;
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
            using (var fileManager = new FileManager(path, maxSize))
            {
                fileManager.Write(events);
            }

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
