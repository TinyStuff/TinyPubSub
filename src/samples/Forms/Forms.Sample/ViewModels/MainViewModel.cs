using System;
using System.Windows.Input;
using Xamarin.Forms;
using TinyPubSubLib;

namespace ViewModels
{
	public class MainViewModel
	{
		public MainViewModel ()
		{
			TinyPubSub.Subscribe ("fire", () => 
				{ 
 					// Do something here, but nuget was down at the time of writing this code
				});	
		}

		public ICommand Fire {
			get {
				return new Command (() => {
					TinyPubSub.Publish("fire");
				});
			}
		}
	}
}