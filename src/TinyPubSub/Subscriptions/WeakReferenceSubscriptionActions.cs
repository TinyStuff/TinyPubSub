// ReSharper disable StyleCop.SA1126 - R# reporting an invalid error
namespace TinyPubSubLib.Subscriptions
{
    using System;

    internal class WeakReferenceSubscriptionActions<T> : SubscriptionActions<T>
    {
        private WeakReference<Action> weakActionReference;

        private WeakReference<Action<T, TinyEventArgs>> weakActionWithArgumentAndArgsReference;

        private WeakReference<Action<T>> weakActionWithArgumentReference;

        public override Action Action
        {
            get => this.weakActionReference.TryGetTarget(out var action) ? action : null;
            set => this.weakActionReference = new WeakReference<Action>(value);
        }

        public override Action<T> ActionWithArgument
        {
            get => this.weakActionWithArgumentReference.TryGetTarget(out var action) ? action : null;
            set => this.weakActionWithArgumentReference = new WeakReference<Action<T>>(value);
        }

        public override Action<T, TinyEventArgs> ActionWithArgumentAndArgs
        {
            get => this.weakActionWithArgumentAndArgsReference.TryGetTarget(out var action) ? action : null;
            set => this.weakActionWithArgumentAndArgsReference = new WeakReference<Action<T, TinyEventArgs>>(value);
        }
    }
}