using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ViewModels;

namespace Views
{
	public partial class MainView : ContentPage
	{
		public MainView()
		{
			InitializeComponent();
			BindingContext = new MainViewModel();
		}
	}
}