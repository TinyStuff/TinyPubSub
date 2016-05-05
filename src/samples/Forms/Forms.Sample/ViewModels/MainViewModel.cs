using System;
using System.Windows.Input;
using Xamarin.Forms;
using TinyPubSubLib;
using Views;

namespace ViewModels
{
	public class MainViewModel : ViewModelBase 
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

		public ICommand NavigateToDuck {
			get {
				return new Command (async () => {
					await Navigation.PushAsync(new DuckView());
				});
			}
		}
	}
}