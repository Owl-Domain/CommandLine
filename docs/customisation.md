# Customisation

There are a lot of customisation options available in the library
already *(with even more to come),* most of them are set through
`With*` methods on the `ICommandEngineBuilder`.

Some of the `With*` methods are used for registering extra components
*(such as custom value parser selectors),* while others are used for
fully replacing the default components, meaning that pretty much every
aspect of the library can be replaced and customised to a decent level.

There is also a special method `Customise(Action<BuilderSettings> callback)`
which can be used for customising some more fine grained settings, you
would use it like so:

```cs
using OwlDomain.CommandLine.Engine;

ICommandEngineBuilder builder = CommandEngine.New();

builder.Customise(settings => settings.Name = "my-cli");

// or
builder.Customise(settings => settings.WithName("my-cli"));
```

The second approach uses the builder approach, so after calling `WithName`
you can chain other methods too:

```cs
builder.Customise(settings =>
{
   settings
      .WithName("my-cli")
      .WithDescription("My cli program description.");
});
```

Along with this, there are also several methods on the `BuilderSettings`
type which set several settings at once *(to use in situations where they
are often used together)*, ie:

```cs
builder.Customise(settings => settings.WithHelp("help", 'h'));

// is equivalent to
builder.Customise(settings =>
{
   settings.IncludeHelpFlag = true;
   settings.LongHelpFlagName = "help";
   settings.ShortHelpFlagName = 'h';
});
```
