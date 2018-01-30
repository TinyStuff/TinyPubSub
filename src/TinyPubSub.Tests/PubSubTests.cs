using System;
using System.Threading.Tasks;
using Xunit;

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
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel, onError: (ex, s) => testsuccessful = ex.Message == "Boom");

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeToTest()
        {
            // Arrange
            var testValue = 0;
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.SubscribeTo<int>(
                channel,
                s => s
                    .Owner(this)
                    .Tag("ABC123")
                    .Action((v, a) =>
                    {
                        testValue = v;
                        a.HaltExecution = true;
                    }));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, 1234);

            // Assert
            Assert.Equal(1234, testValue);
        }

        private class ReferenceTest<T>
        {
            public ReferenceTest(Action<T> action)
            {
                this.Action = action;
            }

            public Action<T> Action { get; }

            public void DoIt(T value)
            {
                this.Action(value);
            }
        }

        [Fact]
        public void SubscribeToStrongReferenceTest()
        {
            // First test strong reference

            // Arrange
            var testValue = 0;
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.SubscribeTo<int>(
                channel,
                s => s
                    .Owner(this)
                    .Tag("ABC123")
                    .Action(new ReferenceTest<int>(v => testValue = v).DoIt));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, 1234);

            // Assert - ensure the reference works before GC
            Assert.Equal(1234, testValue);

            GC.Collect(2, GCCollectionMode.Forced, true, true);

            // Go again
            TinyPubSubLib.TinyPubSub.Publish(channel, 4321);
            Assert.Equal(4321, testValue);

        }

        [Fact]
        public void SubscribeToWeakReferenceTest()
        {
            // First test strong reference

            // Arrange
            var testValue = 0;
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.SubscribeTo<int>(
                channel,
                s => s
                    .Owner(this)
                    .Tag("ABC123")
                    .WeakReference()
                    .Action(new ReferenceTest<int>(v => testValue = v).DoIt));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, 1234);

            // Assert - ensure the reference works before GC
            Assert.Equal(1234, testValue);

            GC.Collect(2, GCCollectionMode.Forced, true, true);

            // Go again, but the value should not have been updated, since the ref obj is GC:ed
            TinyPubSubLib.TinyPubSub.Publish(channel, 4321);
            Assert.Equal(1234, testValue);
        }
    }
}
