/*
 * The MIT License (MIT)
 * Copyright (c) 2016 Johan Karlsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished 
 * to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */

using System;
using Xamarin.Forms;

namespace TinyPubSubLib
{
    public static class TinyPubSubForms
    {
        public static void Init(Application app)
        {
            app.ChildAdded += App_ChildAdded;
            app.PropertyChanged += App_PropertyChanged;
        }

        static void App_ChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is NavigationPage)
            {
                var page = e.Element as NavigationPage;
                BindEvents(page);
            }
        }

        static void App_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Application && e.PropertyName == "MainPage")
            {
                var page = (sender as Application).MainPage;

                if (page is NavigationPage)
                {
                    BindEvents(page as NavigationPage);
                }
            }
        }

        public static void SubscribeOnMainThread<T>(string owner, string channel, Action<T> action) 
        {
            TinyPubSub.Subscribe<T>(owner,channel, (obj) => {
                Device.BeginInvokeOnMainThread(() => action(obj));   
            });
        }

        public static void SubscribeOnMainThread<T>(string channel, Action<T> action)
        {
            SubscribeOnMainThread(null, channel, action);
        }

        public static void SubscribeOnMainThread(string owner, string channel, Action<string> action)
        {
            SubscribeOnMainThread<string>(owner, channel, action);
        }

        public static void SubscribeOnMainThread(string channel, Action<string> action)
        {
            SubscribeOnMainThread(null, channel, action);
        }


        static void BindEvents(NavigationPage page)
        {
            page.Popped += (s, args) => TinyPubSub.Unsubscribe(args.Page.BindingContext);
			page.PoppedToRoot += (s, args) =>
			{
				var poppedToRootEventArgs = args as PoppedToRootEventArgs;
				foreach (var poppedPage in poppedToRootEventArgs.PoppedPages)
				{
					TinyPubSub.Unsubscribe(poppedPage);
					TinyPubSub.Unsubscribe(poppedPage?.BindingContext);
				}
			};
        }
    }
}
