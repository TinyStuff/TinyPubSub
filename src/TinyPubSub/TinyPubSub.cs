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
using System.Linq;
using System.Collections.Generic;

namespace TinyPubSubLib
{
	public static class TinyPubSub
	{
		private static Dictionary<string, List<Subscription>> _channels = new Dictionary<string, List<Subscription>>();

		private static List<Subscription> GetOrCreateChannel(string channel)
		{
			List<Subscription> subscriptions;
			if (!_channels.ContainsKey (channel)) {
				subscriptions = new List<Subscription> ();
				_channels.Add (channel, subscriptions);
			}
			else {
				subscriptions = _channels [channel];
			}
			return subscriptions;
		}

		private static Subscription CreateSubscription (object owner, string channel, Action action)
		{
			var current = GetOrCreateChannel (channel);
			var subscription = new Subscription (owner, action);
			current.Add (subscription);
			return subscription;
		}

        private static Subscription CreateSubscription (object owner, string channel, Action<string> action)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription(owner, action);
            current.Add(subscription);
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
			var subscription = CreateSubscription (null, channel, action);
			return subscription.Tag;
		}

        public static string Subscribe(string channel, Action<string> action)
        {
            var subscription = CreateSubscription(null, channel, action);
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
			var subscription = CreateSubscription (owner, channel, action);
			return subscription.Tag;
		}

        public static string Subscribe(object owner, string channel, Action<string> action)
        {
            var subscription = CreateSubscription(owner, channel, action);
            return subscription.Tag;
        }

        public static void Unsubscribe(string tag)
		{
			foreach (var channel in _channels) {
				foreach (var subscription in channel.Value.ToList()) {
					if (subscription.Tag == tag) {
						channel.Value.Remove (subscription);
					}
				}
			}
		}

		public static void Unsubscribe(object owner)
		{
			if (owner == null)
			{
				return;
			}

			foreach (var channel in _channels) {
				foreach (var subscription in channel.Value.ToList()) {
					if (subscription.Owner == owner) {
						channel.Value.Remove (subscription);
					}
				}
			}
		}

		/// <summary>
		/// Publish an event the specified channel.
		/// </summary>
		/// <param name="channel">The channel name</param>
		public static void Publish(string channel, string argument = default(string))
		{
			if (string.IsNullOrWhiteSpace(channel))
			{
				throw new ArgumentException("You have to specify a channel to publish to");
			}

			if (_channels.ContainsKey (channel)) {
				var current = _channels [channel];
				foreach (var subscription in current.ToList()) {
					try
					{
                        /*  if(argument != null && subscription.ActionWithArgument != null)
                          {
                              subscription.ActionWithArgument(argument);
                          }
                          else
                          {
                              subscription.Action();
                          } */
                        subscription.Action?.Invoke();
                        subscription.ActionWithArgument?.Invoke(argument);
					}
					catch(Exception) 
					{
						// We should not have exceptions leaking all the way up here.
						current.Remove(subscription);
					}
				}
			}
		}

        public static void Clear()
        {
            _channels.Clear();
        }
	}
}
