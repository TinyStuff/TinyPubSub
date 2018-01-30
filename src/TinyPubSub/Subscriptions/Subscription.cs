/*
 * The MIT License (MIT)
 * Copyright (c) 2016 Johan Karlsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished 
 * to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */

namespace TinyPubSubLib.Subscriptions
{
    using System;

    /// <summary>
    ///     Represents one subscription
    /// </summary>
    /// <typeparam name="T">
    ///     The subscription parameter type
    /// </typeparam>
    internal class Subscription<T> : ISubscription
    {
        private SubscriptionActions<T> actions;

        public Subscription(Action action)
            : this()
        {
            this.Action = action;
        }

        public Subscription(Action<T> action)
            : this()
        {
            this.ActionWithArgument = action;
            this.SubscribeToType = typeof(T);
        }

        public Subscription(object owner, Action action, bool removeAfterUse = false)
            : this(action)
        {
            this.Owner = owner;
            this.RemoveAfterUse = removeAfterUse;
        }

        public Subscription(object owner, Action<T> action, bool removeAfterUse = false)
            : this(action)
        {
            this.RemoveAfterUse = removeAfterUse;
            this.Owner = owner;
        }

        public Subscription(Action<T, TinyEventArgs> action)
            : this()
        {
            this.ActionWithArgumentAndArgs = action;
        }

        public Subscription(object owner, Action<T, TinyEventArgs> action, bool removeAfterUse = false)
            : this(action)
        {
            this.Owner = owner;
            this.RemoveAfterUse = removeAfterUse;
        }

        internal Subscription(SubscriptionActions<T> subscriptionActions = null)
        {
            this.Tag = Guid.NewGuid().ToString();
            this.SubscribeToType = typeof(T);
            this.actions = subscriptionActions ?? new SubscriptionActions<T>();
        }

        public Action Action
        {
            get => this.actions.Action;
            set => this.actions.Action = value;
        }

        public Action<T> ActionWithArgument
        {
            get => this.actions.ActionWithArgument;
            set => this.actions.ActionWithArgument = value;
        }

        public Action<T, TinyEventArgs> ActionWithArgumentAndArgs
        {
            get => this.actions.ActionWithArgumentAndArgs;
            set => this.actions.ActionWithArgumentAndArgs = value;
        }

        public object Owner { get; set; }

        public bool RemoveAfterUse { get; set; }

        public Type SubscribeToType { get; set; }

        public string Tag { get; set; }

        Action<object> ISubscription.ActionWithArgument => this.ActionWithArgument as Action<object>;

        internal SubscriptionActions<T> Actions
        {
            get => this.actions;
            set => this.actions = value;
        }

        public void Invoke(object instance, TinyEventArgs returnEventArgs)
        {
            if (instance is T typedInstance)
            {
                this.Invoke(typedInstance, returnEventArgs);
            }
        }

        public void Invoke(T instance, TinyEventArgs returnEventArgs)
        {
            // EB: Moved to a local variable to prevent ActionWithArgumentAndArgs being changed to NULL between the check for NULL and the actuall method call
            var actionWithArgumentAndArgs = this.ActionWithArgumentAndArgs;

            if (actionWithArgumentAndArgs != null)
            {
                actionWithArgumentAndArgs.Invoke(instance, returnEventArgs);
                return;
            }

            var action = this.Action;
            if (action != null)
            {
                action.Invoke();
                returnEventArgs.Handled = true;
                return;
            }

            var actionWithArgument = this.ActionWithArgument;
            if (actionWithArgument != null)
            {
                actionWithArgument.Invoke(instance);
                returnEventArgs.Handled = true;
                return;
            }

            // Now, it seems the subscription does not have a referenced event handler, make sure the subscription is removed
            this.RemoveAfterUse = true;
        }
    }
}