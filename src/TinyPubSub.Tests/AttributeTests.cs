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
        public async Task SubscribeAsyncWithAttributeTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test-async");
            await Task.Delay(100);

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
            TinyPubSubLib.TinyPubSub.Publish("test-with-arguments", data, onError: (Exception arg1, ISubscription arg2) => Console.WriteLine($"Exception occured: {arg1.ToString()}"));

            // Assert
            Assert.True(subject.IsSuccessful);
        }

        /// <summary>
        /// Register subcriptions through attribute but the publish calls data doesn't match the 
        /// signature of the subscription method. At the moment, this results in an error.
        /// </summary>
        [Fact]
        public void SubscribeWithWrongParameterTest()
        {
            // Arrange
            var subject = new TestSubject();
            TinyPubSubLib.TinyPubSub.Register(subject);

            // Act
            var data = new BadTestType();
            TinyPubSubLib.TinyPubSub.Publish("test-with-bad-arguments", data, onError: (Exception arg1, ISubscription arg2) => subject.IsSuccessful = true);

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

        /// <summary>
        /// This method will only be called if the Publish passes
        /// TestType as argument.
        /// </summary>
        /// <param name="data">Data.</param>
        [TinySubscribe("test-with-arguments")]
        public void DoEpicStuffWithArgument(TestType data)
        {
            if (data.DuckLength == 42)
            {
                IsSuccessful = true;
            }
        }

        /// <summary>
        /// This method will only be called if the Publish passes
        /// TestType as argument.
        /// </summary>
        /// <param name="data">Data.</param>
        [TinySubscribe("test-with-bad-arguments")]
        public void DoEpicStuffWithBadArgument(TestType data)
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
