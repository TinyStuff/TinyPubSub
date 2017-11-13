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
            TinyPubSub.Subscribe<bool>("firebool",(obj) => {
                _toggleBool = obj;

            });
		}

        private bool _toggleBool = false;

		public ICommand Fire {
			get {
				return new Command (() => {
					TinyPubSub.Publish("fire");
				});
			}
		}

        public ICommand Fire2
        {
            get
            {
                return new Command(() => {
                    TinyPubSub.Publish<bool>("firebool",!_toggleBool);
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