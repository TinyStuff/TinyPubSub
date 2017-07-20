using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace TinyPubSub.Tests
{
    [TestClass]
    public class PubSubTests
    {
        [TestInitialize]
        public void InitializeTests()
        {
            TinyPubSubLib.TinyPubSub.Clear();
        }

        [TestMethod]
        public void SubscribeWithArgumentTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = (x == "duck"));

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test", "duck");

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public void SubscribeWithArgumentMissingTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test", "dumbargument");

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public void SubscribeWithArgumentMissingButArgumentedSubscriptionTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = (x == null));

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test");

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public void SubscribePublishTheMostCommonWayTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.Publish("test");

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public async Task DelayedSubscribePublishTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask("test");
            await Task.Delay(100);

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public void DelayedSubscribePublishNotWaitingForCompletionTest()
        {
            // Arrange
            var testsuccessful = true;
            TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = false);

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask("test");

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public async Task DelayedSubscribePublishWithArgumentsTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", (x) => testsuccessful = x == "duck");

            // Act
            TinyPubSubLib.TinyPubSub.PublishAsTask("test", "duck");
            await Task.Delay(100);

            // Assert
            Assert.IsTrue(testsuccessful);
        }

        [TestMethod]
        public async Task PublishAsyncTest()
        {
            // Arrange
            var testsuccessful = false;
            TinyPubSubLib.TinyPubSub.Subscribe("test", () => testsuccessful = true);

            // Act
            await TinyPubSubLib.TinyPubSub.PublishAsync("test");

            // Assert
            Assert.IsTrue(testsuccessful);
        }
    }
}
