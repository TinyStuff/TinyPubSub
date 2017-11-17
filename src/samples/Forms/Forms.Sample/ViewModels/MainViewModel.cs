using System;
using System.Windows.Input;
using Xamarin.Forms;
using TinyPubSubLib;
using Views;
using System.ComponentModel;

namespace ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
	{
        int duckCount = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public int DuckCount
        {
            get
            {
                return duckCount;
            }

            set
            {
                duckCount = value;
            }
        }

		public MainViewModel ()
		{
			TinyPubSub.Subscribe ("fire", () => 
				{ 
 					// Do something here, but nuget was down at the time of writing this code
				});	
        TinyPubSub.Subscribe<bool>("firebool",(obj) => {
            _toggleBool = obj;

        });
        TinyPubSubLib.TinyPubSubForms.SubscribeOnMainThread("onmain", (obj) => {
            DuckCount++;
            PropertyChanged(this, new PropertyChangedEventArgs("DuckCount"));
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
        
        public ICommand Fire3
        {
            get
            {
                return new Command(() => {
                    TinyPubSub.Publish("onmain");
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