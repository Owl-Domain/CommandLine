# Value parser selectors

The value parser selector is the mechanism through which the command engine assigns the parsers for flag and argument values.


## Creating a custom value parser selector

If you want to create a custom selector, simply implement the `IValueParserSelector` interfaces *(found in the `OwlDomain.CommandLine.Parsing.Values` namespace).*


### Using the base value parser selector implementation

It is recommended that you use the `BaseValueParserSelector` type *(found in the `OwlDomain.CommandLine.Parsing.Values` namespace)* to create your custom selector as it will:

- Simplify the selection process for most cases.
- Verify that the selected parser is for values of the expected type.
- Properly implement caching.

If you need to you can opt-out of the automatic caching *(through the `CacheAutomatically` property),*
as it might not be desireable in certain cases - you'll still be able to use the caching methods provided by the `BaseValueParserSelector` to manually cache and retrieve the parsers.

> [!NOTE]
> Caching had to be implemented on a per-selector basis in order to preserve the priority of the selectors.

__A simple implementation might look like this:__

```cs
public class PrimitiveValueParserSelector : BaseValueParserSelector
{
   protected override IValueParser? TrySelect(IRootValueParserSelector rootSelector, Type type)
   {
      if (type == typeof(string))
         return new StringValueParser();

      if (type == typeof(bool))
         return new BooleanValueParser();

      return null;
   }
}
```
