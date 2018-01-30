namespace TinyPubSubLib.Subscriptions
{
    using System;

    public struct SubscriptionConfigurator<T>
    {
        internal SubscriptionConfigurator(Subscription<T> subscription)
        {
            this.Subscription = subscription;
        }

        internal Subscription<T> Subscription { get; set; }

        public SubscriptionConfigurator<T> Owner(object owner)
        {
            this.Subscription.Owner = owner;
            return this;
        }

        public SubscriptionConfigurator<T> RemoveAfterUse()
        {
            this.Subscription.RemoveAfterUse = true;
            return this;
        }

        public SubscriptionConfigurator<T> Tag(string tag)
        {
            this.Subscription.Tag = tag;
            return this;
        }

        public SubscriptionConfigurator<T> Action(Action action)
        {
            // Ensure we have only a single action
            this.Subscription.Action = action;
            this.Subscription.ActionWithArgument = null;
            this.Subscription.ActionWithArgumentAndArgs = null;
            return this;
        }

        public SubscriptionConfigurator<T> Action(Action<T> action)
        {
            // Ensure we have only a single action
            this.Subscription.Action = null;
            this.Subscription.ActionWithArgument = action;
            this.Subscription.ActionWithArgumentAndArgs = null;
            return this;
        }

        public SubscriptionConfigurator<T> Action(Action<T, TinyEventArgs> action)
        {
            // Ensure we have only a single action
            this.Subscription.Action = null;
            this.Subscription.ActionWithArgument = null;
            this.Subscription.ActionWithArgumentAndArgs = action;
            return this;
        }

        public SubscriptionConfigurator<T> WeakReference()
        {
            this.Subscription.Actions = new WeakReferenceSubscriptionActions<T>().UpdateWithValuesFrom(this.Subscription.Actions);
            return this;
        }
    }
}