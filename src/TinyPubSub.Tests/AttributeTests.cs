using System;
using System.Threading.Tasks;
using TinyPubSubLib;
using Xunit;

namespace TinyPubSub.Tests
{
    public class AttributeTests
    {
        [Fact]
        public void SubscribeWithAttributeTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test");

            // Assert
            Assert.True(subject.IsSuccessful);
        }

        [Fact]
        public void SubscribeAsyncWithAttributeTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test-async");

            // Assert
            Assert.True(subject.IsSuccessful);
        }

        [Fact]
        public void SubscribeWithParameterTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            var data = new TestType() { DuckLength = 42 };
            TinyPubSubLib.TinyPubSub.Publish("test-with-arguments", data);

            // Assert
            Assert.True(subject.IsSuccessful);
        }

        [Fact]
        public void SubscribeWithWrongParameterTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            var data = new BadTestType();
            TinyPubSubLib.TinyPubSub.Publish("test-with-arguments", data, OnError: (Exception arg1, ISubscription arg2) => subject.IsSuccessful = true);

            // Assert
            Assert.True(subject.IsSuccessful);
        }
    }

    public class TestSubject
    {
        public bool IsSuccessful
        {
            get;
            set;
        }

        [TinySubscribe("test")]
        public void DoEpicStuff()
        {
            IsSuccessful = true;
        }

        [TinySubscribe("test-async")]
        public async Task DoEpicAsyncStuff()
        {
            await Task.Delay(1);
            IsSuccessful = true;
        }

        [TinySubscribe("test-with-arguments")]
        public void DoEpicStuffWithArgument(TestType data)
        {
            if (data.DuckLength == 42)
            {
                IsSuccessful = true;
            }
        }
    }

    public class TestType
    {
        public string DuckSize { get; set; }
        public int DuckLength { get; set; }
    }

    public class BadTestType
    {

    }
}
