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

using System;

namespace TinyPubSubLib
{
    /// <summary>
    /// Represents one subscription
    /// </summary>
    internal class Subscription<T> : ISubscription
    {
        /// <summary>
        /// Order of executing subscriptions.
        /// </summary>
        /// <remarks>Since a user can halt execution of subscriptions, the order in which they
        /// are added matters. It was a List&lt;T&gt; earlier, now i't a Dictionary&lt;T&gt; which
        /// does not guarantee the order when enumerating. So we need to sort.</remarks>
        public int Order { get; set; }
        
        public Action Action { get; set; }

        public Action<T> ActionWithArgument { get; set; }

        public Action<T, TinyEventArgs> ActionWithArgumentAndArgs { get; set; }

        Action<object> ISubscription.ActionWithArgument => ActionWithArgument as Action<object>;

        public string Tag { get; set; }

        public Type SubscribeToType { get; set; }

        public WeakReference Owner { get; set; }

        public bool RemoveAfterUse { get; set; }

        internal Subscription()
        {
            Tag = Guid.NewGuid().ToString();
            SubscribeToType = typeof(T);
        }

        public Subscription(Action action, bool removeAfterUse = false, int order = 0) : this()
        {
            Action = action;
            RemoveAfterUse = removeAfterUse;
            Order = order;
        }

        public Subscription(Action<T> action, bool removeAfterUse = false, int order = 0) : this()
        {
            ActionWithArgument = action;
            SubscribeToType = typeof(T);
            RemoveAfterUse = removeAfterUse;
            Order = order;
        }

        public Subscription(object owner, Action action, bool removeAfterUse = false, int order = 0) : this(action)
        {
            Owner = new WeakReference(owner);
            RemoveAfterUse = removeAfterUse;
            Order = order;
        }

        public Subscription(object owner, Action<T> action, bool removeAfterUse = false, int order = 0) : this(action)
        {
            RemoveAfterUse = removeAfterUse;
            Owner = new WeakReference(owner);
            Order = order;
        }

        public Subscription(Action<T, TinyEventArgs> action, bool removeAfterUse = false, int order = 0) : this()
        {
            ActionWithArgumentAndArgs = action;
            RemoveAfterUse = removeAfterUse;
            Order = order;
        }

        public Subscription(object owner, Action<T, TinyEventArgs> action, bool removeAfterUse = false, int order = 0) : this(action)
        {
            Owner = new WeakReference(owner);
            RemoveAfterUse = removeAfterUse;
            Order = order;
        }
    }
}
