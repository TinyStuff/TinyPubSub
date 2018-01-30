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

namespace TinyPubSubLib
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using TinyPubSubLib.Subscriptions;

    public static class TinyPubSub
    {
        //// EB: replaced Dictionary<,> with ConcurrentDictionary<,> for thread safety. The use of a dictionary inside the dictionary is due to the fact that there is no standard concurrent hashset nor concurrent list in ".NET *".
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<ISubscription, ISubscription>> _channels = new ConcurrentDictionary<string, ConcurrentDictionary<ISubscription, ISubscription>>();

        private static ConcurrentDictionary<ISubscription, ISubscription> GetOrCreateChannel(string channel)
        {
            return _channels.GetOrAdd(channel, key => new ConcurrentDictionary<ISubscription, ISubscription>());
        }

        // EB: Rename to CreateAndAdd?
        private static ISubscription CreateSubscription(object owner, string channel, Action action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<string>(owner, action, disableAfterFirstUse);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        // EB: Rename to CreateAndAdd?
        private static ISubscription CreateSubscription<T>(object owner, string channel, Action<T> action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action, disableAfterFirstUse);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        // EB: Rename to CreateAndAdd?
        private static ISubscription CreateSubscription<T>(object owner, string channel, Action<T, TinyEventArgs> action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action, disableAfterFirstUse);
            current.TryAdd(subscription, subscription);
            return subscription;
        }

        /// <summary>
        /// Subscribe to a channel
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        public static string Subscribe(string channel, Action action)
        {
            var subscription = CreateSubscription(null, channel, action);
            return subscription.Tag;
        }

        public static string SubscribeTo<T>(string channel, Action<SubscriptionConfigurator<T>> config)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>();
            config(new SubscriptionConfigurator<T>(subscription));
            current.TryAdd(subscription, subscription);
            return subscription.Tag;
        }

        public static string SubscribeTo(string channel, Action<SubscriptionConfigurator<string>> config)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<string>();
            config(new SubscriptionConfigurator<string>(subscription));
            current.TryAdd(subscription, subscription);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        public static string Subscribe(string channel, Action<string> action)
        {
            var subscription = CreateSubscription<string>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel
        /// </summary>
        /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubcribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action action)
        {
            var subscription = CreateSubscription(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubcribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action<string> action)
        {
            var subscription = CreateSubscription<string>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T> action)
        {
            var subscription = CreateSubscription<T>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T> action)
        {
            var subscription = CreateSubscription<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancle execution and specify if the event has been handled
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="owner">Owner.</param>
        /// <param name="channel">Channel.</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publishreturn.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateSubscription<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancle execution and specify if the event has been handled
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="channel">Channel.</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publishreturn.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateSubscription<T>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Unsubscribes to a channel based on a tag
        /// </summary>
        /// <param name="tag"></param>
        public static void Unsubscribe(string tag)
        {
            foreach (var channel in _channels.Values)
            {
                foreach (var subscription in channel.Keys)
                {
                    if (subscription.Tag == tag)
                    {
                        channel.TryRemove(subscription, out var _);
                    }
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

            foreach (var channel in _channels.Values)
            {
                foreach (var subscription in channel.Keys)
                {
                    if (subscription.Owner == owner)
                    {
                        channel.TryRemove(subscription, out var _);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument.
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        /// <typeparam name="T">The type of the instance to pass to the receiver.</typeparam>
        public static void Publish<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            PublishControlled<T>(channel, instance, onError);
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument and returns if the event is handled.
        /// </summary>
        /// <returns>The controlled.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        public static TinyEventArgs PublishControlled(string channel, string instance, Action<Exception, ISubscription> onError = null)
        {
            return PublishControlled<string>(channel, instance, onError);
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument and returns if the event is handled.
        /// </summary>
        /// <returns>The controlled.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static TinyEventArgs PublishControlled<T>(string channel, T instance, Action<Exception, ISubscription> onError = null)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("You have to specify a channel to publish to");
            }

            var returnEventArgs = new TinyEventArgs();

            if (_channels.TryGetValue(channel, out var current))
            {
                // EB: ToList removed, since ConcurrentDictionary always returns a copy of the data
                foreach (var subscription in current.Keys.OfType<Subscription<T>>())
                {
                    try
                    {
                        subscription.Invoke(instance, returnEventArgs);
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

                // Concept code - fall back to calling each with object
                // this is the way we need to do it for allowing attribute
                // subscription.
                if (typeof(T) != typeof(object))
                {
                    PublishControlled<object>(channel, instance, onError);
                }
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
            Publish<TinyException>(TinyException.DefaultChannel, message);
        }

        /// <summary>
        /// Publish an event the specified channel.
        /// </summary>
        /// <param name="channel">The channel name</param>
        public static void Publish(string channel, string argument = default(string), Action<Exception, ISubscription> OnError = null)
        {
            Publish<string>(channel, argument, OnError);
        }

        /// <summary>
        /// Publish using Task.Run
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
        /// <remarks>This method is not blocking, it simply uses a Task.Run(() => Publish(...)) internally
        /// to hand of the call to be handled by someone else.</remarks>
        public static void PublishAsTask(string channel, string argument = default(string), Action<Exception, ISubscription> onError = null)
        {
            // Add to delayed handle queue
            //// EB: Configure await is only required when awaiting a task, not creating a fire and forget task
            //// Task.Run(() => Publish(channel, argument, OnError)).ConfigureAwait(false);
            Task.Run(() => Publish(channel, argument, onError));
        }

        /// <summary>
        /// Publish async
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
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
            _channels.Clear();
        }

        /// <summary>
        /// Registers an object by attributes to subscribe to TinyPubSub
        /// </summary>
        /// <param name="obj">The object to register</param>
        public static void Register(object obj)
        {
            //// TODO: EB: Move the reflection code to a separate type, for performance - add a cache (ConcurrentDictionary) to scanned objects and use expressions to invoke the subscriber methods instead of method.Invoke()..

            var typeInfo = IntrospectionExtensions.GetTypeInfo(obj.GetType());

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
                        var firstParam = methodParameters.First();
                        var paramType = firstParam.ParameterType;   // EB: Not used
                        TinyPubSub.Subscribe<object>(obj, channel, (data) => method.Invoke(obj, new object[] { data }));
                    }
                    else
                    {
                        // Register without parameters since the target method has none
                        TinyPubSub.Subscribe(obj, channel, () => method.Invoke(obj, null));
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
