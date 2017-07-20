using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    }
}
