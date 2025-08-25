# OwlDomain.CommandLine

[![Build](https://github.com/Owl-Domain/CommandLine/actions/workflows/build.yml/badge.svg)](https://github.com/Owl-Domain/CommandLine/actions/workflows/build.yml)
[![Test](https://github.com/Owl-Domain/CommandLine/actions/workflows/test.yml/badge.svg)](https://github.com/Owl-Domain/CommandLine/actions/workflows/test.yml)

---

> [!IMPORTANT]
> This project will eventually be in-lined into the [`OwlDomain.Console`](https://github.com/Owl-Domain/Console) project *(which does not exist yet)*, so prepare for this project to eventually be deprecated.

This project focuses on experimenting with a "zero boilerplate" approach to making command line tools.

*(In reality it's a "zero or the minimal amount of necessary boilerplate" approach but that isn't quite as catchy).*

Which will result in regular C# code like this:

```cs
using OwlDomain.CommandLine.Engine;

namespace ListManager;

class Program
{
   static void Main(string[] args)
   {
      ICommandEngine engine = CommandEngine.New()
         .From<Commands>()
         .Build();

      engine.Run(args);
      // Or
      engine.Repl();
   }
}

class Commands
{
   /// <summary>The list to manage.</summary>
   public required string List { get; init; }

   /// <summary>Adds a new <paramref name="item"/> to the specified <see cref="List"/>.</summary>
   /// <param name="item">The item to add to the specified <see cref="List"/>.</param>
   public void Add(string item)
   {
      /* implementation code */
   }

   /// <summary>Removes an <paramref name="item"/> from the specified <see cref="List"/>.</summary>
   /// <param name="item">The item to remove from the specified <see cref="List"/>.</param>
   public void Remove(string item)
   {
      /* implementation code */
   }
}
```

Turning into a CLI that looks a little like this:

__Output from `help`:__
![Help overview of the created CLI program](.github/assets/help_overview.png)

__Output from `add --help`:__
![Help overview for the add command for the created CLI program](.github/assets/help_command.png)

*(The displayed colors will depend on the colors you have selected for your terminal).*

## Features

- Very customisable, almost every component can be fully replaced.
- Long and short flags.
- Required/optional flag and argument values.
- CLI and REPL modes.
- Customisable "dependency injection" like feature.
- Virtual flags and commands, ie. `help` and `version`.
- Co-operative cancellation support.
- Support for return values.


## Contributions

Code contributions will not be accepted, however feel free to provide feedback / suggestions
by creating a [new issue](https://github.com/Owl-Domain/CommandLine/issues/new), or look at
the [existing issues](https://github.com/Owl-Domain/CommandLine/issues?q=) to see if your
concern / suggestion has already been raised.


## License

This project (the source, the release files, e.t.c) is released under the
[OwlDomain License](https://github.com/Owl-Domain/CommandLine/blob/master/license.md).
