using System;
using System.Linq;
using System.Collections.Generic;

namespace TinyPubSubLib
{
	public static class TinyPubSub
	{
		private static Dictionary<string, List<Action>> _channels = new Dictionary<string, List<Action>>();

		/// <summary>
		/// Subscribe to a channel
		/// </summary>
		/// <param name="channel">The channel name</param>
		/// <param name="action">The action to run</param>
		/// <returns>A tag that can be used to unsubscribe</returns>
		public static string Subscribe(string channel, Action action)
		{
			List<Action> actions;
			if (!_channels.ContainsKey (channel)) {
				actions = new List<Action> ();
				_channels.Add (channel, actions);
			} else {
				actions = _channels [channel];
			}

			actions.Add (action);
			return string.Empty;
		}

		public static void Unsubscribe(string tag)
		{
			throw new NotImplementedException ();
		}

		public static void Publish(string channel)
		{
			if (_channels.ContainsKey (channel)) {
				var actions = _channels [channel];
				foreach (var action in actions.ToList()) {
					try
					{
						action.Invoke ();
					}
					catch(Exception ex) {
						actions.Remove (action);
					}
				}
			}
		}
	}
}