using System;
using System.Collections.Generic;
using System.Linq;
using Business;
using EventSource;
using Xunit;

namespace EventSourceTest
{
    public class EventGeneratorTest
    {
        [Fact]
        public void Get_HasContent()
        {
            const int limit = 200;
            var generator = new EventGenerator();
            var actual = generator.Get(limit).ToList();
            Assert.NotEmpty(actual);
            Assert.Equal(limit, actual.Count);
        }

        [Fact]
        public void Get_VerifySequenceId()
        {
            const int limit = 200;
            var generator = new EventGenerator();
            var actual = generator.Get(limit);

            var i = 1;
            foreach (var item in actual)
            {
                Assert.Equal(i++, item.SequenceId);
            }
        }

        [Fact]
        public void Get_VerifyAggregateTypeId()
        {
            const int limit = 200;
            var generator = new EventGenerator();
            var actual = generator.Get(limit);

            actual.AsParallel().ForAll(e =>
            {
                Assert.Equal(e.GetType() == typeof(CustomerCreatedEvent) ? 11 : 12, e.AggregateTypeId);
            });
        }

        [Fact]
        public void Get_VerifyMessageTypeId()
        {
            const int limit = 200;
            var generator = new EventGenerator();
            var actual = generator.Get(limit);

            actual.AsParallel().ForAll(e =>
            {
                var actualId = e.MessageTypeId;
                if (e.GetType() == typeof(CustomerCreatedEvent))
                {
                    var expectedIds = new List<int> { 1, 16 };
                    Assert.True(expectedIds.Contains(actualId));
                }
                else
                {
                    var expectedIds = new List<int> { 83, 84, 85, 87, 89, 92 };
                    Assert.True(expectedIds.Contains(actualId));
                }
            });
        }

        [Fact]
        public void Get_VerifyTimestamp()
        {
            const int limit = 200;
            var generator = new EventGenerator();
            var actual = generator.Get(limit);
            var now = DateTimeOffset.Now;

            actual.AsParallel().ForAll(e =>
            {
                var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(e.Timestamp);
                Assert.Equal(now.Year, timestamp.Year);
                Assert.Equal(now.Month, timestamp.Month);
                Assert.Equal(now.Day, timestamp.Day);
            });
        }
    }
}
