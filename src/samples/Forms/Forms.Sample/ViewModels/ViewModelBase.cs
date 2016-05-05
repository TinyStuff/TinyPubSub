using System;
using Xamarin.Forms;

namespace ViewModels
{
	public class ViewModelBase
	{
		public INavigation Navigation {
			get;
			set;
		}
	}
}