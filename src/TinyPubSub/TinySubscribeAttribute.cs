using System;
namespace TinyPubSubLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public class TinySubscribeAttribute : Attribute
    {
        public string Channel
        {
            get;
            set;
        }

        public TinySubscribeAttribute(string channel)
        {
            Channel = channel;
        }
    }
}
