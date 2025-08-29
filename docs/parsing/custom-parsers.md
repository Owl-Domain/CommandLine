# Custom parsers

> [!NOTE]
> This document describes the process for implementing custom parsers, for the built-in
> parsers see [built-in parsers](./builtin-parsers.md) document, and for understanding
> the parsing syntax see the [parsing syntax](./parsing-syntax.md) document.

The parsing engine works on a system very similar to combinatoral parsers, and as such doesn't actually require any specific syntax for parsing/tokenising values, therefore any value parser is free to consume as much or as little characters as it needs to, without having to rely on tokenising to correctly split the values up.

## Things you need to understand

Before implementing a custom parser, there are some things that you should understand so that you know how to best implement this custom parser.

### Text fragments

When a parsing operation is started, the parsing engine is fed with text fragments, in REPL mode this will be a single text fragment *(such as the full command input),* and in CLI mode, this will be one or more text fragments, where each fragment is a single argument that was passed in to the process.

> [!WARNING]
> Process arguments *(understood by the OS/shell)* are different from command arguments *(understood by this library)* and they should not be confused.


### Parsing modes

The parsing system has different parsing modes *(two modes at the moment),* and the parsing mode can change several times during the parse operation for a single input.

The parsing mode used at the start depends on whether the parsing operation is happening in REPL mode *(lazy parsing)* or CLI mode *(greedy parsing).*


#### Greedy mode

In greedy mode, parsers are expected to use up the full text fragment to parse a value, even if that text fragment contains white-space, or other breaking point characters.


#### Lazy mode

In lazy mode, parsers are expected to only use up the characters until the next natural breaking point, typically this will mean white-space characters, however parsers can create scopes where this is different.

An example of this are the collection parsers, which will include the separator character `,` and the closing character `]` as break points *(collection parsers will also switch the parsing mode as necessary when parsing the collection values).*


#### Reasoning

The reason for this approach is that when you type something like this `my-program --flag "some value" 123` in your terminal, the called program will actually receive a tokenised list of arguments that contains the following values: `--flag`, `some value` and `123`.

This tokenisation is done by your shell, and as you may have already noticed, the text value `"some value"` that you typed in does not actually contain the quotes anymore, as they are stripped out by your shell.

When the program receives these arguments, it would then not only be a waste to merge the arguments into a single value and undo the tokenising work done by your shell, but it would also be problematic as the parsing would have to be aware of the semantics of all of the value parsers, which isn't really feasabile *(and even if it was, it wouldn't be nice).*

The opposite idea of this, is that when you get a single text fragment with the full input *(such as in REPL mode),* the parsing engine would have to manually split that input into several tokens, which means that the parsing engine would have to assume what boundaries to use, which has it's own set of problems.

### Missing vs empty values

Missing and empty values are considered different things, and they should not be confused as meaning the same thing.

Empty values are completely empty text fragments that have been explicitly passed in as empty value by the user
*(this is only observable in CLI mode with greedy parsing).*

Missing values occur in situations where there is nothing left for the value parser to parse, such as
at the end of the input.

This distinction is crucial as *some* parsers need to behave differently in these two cases, the only example of handling empty values at the moment is the string parser in greedy mode, when `""` would've been explicitly passed in by the user and should be allowed as a valid value.

### The text parser

The `ITextParser` interface is the main component that you will interact with when making custom parsers, and it provides easy ways of dealing with the different parsing modes.


## Value parsers

To implement a custom value parser, you need to:

- Implement the `IValueParser<>` interface *(in the `OwlDomain.CommandLine.Parsing.Values` namespace)*.
- Track the location where your parsed value is inside of the text fragments.
- Consume whatever characters you need to in order to be able to parse the value, while also reaching a good stopping point, *(ie. if the input is `123.123` then you shouldn't just parse the first `123` as a valid value).*
- Account for the value being missing or empty and error appropriately.
- Return either a successful result with a set value, or a failed result with an error message.
- Register your custom value parser through a custom [value parser selector](./value-parser-selectors.md).

you __do not__ need to worry about:

- Any white-space characters before or after the value.
- Rewinding/resetting the `ITextParser` if the parsing fails.

An error value of `null` means that no error has occured, and that a valid value was parsed
*(even if that value happens to be `null`).*

And an error value of an empty string `""` ([`string.Empty`](https://learn.microsoft.com/dotnet/api/system.string.empty)) tells the parsing engine that the value was missing, at which point the parsing engine will
make sure to emit a proper error that's easier for the user to understand.

> [!TIP]
> Most of the time, you will only need to use the `AdvanceUntilBreak()` extension method for the `ITextParser`,
> which will return a string consisting of the all of the characters that you should use for parsing the value.


### Using the base value parser implementation

It is *highly* recommended that you use the `BaseValueParser<>` type
*(in the `OwlDomain.CommandLine.Parsing.Values` namespace)* for most of your custom value parsers.

This is because the base implementation:

- Tracks the location where your parsed value is inside of the text fragments.
- Simplifies returning the result.
- Handles missing values.
- Handles empty values *(the `AllowEmptyValues` property can be used to indicate that you want to handle empty values yourself).*

__A simple implementation might look like this:__

```cs
public class BooleanValueParser : BaseValueParser<bool>
{
   protected override bool TryParse(
      IValueParseContext context,
      ITextParser parser,
      out string? error)
   {
      string text = parser.AdvanceUntilBreak();

      if (text is "true")
      {
         error = default;
         return true;
      }

      if (text is "false")
      {
         error = default;
         return false;
      }

      error = $"Couldn't parse '{text}' as a boolean value.";
      return default;
   }
}
```


### Custom collection value parsers

A collection value parser is a much more complex parser, with many more edge cases that need to be
handled, as well as having a more complex return type, so you should honestly just use the `BaseCollectionValueParser<,>` and let it do all of the
hard work, and just implement the method necessary for creating the final collection instance.

__A simple implementation might look like this:__

```cs
public class ArrayCollectionValueParser<TValue>(IValueParser<TValue> valueParser)
   : BaseCollectionValueParser<TValue[], TValue>(valueParser)
{
   protected override TValue[] CreateCollection(IReadOnlyList<TValue> values)
   {
      TValue[] array = new TValue[values.Count];

      for (int i = 0; i < values.Count; i++)
         array[i] = values[i];

      return array;
   }
}
```

> [!NOTE]
> It is up to the [value parser selector](./value-parser-selectors.md)
> to determine the parser for the values inside of the collection.
