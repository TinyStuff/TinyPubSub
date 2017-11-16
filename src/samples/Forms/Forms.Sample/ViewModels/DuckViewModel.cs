using System;

using Xamarin.Forms;
using TinyPubSubLib;

using System.Windows.Input;
using Views;
using System.ComponentModel;

namespace ViewModels
{
   
	public class DuckViewModel : ViewModelBase, INotifyPropertyChanged
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

        public DuckViewModel ()
		{
			TinyPubSub.Subscribe (this, "fire", () => 
			{ 
				// This line should not be fired when the page is navigated away from
				int i = 10;
			});	
            TinyPubSubLib.TinyPubSubForms.SubscribeOnMainThread("onmain",(obj) => {
                DuckCount++;
                PropertyChanged(this, new PropertyChangedEventArgs("DockCount"));
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