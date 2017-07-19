# TinyPubSub
<del>Worlds smallest</del> A really small pub/sub thingy created mostly for Xamarin Forms but should also work else where...

## Build status

![buildstatus](https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/5/badge)

##TLDR

Init - if you are using forms, install the TinyPubSub.Forms package

```csharp
public App ()
{
    // If you are using forms
    TinyPubSubLib.TinyPubSubForms.Init(this);

	// The root page of your application
	var navPage = new NavigationPage(new MainView());

    // If you don't use the TinyPubSubForms.Init(..) method you can register the events yourself like this
	// navPage.Popped += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
	// navpage.PoppedToRoot += (s, args) =>
	//		{
	//			var poppedToRootEventArgs = args as PoppedToRootEventArgs;
	//			foreach (var poppedPage in poppedToRootEventArgs.PoppedPages)
	//			{
	//				TinyPubSub.Unsubscribe(poppedPage.BindingContext);
	//			}
	//		};
	MainPage = navPage;
}

```

Subscribe

```csharp
// The forms way (from ViewModel)
TinyPubSub.Subscribe(this, "new-duck-added", () => { RebindDuckGui(); });

// Non-forms way
TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```


Publish

```csharp
TinyPubSub.Publish("new-duck-added");
```

## WHY SHOULD I USE IT?

<img align="right" src="http://i.imgur.com/p0xJYYC.png">

This lib should be used when you want to easily register to events within a small app. It's not meant for data transfer (at least not at this point), it's not thread safe and it's never going to be finished. :)

### EXAMPLE
I have a view that shows ducks. This is my main view. When I edit ducks on another view and the main page is covered I still want the main view to get new ducks before I return to it. I don't want the MainPage to start loading when it gets displayed. 

The main view can listen for the "ducks-added" event and run an action when that happens. When I create a new function in the system I can trust that if I publish an event on the "ducks-added" channel, all my other views subscribing to that event will get notified.

And by following some patterns regarding the NavigationPage(...) we can also make sure that the subscriptions are removed when the view go out of scope.

It's designed with MVVM in mind. Subscription to new events should be done in the ViewModel and the Unsubscription should be made automatically when pages are popped (see usage).

It's not meant to solve world problems so if you want a robust and mature pub/sub framework then there are plenty others out there to use. This is bare metal.

## STATE

Release (1.0 for TinyPubSub and 1.1 for TinyPubSub.Forms)

## NUGET

Package are built for profile 259.

###Forms
If you are using TinyPubSub from Xamarin forms, install this package and call the init method as described at the top. You don't have to install any other package.

[https://www.nuget.org/packages/tinypubsub.forms](https://www.nuget.org/packages/tinypubsub.forms)

###Vanilla
[https://www.nuget.org/packages/tinypubsub](https://www.nuget.org/packages/tinypubsub)


## USAGE

To subscribe, simply register what "channel" (we call them channels) you would like to subscribe to.

```c#
TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```

And in another part of you application, publish events to execute the actions that are registered for that channel.

```c#
TinyPubSub.Publish("new-duck-added");
```

### WHAT ABOUT MEMORY ISSUES?

If you are using the Xamarin Forms version (TinyPubSub.Forms) and call the Init(..) method as described at the top of this page, then you have no worries. The lib will take care of deregistration just in time given that you take two things into consideration.

* You register your subscriptions from the ViewModel (whatever object you bind to BindingContext) or the page it self
* You pass in `this` into the subscription registration like `TinyPubSub.Subscribe(this, "new-duck-added", () => { RebindDuckGui(); });`

If you use the vanilla version, continue reading.

#### Plan A - tags

When subscribing you get a tag.

```c#
var tag = TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```

And when you are done you unsubscribe with that tag.

```c#
TinyPubSub.Unsubscribe(tag);
```

#### Plan B - object refs

This is a more suitable option for Xamarin MVVM (which is really the reason for this projects existance). I don't like having to keep track of tags. So instead we pass a reference to an object that counts as the owner of the subscription. Usually this and most usually a ViewModel. This way we can subscribe to several channels.

```c#
TinyPubSub.Subscribe(this, "new-duck-added", () => { RebindDuckGui(); });
TinyPubSub.Subscribe(this, "old-duck-removed", () => { RebindDuckGui(); });
```

And when the view is done (if we're talking MVVM) then the unsubscription could look like this.

```c#
TinyPubSub.Unsubscribe(this);
```

Or specifically in Xamarin Forms

```c#
TinyPubSub.Unsubscribe(this.BindingContext); // if this is a View and the Binding context the view model
```

The tricky part is still knowing when the view is done. One way is to hook up to the navigation page Popped and PoppedToRoot (if Forms, but then just use TinyPubSub.Forms package instead).

```c#
// The root page of your application
var navPage = new NavigationPage(new MainView());
navPage.Popped += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
navpage.PoppedToRoot += (s, args) =>
			{
				var poppedToRootEventArgs = args as PoppedToRootEventArgs;
				foreach (var poppedPage in poppedToRootEventArgs.PoppedPages)
				{
					TinyPubSub.Unsubscribe(poppedPage.BindingContext);
				}
			};
MainPage = navPage;
```
<del>This works as long as PopToRoot isn't called and you are more than one level deep in the navigation stack. There is also a NavigationPage.PoppedToRoot event, but looking at the Xamarin Forms code it simply clears the children without calling popped for each page. I've started a thread about this at the xamarin forums. </del>

I got some new code into the Xamarin Forms Core Navigation stuff so now we can get information on what pages that are popped.
