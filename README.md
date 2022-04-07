# TinyPubSub
A really small pub/sub thingy created for .net!
In memory, in-process, tiny and shiny. Sync publish, fire-and-forget publish, async publish, non-generic and generic publish/subscribe.

## Roadmap

* V2 is under development - the aim is to modernize the lib to make use of new C# language features and runtimes.

## Build status

[![.NET](https://github.com/TinyStuff/TinyPubSub/actions/workflows/BuildAndTest.yml/badge.svg)](https://github.com/TinyStuff/TinyPubSub/actions/workflows/BuildAndTest.yml)

## TLDR

What does it do? TinyPubSub lets you run code when an event happens.

Init - if you are using forms, install the TinyPubSub.Forms package

```csharp

// If you are using Xamarin
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
TinyPubSub.Subscribe(this, "new-duck-added", () => RebindDuckGui());

// Non-forms way
TinyPubSub.Subscribe("new-duck-added", () => RebindDuckGui());

// The forms way (from ViewModel) with an argument
TinyPubSub.Subscribe(this, "new-duck-added", (x) => RebindDuckGui(nameofduck: x));

// Non-forms way
TinyPubSub.Subscribe("new-duck-added", (x) => RebindDuckGui(nameofduck: x));

// The forms way with a typed argument, where MessageModel can be any class you'd like
TinyPubSub.Subscribe<MessageModel>(this, "new-duck-added", (model) => RebindDuckGui(nameofduck: model.Name));

// Subscription by attribute
public class MyClass
{
    public MyClass()
    {
        // Register it directly in the class or somewhere else
        TinyPubSub.Register(this);
    }

    [TinySubscribe("new-duck-added")]
    public void DuckAdded(MessageModel duck)
    {
        // Do something with the duck
    }
}

// Subscription with control to determine if we should prevent the next subscriber on the same channel to handle the event
// NOTE: Must be triggered by any PublishControlled* method to work. The _ argument is because we need an argument even if it's null
TinyPubSub.Subscribe("new-duck-added", (_, args) =>
{
    args.HaltExecution = true;
});

```

Publish

```csharp
// Without argument
TinyPubSub.Publish("new-duck-added");

// With argument
TinyPubSub.Publish("new-duck-added", "Ducky McDuckface");

// As a task (that runs later on)
TinyPubSub.PublishAsTask("new-duck-added");

// As a task (that runs later on) with argument
TinyPubSub.PublishAsTask("new-duck-added", "Ducky McDuckface");

// Async
await TinyPubSub.PublishAsync("new-duck-added");

// Async with argument
await TinyPubSub.PublishAsync("new-duck-added", "Ducky McDuckface");

// Publish with a typed argument
TinyPubSub.Publish("new-duck-added", new MessageModel() { Name = "Ducky" });

// Publish and wait for the result - can be used to make sure someone has handled it
var result = TinyPubSub.PublishControlled("new-duck-added");
var successful = result.Handled;

// Publish controlled with a typed argument
var result = TinyPubSub.PublishControlled("new-duck-added", new MessageModel() { Name = "Ducky" });
var successful = result.Handled;

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

Release (1.2.x for TinyPubSub and 1.2.x for TinyPubSub.Forms)

## NUGET

Package 1.0.x and 1.1.x are built for profile 259. Packaged 1.2.x are built using netstandard 1.0.

## EXCEPTION HANDLING

Exceptions are sent back to you through TinyPubSub itself.

```csharp
TinyPubSub.Subscribe(this, TinyExceptionDefaultChannel, (TinyException ex) => { HandleException(ex) });
```

You can also send an error handler directly into the publish call.

```csharp
TinyPubSub.Publish(this, "new-duck-added", () => HandleDuck(), onError: (ex, s) => 
    {
        // ex is the Exception that was thrown
        // s is the subscription that failed
    });
```

### Forms
If you are using TinyPubSub from Xamarin forms, install this package and call the init method as described at the top. You don't have to install any other package.

[https://www.nuget.org/packages/tinypubsub.forms](https://www.nuget.org/packages/tinypubsub.forms)

### Vanilla
[https://www.nuget.org/packages/tinypubsub](https://www.nuget.org/packages/tinypubsub)


## USAGE

To subscribe, simply register what "channel" (we call them channels) you would like to subscribe to.

```csharp
TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```

And in another part of you application, publish events to execute the actions that are registered for that channel.

```csharp

// Sync publish with or without argument
TinyPubSub.Publish("new-duck-added", "optional argument");

// As a task (fire and forget)
TinyPubSub.PublishAsTask("new-duck-added", "optional argument");

// As an async call
await TinyPubSub.PublishAsync("new-duck-added", "optional argument");
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
