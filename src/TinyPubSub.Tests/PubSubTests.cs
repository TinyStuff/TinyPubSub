using System;
using System.Threading.Tasks;
using Xunit;

namespace TinyPubSub.Tests
{
    // Dummy class for instance sending
    public class TestEventType {
        public int Sklep { get; set; } = 5;
    }

    // Dummy class for instance sending with inheritance
    public class InheritedTestEventType : TestEventType {
        public int MyOtherInt { get; set; } = 2;
    }

    public class PubSubTests
    {
		public PubSubTests()
		{
			TinyPubSubLib.TinyPubSub.Clear();
		}

		[Fact]
		public void SubscribeWithArgumentTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = (x == "duck"));

			// Act
			TinyPubSubLib.TinyPubSub.Publish("test", "duck");

			// Assert
            Assert.True(testsuccessful);
		}

        [Fact]
        public void SubscribeWithTypeTest()
        {
            // Arrange
            var testsuccessful = false;
            var tstType = new TestEventType();
            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>("test", (x) => testsuccessful = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test", tstType);

            // Assert
            Assert.True(testsuccessful);
        }

        [Fact]
        public void SubscribeWithTypeInheritanceTest()
        {
            // Arrange
            var testsuccessful = false;
            var tstType = new InheritedTestEventType();
            TinyPubSubLib.TinyPubSub.Subscribe<TestEventType>("test", (x) => testsuccessful = (x.Sklep == 5));

            // Act
            TinyPubSubLib.TinyPubSub.Publish<TestEventType>("test", tstType);

            // Assert
            Assert.True(testsuccessful);
        }

		[Fact]
		public void SubscribeWithArgumentMissingTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

			// Act
			TinyPubSubLib.TinyPubSub.Publish("test", "dumbargument");

			// Assert
			Assert.True(testsuccessful);
		}

		[Fact]
		public void SubscribeWithArgumentMissingButArgumentedSubscriptionTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = (x == null));

			// Act
			TinyPubSubLib.TinyPubSub.Publish("test");

			// Assert
			Assert.True(testsuccessful);
		}

		[Fact]
		public void SubscribePublishTheMostCommonWayTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

			// Act
			TinyPubSubLib.TinyPubSub.Publish("test");

			// Assert
			Assert.True(testsuccessful);
		}

        [Fact]
        public void SubscribePublishExceptionHandling()
        {
            // Arrange
            var testsuccessful = false;

            var subId = TinyPubSubLib.TinyPubSub.Subscribe("testerror", () => throw new Exception("Error in handling") );
            TinyPubSubLib.TinyPubSub.Subscribe<TinyPubSubLib.TinyException>(TinyPubSubLib.TinyException.DefaultChannel, (msg) => testsuccessful = msg.SubscriptionTag==subId);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("testerror");

            // Assert
            Assert.True(testsuccessful);
        }

		[Fact]
		public async Task DelayedSubscribePublishTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

			// Act
			TinyPubSubLib.TinyPubSub.PublishAsTask("test");
			await Task.Delay(100);

			// Assert
			Assert.True(testsuccessful);
		}

		[Fact]
		public void DelayedSubscribePublishNotWaitingForCompletionTest()
		{
			// Arrange
			var testsuccessful = true;
			TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = false);

			// Act
			TinyPubSubLib.TinyPubSub.PublishAsTask("test");

			// Assert
			Assert.True(testsuccessful);
		}

		[Fact]
		public async Task DelayedSubscribePublishWithArgumentsTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = x == "duck");

			// Act
			TinyPubSubLib.TinyPubSub.PublishAsTask("test", "duck");
			await Task.Delay(100);

			// Assert
			Assert.True(testsuccessful);
		}

		[Fact]
		public async Task PublishAsyncTest()
		{
			// Arrange
			var testsuccessful = false;
			TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

			// Act
			await TinyPubSubLib.TinyPubSub.PublishAsync("test");

			// Assert
			Assert.True(testsuccessful);
		}
    }
}
