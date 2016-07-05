using System;
using TinyPubSubLib;

using Xamarin.Forms;
using Views;

namespace TinyPubSubForms.Sample
{
	public class App : Application
	{
		public App ()
		{
            // If you are using forms, then Init before setting the main view to automate the
            // unsubscription of events.
            TinyPubSubLib.TinyPubSubForms.Init(this);

			// The root page of your application
			var navPage = new NavigationPage(new MainView());

            // If you don't use the TinyPubSubForms.Init(..) method you can register the events yourself like this
			// navPage.Popped += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
			// navPage.PoppedToRoot += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
			
			MainPage = navPage;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}