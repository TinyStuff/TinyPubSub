using System;

namespace TinyPubSubLib
{
    public class TinyEventArgs
    {
        public bool Handled { get; set; }
        public bool HaltExecution { get; set; }
        public object StateObject { get; set; }
    }
}
