using System;

using Xamarin.Forms;
using TinyPubSubLib;
using System.Windows.Input;
using Views;

namespace ViewModels
{
	public class DuckViewModel : ViewModelBase
	{
		public DuckViewModel ()
		{
			TinyPubSub.Subscribe (this, "fire", () => 
			{ 
				// This line should not be fired when the page is navigated away from
				int i = 10;
			});	
		}

		public ICommand PopToRoot 
		{
			get {
				return new Command (async () => await Navigation.PopToRootAsync ());
			}
		}

		public ICommand ViewAnotherDuck
		{
			get {
				return new Command (async () => await Navigation.PushAsync (new DuckView ()));
			}
		}
	}
}