using System;
using System.Linq;
using System.Collections.Generic;

namespace TinyPubSubLib
{
	public static class TinyPubSub
	{
		private static Dictionary<string, List<Subscription>> _channels = new Dictionary<string, List<Subscription>>();

		static List<Subscription> GetOrCreateChannel(string channel)
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

		static Subscription CreateSubscription (object owner, string channel, Action action)
		{
			var current = GetOrCreateChannel (channel);
			var subscription = new Subscription (owner, action);
			current.Add (subscription);
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

		public static string Subscribe(object owner, string channel, Action action)
		{
			var subscription = CreateSubscription (owner, channel, action);
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
			foreach (var channel in _channels) {
				foreach (var subscription in channel.Value.ToList()) {
					if (subscription.Owner == owner) {
						channel.Value.Remove (subscription);
					}
				}
			}
		}

		public static void Publish(string channel)
		{
			if (_channels.ContainsKey (channel)) {
				var current = _channels [channel];
				foreach (var subscription in current.ToList()) {
					try
					{
						subscription.Action();
					}
					catch(Exception) 
					{
						current.Remove(subscription);
					}
				}
			}
		}
	}

	public class Subscription
	{
		public Action Action { get; set; }
		public string Tag { get; set; }
		public object Owner { get; set; }

		public Subscription (Action action)
		{
			Action = action;
			Tag = Guid.NewGuid().ToString();
		}

		public Subscription (object owner, Action action)
		{
			Action = action;
			Tag = Guid.NewGuid().ToString();
			Owner = owner;
		}
	}
}