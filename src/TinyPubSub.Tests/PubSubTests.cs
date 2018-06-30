using System;
using System.Threading.Tasks;
using Xunit;
using TinyPubSubLib;

namespace TinyPubSub.Tests
{
    // Dummy class for instance sending
    public class TestEventType
    {
        public int Sklep { get; set; } = 5;
    }

    // Dummy class for instance sending with inheritance
    public class InheritedTestEventType : TestEventType
    {
        public int MyOtherInt { get; set; } = 2;
    }

    public class PubSubTests
    {
        [Fact]
        public void SubscribeWithArgumentTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => testsuccessful = (x == "duck"));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, "duck");

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeWithTypeTest()
        {
            // Arrange
            var testsuccessful = false;
            var tstType = new TestEventType();
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(channel, (x) => testsuccessful = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, tstType);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeWithEventArguments()
        {
            // Arrange
            var testsuccessful = 0;
            var tstType = new TestEventType();
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x) =>
            {
                testsuccessful = 3;
            });

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x, TinyPubSubLib.TinyEventArgs evtargs) =>
            {
                evtargs.HaltExecution = true;
            });

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x) =>
            {
                testsuccessful = 4;
            });

            // Act
            var ret = TinyPubSubLib.TinyPubSub.PublishControlled(channel, tstType);

            // Assert
            Assert.True(testsuccessful == 3 && ret.Handled == true);
        }

        [Fact]
        public void SubscribeWithTypeInheritanceTest()
        {
            // Arrange
            var testsuccessful = false;
            var tstType = new InheritedTestEventType();
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(channel, (x) => testsuccessful = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish<TestEventType>(channel, tstType);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeWithArgumentMissingTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, "dumbargument");

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeWithArgumentMissingButArgumentedSubscriptionTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => testsuccessful = (x == null));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribePublishTheMostCommonWayTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribePublishExceptionHandling()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            var subId = TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception("Error in handling"));
            TinyPubSubLib.TinyPubSub.Subscribe<TinyPubSubLib.TinyException>(TinyPubSubLib.TinyException.DefaultChannel, (msg) => testsuccessful = msg.SubscriptionTag == subId);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public async Task DelayedSubscribePublishTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask(channel);
            await Task.Delay(100);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void DelayedSubscribePublishNotWaitingForCompletionTest()
        {
            // Arrange
            var testsuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = false);

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public async Task DelayedSubscribePublishWithArgumentsTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => testsuccessful = x == "duck");

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask(channel, "duck");
            await Task.Delay(100);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public async Task PublishAsyncTest()
        {
            // Arrange
            var testsuccessful = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = true);

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public async Task PublishAsyncWithExceptionTest()
        {
            // Arrange
            var testsuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception());

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public async Task PublishWithOnErrorActionTest()
        {
            // Arrange
            var testsuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception("Boom"));

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel, OnError: (ex, s) => testsuccessful = ex.Message == "Boom");

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void PublishWithEventArgsTest()
        {
            // Arrange
            var testsuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (string argument, TinyEventArgs args) => args.HaltExecution = true);
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => testsuccessful = false); // This subscription should never be called

            // Act
            TinyPubSubLib.TinyPubSub.PublishControlled(channel);

            // Assert
            Assert.True(testsuccessful);
        }
    }
}
