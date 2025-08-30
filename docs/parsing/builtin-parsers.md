# Built-in parsers

> [!NOTE]
> This document describes the built-in parsers, for notes on implementing
> custom parsers see the [custom parsers](./custom-parsers.md) document, and for
> understanding the parsing syntax see the [parsing syntax](./parsing-syntax.md) document.

By default, the command engine has support for parsing quite a lot of different types, some of these types have explicit parsers, and some will be parsed through common .NET patterns.


## Implicit parsers

Implicit parsers are always chosen as a fallback so as to give explicit parsers a higher priority.

### Enums

Any type that is an `enum` *(whether a regular `enum` or a flags `enum`)* will be parsed according the names of the `enum` values.
By default the parser will be case-insensitive, unless that causes problems for a specific `enum` type.


### `Nullable<>`

Anything that implements [`Nullable<>`](https://learn.microsoft.com/dotnet/api/system.nullable-1) will be parsable as long as it's underlying type is parsable.


### `IBinaryInteger<>`

Anything that implements the [`IBinaryInteger<>`](https://learn.microsoft.com/dotnet/api/system.numerics.ibinaryinteger-1) interface will automatically be parsed, this means that any of the common integer types like `sbyte`, `short`, `int` and `ulong` *(along with their unsigned variants),* will be supported.


### `IFloatingPoint<>`

Anything that implements the [`IFloatingPoint<>`](https://learn.microsoft.com/dotnet/api/system.numerics.ifloatingpoint-1) interface will automatically be parsed, this means that any of the common floating point types like `float`, `double`, `decimal` *(and even `Half`)* will be supported.


### `IParsable<>`

Anything that implements the [`IParsable<>`](https://learn.microsoft.com/dotnet/api/system.iparsable-1) interface will automatically be parsed, [`ISpanParsable<>`](https://learn.microsoft.com/dotnet/api/system.ispanparsable-1) is not explicitly supported, however it requires the [`IParsable<>`](https://learn.microsoft.com/dotnet/api/system.iparsable-1) interface to be implemented anyway, so the type will still be parsable.

> [!WARNING]
> While [`IParsable<>`](https://learn.microsoft.com/dotnet/api/system.iparsable-1) means that almost every type can be parsed without any boilerplate, it is not the preferred approach as it will typically decrease the quality of the errors shown to the user when they make a mistake, as it will rely on the message included in the exception that is thrown when parsing fails.


### Collection types

Some collection types have explicit support, such as:

- Any single dimensional *typed* array.
- The [`Memory<>`](https://learn.microsoft.com/dotnet/api/system.memory-1) type.
- The [`ReadOnlyMemory<>`](https://learn.microsoft.com/dotnet/api/system.readonlymemory-1) type.

> [!WARNING]
> [`Span<>`](https://learn.microsoft.com/dotnet/api/system.span-1) and [`ReadOnlySpan<>`](https://learn.microsoft.com/dotnet/api/system.readonlyspan-1) __cannot__ be supported as they are ref struct types, which to my knowledge cannot be used in reflection contexts as they cannot be boxed and converted to an `object` instance *(but please do correct me if I'm wrong).*

After this, any class type that follows at least one of the following patterns will be parsable *(`T` means the type of the values in the collection)*:

- Has a constructor that accepts an `IEnumerable<T>` instance.
- Has an empty constructor and implements an `AddRange(IEnumerable<T> items)` method *(regardless of the return type)*.
- Has an empty constructor and implements an `Add(T item)` method *(regardless of the return type)*.

If the collection type is an interface instead, then that interface will try to be matched to a collection type, currently that means either [`List<>`](https://learn.microsoft.com/dotnet/api/system.collections.generic.list-1) or [`HashSet<>`](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset-1), as that will cover the majority *(if not all)* of the collection interfaces.


> [!WARNING]
> Currently only single type collections *(like lists)* are supported, dictionaries are not.


## Explicit parsers

There are types that parsers have been explicitly defined for, these are:


### Strings

The [`string`](https://learn.microsoft.com/dotnet/api/system.string) type supports various formats, you can use unqouted string *(will only parse until the next boundary)*, or strings quoted with the `"` or `'` characters.

Some common sequences can be escaped by prefixing them with the `\` character.


### Booleans

The [`bool`](https://learn.microsoft.com/dotnet/api/system.boolean) type supports a few different keywords.

For `true` values, the valid keywords are `true`, `yes`, `t` and `y`, regardless of casing.
For `false` values, the valid keywords are `false`, `no`, `f` and `n`, regardless of casing.


### Networking types

Several of the .NET networking types have had parsers implemented for them, along with one custom struct `Port` for representing a networking port value.

#### IPAddress

The [`IPAddress`](https://learn.microsoft.com/dotnet/api/system.net.ipaddress) type allows for both IPv4 and IPv6 addresses to be parses.


#### Port

The custom `Port` struct *(found in the `OwlDomain.CommandLine.SpecialTypes` namespace)*
allows for parsing networking ports *(numbers between `0` and `65,535` inclusively),* and
for using special aliases for commonly used ports, ie. `http` would mean port `80`.

> [!NOTE]
> The aliasing behaviour cannot be changed right now, unless you create a custom parser
> for the `Port`, `IPEndPoint` and `DnsEndPoint` types.


#### IPEndPoint

The [`IPEndPoint`](https://learn.microsoft.com/dotnet/api/system.net.ipendpoint) type allows for a combination of either an IPv4 or IPv6 address, and a port value *(which will allow for the same aliases as the `Port` parser)*.


#### DnsEndPoint

The [`DnsEndPoint`](https://learn.microsoft.com/dotnet/api/system.net.dnsendpoint) type allows for a combination of a hostname, and a port value *(which will allow for the same aliases as the `Port` parser)*.


### Path like types

There are parsers for several .NET types that resemble file/directory paths.

#### Uri

The [`Uri`](https://learn.microsoft.com/dotnet/api/system.uri) type will allow for
both relative and absolute paths to be parsed.


#### FileInfo

The [`FileInfo`](https://learn.microsoft.com/dotnet/api/system.io.fileinfo) type will parse paths that can point to a file, regardless of whether it currently exists on the file system or not.


#### DirectoryInfo

The [`DirectoryInfo`](https://learn.microsoft.com/dotnet/api/system.io.directoryinfo) type will parse paths that can point to a directory, regardless of whether it currently exists on the file system or not.

> [!TIP]
> This will also allow for special directories like `.` *(current directory)* and `..` *(parent directory).*


#### FileSystemInfo

The [`FileSystemInfo`](https://learn.microsoft.com/dotnet/api/system.io.filesysteminfo) type will parse paths that point to either a file or to a directory, with the caveat that
it must exist on the file system at the time of parsing.

The caveat is there because if the path does not exist on the file system, then it isn't safe to assume whether the user meant to specify a file, or if they meant to specify a directory.

> [!WARNING]
> Even though the path must exist on the file system at the time of parsing, this does
> not mean that it will still exist when you try to access it.
