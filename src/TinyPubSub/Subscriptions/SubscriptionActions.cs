namespace TinyPubSubLib.Subscriptions
{
    using System;

    internal class SubscriptionActions<T>
    {
        public virtual Action Action { get; set; }

        public virtual Action<T> ActionWithArgument { get; set; }

        public virtual Action<T, TinyEventArgs> ActionWithArgumentAndArgs { get; set; }

        public SubscriptionActions<T> UpdateWithValuesFrom(SubscriptionActions<T> other)
        {
            if (other != null)
            {
                this.Action = other.Action;
                this.ActionWithArgument = other.ActionWithArgument;
                this.ActionWithArgumentAndArgs = other.ActionWithArgumentAndArgs;
            }

            return this;
        }
    }
}