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

using System.Threading;

namespace TinyPubSubLib
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public static class TinyPubSub
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<ISubscription, ISubscription>> Channels = new ConcurrentDictionary<string, ConcurrentDictionary<ISubscription, ISubscription>>();
        private static int order;

        private static object genericOwner = new {};
        
        private static ConcurrentDictionary<ISubscription, ISubscription> GetOrCreateChannel(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("You have to provide a channel name");
            }
            
            return Channels.GetOrAdd(channel, key => new ConcurrentDictionary<ISubscription, ISubscription>());
        }

        private static ISubscription CreateAndAdd(object owner, string channel, Action action, bool disableAfterFirstUse = false)
        {
            Interlocked.Increment(ref order);
            
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<string>(owner, action, disableAfterFirstUse, order);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        private static ISubscription CreateAndAdd<T>(object owner, string channel, Action<T> action, bool disableAfterFirstUse = false)
        {
            Interlocked.Increment(ref order);

            if (owner == null)
            {
                owner = genericOwner;
            }

            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action, disableAfterFirstUse, order);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        private static ISubscription CreateAndAdd<T>(object owner, string channel, Action<T, TinyEventArgs> action, bool disableAfterFirstUse = false)
        {
            Interlocked.Increment(ref order);

            if(owner == null)
            {
                owner = genericOwner;
            }

            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action, disableAfterFirstUse, order);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        /// <summary>
        /// Occurs when on subscription removed.
        /// </summary>
        public static event EventHandler<ISubscription> OnSubscriptionRemoved;

        /// <summary>
        /// Subscribe to a channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>A tag that can be used to unsubscribe.</returns>
        public static string Subscribe(string channel, Action action)
        {
            var subscription = CreateAndAdd(genericOwner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>A tag that can be used to unsubscribe.</returns>
        public static string Subscribe(string channel, Action<string> action)
        {
            var subscription = CreateAndAdd<string>(genericOwner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel.
        /// </summary>
        /// <param name="owner">The owner of the subscription.</param> 
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>A tag that can be used to unsubscribe.</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubscribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action action)
        {
            var subscription = CreateAndAdd(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument.
        /// </summary>
        /// <param name="owner">The owner of the subscription.</param> 
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>A tag that can be used to unsubscribe.</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubscribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action<string> action)
        {
            var subscription = CreateAndAdd(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument.
        /// </summary>
        /// <returns>The subscription tag.</returns>
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T> action)
        {
            var subscription = CreateAndAdd<T>(genericOwner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribes to a channel with an argument and control flow event.
        /// </summary>
        /// <returns>The subscription tag.</returns>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="action">The action to execute.</param>
        public static string Subscribe(string channel, Action<string, TinyEventArgs> action)
        {
            var subscription = CreateAndAdd(Channels, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner.
        /// </summary>
        /// <returns>The subscription tag</returns>
        /// /// <param name="owner">The owner of the subscription.</param> 
        /// <param name="channel">The channel name.</param>
        /// <param name="action">The action to run.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T> action)
        {
            var subscription = CreateAndAdd<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancel execution and specify if the event has been handled.
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="owner">The owner of the subscription - used for automatic deregistration.</param>
        /// <param name="channel">The channel to subscribe to.</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publish return.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateAndAdd<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancel execution and specify if the event has been handled.
        /// </summary>
        /// <returns>The subscription tag</returns>
        /// <param name="channel">The channel to subscribe to</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publish return.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateAndAdd<T>(genericOwner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Unsubscribes to a channel based on a tag
        /// </summary>
        /// <param name="tag"></param>
        public static void Unsubscribe(string tag)
        {
            foreach (var channel in Channels.Values)
            {
                foreach (var subscription in channel.Keys.Where(subscription => subscription.Tag == tag))
                {
                    channel.TryRemove(subscription, out var _);
                    OnSubscriptionRemoved?.Invoke(null, subscription);
                }
            }
        }

        /// <summary>
        /// Unsubscribes to a channel based on an object owner
        /// </summary>
        /// <param name="owner">The owner object</param>
		public static void Unsubscribe(object owner)
        {
            if (owner == null)
            {
                return;
            }

            foreach (var channel in Channels.Values)
            {
                var subscriptionsToRemove = new List<ISubscription>();

                foreach (var subscription in channel.Keys)
                {
                    if(subscription.Owner.Target == null)
                    {
                        subscriptionsToRemove.Add(subscription);
                    }
                    else if (subscription.Owner.Target == owner)
                    {
                        channel.TryRemove(subscription, out var _);
                        OnSubscriptionRemoved?.Invoke(owner, subscription);
                    }
                }

                foreach(var subscription in subscriptionsToRemove)
                {
                    channel.TryRemove(subscription,out _);
                }
            }
        }

        /// <summary>
        /// Publish an event to the specified channel with instance argument.
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance of an object to pass to the receiver. Think argument.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Publish<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            PublishControlled<T>(channel, instance, onError);
        }

        /// <summary>
        /// Publish a controlled event to the specified channel with instance argument and returns when the event is handled.
        /// </summary>
        /// <returns>The result of the call.</returns>
        /// <param name="channel">The channel name.</param>
        /// <param name="instance">Instance of an object to pass to the receiver. Think argument.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        public static TinyEventArgs PublishControlled(string channel, string instance = default(string), Action<Exception, ISubscription> onError = null)
        {
            return PublishControlled<string>(channel, instance, onError);
        }

        /// <summary>
        /// Publish a controlled event to the specified channel with instance argument and returns when the event is handled.
        /// </summary>
        /// <returns>The result of the call.</returns>
        /// <param name="channel">The channel name.</param>
        /// <param name="argument">The argument to pass.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        public static async Task<TinyEventArgs> PublishControlledAsync(string channel, string argument = default, Action<Exception, ISubscription> onError = null)
        {
            return await Task.Run(() => PublishControlled(channel, argument, onError)).ConfigureAwait(false);
        }

        /// <summary>
        /// Publish a controlled event to the specified channel with instance argument and returns when the event is handled.
        /// </summary>
        /// <returns>The result of the call</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance of an object to pass to the receiver. Think argument.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<TinyEventArgs> PublishControlledAsync<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            return await Task.Run(() => PublishControlled(channel, instance, onError)).ConfigureAwait(false);
        }

        /// <summary>
        /// Publish an event to the specified channel with instance argument and returns when the event is handled.
        /// </summary>
        /// <returns>The result of the call</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance of an object to pass to the receiver. Think argument.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static TinyEventArgs PublishControlled<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("You have to specify a channel to publish to");
            }

            var returnEventArgs = new TinyEventArgs();

            if (!Channels.TryGetValue(channel, out var current))
            {
                return returnEventArgs;
            }
            
            var subscriptionsToRemove = new List<ISubscription>();

            foreach (var subscription in current.Keys.OfType<Subscription<T>>().OrderBy(s => s.Order))
            {
                try
                {
                    if (subscription.Owner.Target == null)
                    {
                        subscriptionsToRemove.Add(subscription);
                        continue;
                    }
                        
                    if (subscription.ActionWithArgumentAndArgs != null)
                    {
                        subscription.ActionWithArgumentAndArgs.Invoke(instance, returnEventArgs);
                    }
                    else
                    {
                        var hasBeenHandled = false;

                        if (subscription.Action != null)
                        {
                            subscription.Action.Invoke();
                            hasBeenHandled = true;
                        }
                        else if (subscription.ActionWithArgument != null)
                        {
                            subscription.ActionWithArgument.Invoke(instance);
                            hasBeenHandled = true;
                        }

                        returnEventArgs.Handled = hasBeenHandled;
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, subscription);
                    SendException(ex, subscription.Tag);
                }

                if (subscription.RemoveAfterUse)
                {
                    Unsubscribe(subscription.Tag);
                }

                if (returnEventArgs.HaltExecution)
                {
                    return returnEventArgs;
                }
            }

            foreach(var subscription in subscriptionsToRemove)
            {
                current.TryRemove(subscription, out _);
            }

            // Concept code - fall back to calling each with object
            // this is the way we need to do it for allowing attribute
            // subscription.
            if (typeof(T) != typeof(object))
            {
                PublishControlled<object>(channel, instance, onError);
            }

            return returnEventArgs;
        }

        private static void SendException(Exception ex, string tag)
        {
            var message = new TinyException()
            {
                Message = "Error sending event to receiver: " + ex.Message,
                InnerException = ex,
                SubscriptionTag = tag
            };
            
            Publish(TinyException.DefaultChannel, message);
        }

        /// <summary>
        /// Publish an event the specified channel.
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="argument">The argument to pass.</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        public static void Publish(string channel, string argument = default, Action<Exception, ISubscription> onError = null)
        {
            Publish<string>(channel, argument, onError);
        }

        /// <summary>
        /// Publish using Task.Run
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <remarks>This method is not blocking, it simply uses a Task.Run(() => Publish(...)) internally
        /// to hand of the call to be handled by someone else.</remarks>
        public static void PublishAsFireAndForgetTask(string channel, string argument = default, Action<Exception, ISubscription> onError = null)
        {
            // Add to delayed handle queue
            Task.Run(() => Publish(channel, argument, onError));
        }

        /// <summary>
        /// Publish using Task.Run
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="instance">An instance of an object to pass to the handler of the event</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <remarks>This method is not blocking, it simply uses a Task.Run(() => Publish(...)) internally
        /// to hand of the call to be handled by someone else.</remarks>
        public static void PublishAsFireAndForgetTask<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            Task.Run(() => Publish(channel, instance, onError));
        }

        /// <summary>
        /// Publish async
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
        /// <param name="onError">Called if there is an error executing the subscription.</param>
        /// <returns>A task</returns>
        public static Task PublishAsync(string channel, string argument = default(string), Action<Exception, ISubscription> onError = null)
        {
            // EB: Not necessary to await the task.
            ////await Task.Run(() => Publish(channel, argument, OnError));
            return Task.Run(() => Publish(channel, argument, onError));
        }

        /// <summary>
        /// Clears all channels
        /// </summary>
        public static void Clear()
        {
            Channels.Clear();
        }

        /// <summary>
        /// Scans an object after attributes to hook up to TinyPubSub
        /// </summary>
        /// <param name="obj">The object to scan</param>
        public static void Register(object obj)
        {
            //// TODO: EB: Move the reflection code to a separate type, for performance - add a cache (ConcurrentDictionary) to scanned objects and use expressions to invoke the subscriber methods instead of method.Invoke()..

            var typeInfo = obj.GetType().GetTypeInfo();

            foreach (var method in typeInfo.DeclaredMethods)
            {
                var attributes = method.GetCustomAttributes(typeof(TinySubscribeAttribute)).OfType<TinySubscribeAttribute>();

                foreach (var attribute in attributes)
                {
                    var channel = attribute.Channel;

                    var methodParameters = method.GetParameters();

                    if (methodParameters.Length > 0)
                    {
                        // Concept code for subscriptions
                        Subscribe<object>(obj, channel, (data) => method.Invoke(obj, new[] { data }));
                    }
                    else
                    {
                        // Register without parameters since the target method has none
                        Subscribe(obj, channel, () => method.Invoke(obj, null));
                    }
                }
            }
        }

        public static void Deregister(object obj)
        {
            TinyPubSub.Unsubscribe(obj);
        }
    }
}
