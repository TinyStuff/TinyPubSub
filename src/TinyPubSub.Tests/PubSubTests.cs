using System;
using System.Threading.Tasks;
using FluentAssertions;
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
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => actionCalled = (x == "duck"));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, "duck");

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribeWithTypeTest()
        {
            // Arrange
            var actionCalled = false;
            var tstType = new TestEventType();
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(channel, (x) => actionCalled = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, tstType);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribeWithEventArguments()
        {
            // Arrange
            var actionCalled = 0;
            var tstType = new TestEventType();
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x) =>
            {
                actionCalled = 3;
            });

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x, TinyPubSubLib.TinyEventArgs evtargs) =>
            {
                evtargs.HaltExecution = true;
            });

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(null, channel, (TestEventType x) =>
            {
                actionCalled = 4;
            });

            // Act
            var ret = TinyPubSubLib.TinyPubSub.PublishControlled(channel, tstType);

            // Assert
            Assert.True(actionCalled == 3 && ret.Handled == true);
        }

        [Fact]
        public void SubscribeWithTypeInheritanceTest()
        {
            // Arrange
            var actionCalled = false;
            var tstType = new InheritedTestEventType();
            var channel = Guid.NewGuid().ToString();

            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(channel, (x) => actionCalled = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish<TestEventType>(channel, tstType);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribeWithArgumentMissingTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => actionCalled = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, "dumbargument");

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribeWithArgumentMissingButArgumentedSubscriptionTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => actionCalled = (x == null));

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribePublishTheMostCommonWayTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => actionCalled = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void SubscribePublishExceptionHandling()
        {
            // Arrange
            var wasSuccessful = false;
            var channel = Guid.NewGuid().ToString();
            var subId = TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception("Error in handling"));
            TinyPubSubLib.TinyPubSub.Subscribe<TinyException>(TinyException.DefaultChannel, (msg) => wasSuccessful = msg.SubscriptionTag == subId);

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel);

            // Assert
            Assert.True(wasSuccessful);
        }

        [Fact]
        public async Task DelayedSubscribePublishTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => actionCalled = true);

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsFireAndForgetTask(channel);
            await Task.Delay(100);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public async Task DelayedSubscribePublishWithArgumentsTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (x) => actionCalled = x == "duck");

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsFireAndForgetTask(channel, "duck");
            await Task.Delay(100);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public async Task PublishAsyncTest()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => actionCalled = true);

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel);

            // Assert
            Assert.True(actionCalled);
        }
        
        [Fact]
        public void ShouldThrowExceptionIfNoChannelWasProvidedWhenSubscribing()
        {
            // Arrange
            Func<string> act = () => TinyPubSubLib.TinyPubSub.Subscribe(null, () => { });

            // Act
            act.Should().ThrowExactly<ArgumentException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionIfNoChannelWasProvidedWhenPublishing()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => actionCalled = true);
            Func<TinyEventArgs> act = () => TinyPubSubLib.TinyPubSub.PublishControlled(null);
            
            // Act
            act.Should().ThrowExactly<ArgumentException>();
            actionCalled.Should().BeFalse();
        }

        [Fact]
        public async Task PublishAsyncWithExceptionTest()
        {
            // Arrange
            var wasSuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception());

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel);

            // Assert
            Assert.True(wasSuccessful);
        }

        [Fact]
        public async Task PublishWithOnErrorActionTest()
        {
            // Arrange
            var wasSuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => throw new Exception("Boom"));

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync(channel, onError: (ex, s) => wasSuccessful = ex.Message == "Boom");

            // Assert
            Assert.True(wasSuccessful);
        }

        [Fact]
        public void PublishWithEventArgsTest()
        {
            // Arrange
            var wasSuccessful = true;
            var channel = Guid.NewGuid().ToString();
            TinyPubSubLib.TinyPubSub.Subscribe(channel, (_, args) => args.HaltExecution = true);
            TinyPubSubLib.TinyPubSub.Subscribe(channel, () => wasSuccessful = false); // This subscription should never be called

            // Act
            TinyPubSubLib.TinyPubSub.PublishControlled(channel);

            // Assert
            Assert.True(wasSuccessful);
        }

        [Fact]
        public void ShouldHandleSubscriptionWithOwner()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            var owner = new object();
            var tag = TinyPubSubLib.TinyPubSub.Subscribe(owner, channel, x => actionCalled = x == "duck");

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, "duck");

            // Assert
            actionCalled.Should().BeTrue();
            tag.Should().NotBeNull();
        }
        
        [Fact]
        public void ShouldHandleSubscriptionForTypeWithReturningEventArgs()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            var tag = TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>(channel, (x, args) =>
            {
                actionCalled = x.Sklep == 42;
                args.HaltExecution = true;
            });

            // Act
            TinyPubSubLib.TinyPubSub.Publish(channel, new TestEventType
                { Sklep = 42});

            // Assert
            actionCalled.Should().BeTrue();
            tag.Should().NotBeNull();
        }
        
        [Fact]
        public async Task ShouldPublishControlledAsyncWithoutExceptionHandlingAction()
        {
            // Arrange
            var actionCalled = false;
            var channel = Guid.NewGuid().ToString();
            var tag = TinyPubSubLib.TinyPubSub.Subscribe(channel, x =>
            {
                actionCalled = x == "Duck";
            });

            // Act
            await TinyPubSubLib.TinyPubSub.PublishControlledAsync(channel, "Duck");
            await Task.Delay(100);

            // Assert
            actionCalled.Should().BeTrue();
            tag.Should().NotBeNull();
        }
        
        [Fact]
        public async Task ShouldPublishControlledAsyncWithExceptionHandlingAction()
        {
            // Arrange
            var onErrorWasCalled = false;
            var channel = Guid.NewGuid().ToString();
            var tag = TinyPubSubLib.TinyPubSub.Subscribe(channel, x => throw new Exception("Something went bad"));

            // Act
            await TinyPubSubLib.TinyPubSub.PublishControlledAsync(channel, "Duck", (exception, subscription) => onErrorWasCalled = true);
            await Task.Delay(100);

            // Assert
            onErrorWasCalled.Should().BeTrue();
            tag.Should().NotBeNull();
        }
    }
}
