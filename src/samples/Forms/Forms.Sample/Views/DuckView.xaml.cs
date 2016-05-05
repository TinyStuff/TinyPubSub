using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ViewModels;

namespace Views
{
	public partial class DuckView : ContentPage
	{
		public DuckView ()
		{
			InitializeComponent ();

			var vm = new DuckViewModel();
			vm.Navigation = this.Navigation;
			BindingContext = vm;
		}
	}
}

