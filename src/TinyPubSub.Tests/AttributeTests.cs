using System;
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
    }
}
