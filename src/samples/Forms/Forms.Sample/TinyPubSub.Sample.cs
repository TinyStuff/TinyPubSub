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
			// The root page of your application
			var navPage = new NavigationPage(new MainView());

			navPage.Popped += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
			navPage.PoppedToRoot += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
			
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