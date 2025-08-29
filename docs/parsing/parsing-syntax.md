# Parsing syntax

> [!NOTE]
> This document describes parsing from the user's point of view, for notes on implementing
> custom parsers see [custom parsers](./custom-parsers.md) document, and for the built-in
> parsers see the [built-in parsers](./builtin-parsers.md) document.

The parsing system *(which is also quite customisable, although not as much as I'd like it to be yet),*
allows for quite a few different syntax styles by default, along with respecting commonly expected features.


## Flags

The parsers has support for short and long flags, these are differentiated by their prefix,
short flags *(by default)* use the `-` prefix *(ie. `-h`)*, and long flags use the `--` prefix *(ie. `--help`).*


### Values

Flags can specify their values in several ways by default, this can be done by either using `=`, `:` or a white-space character.

__Example syntaxes:__

```
--help true
--help:true
--help=true
-h true
-h:true
-h=true
```

> [!WARNING]
> Currently the parser doesn't allow for white-space characters before the separator, ie.
> `-h :true` and `-h =true` are __not__ allowed, but `-h: true` and `-h= true` are perfectly fine.


### Toggle flags

Boolean *(true/false)* flags do not have to explicitly specify their value, simply including
them will cause their value to be `true`, ie. just doing `-h` and `--help` is fine.


#### Chain flags

Toggle flags can also be chained *(this is only supported for short flags),* where instead
of doing `-a -b -c`, you can do `-abc`.


### Repeat flags

Some short flags can also be considered repeat flags, where the more you repeat the short name the higher it's value will be, ie. `-v` would be the same as doing `-v 1`, and `-vvv` would be the same as doing `-v 3`.

> [!WARNING]
> Currently this is only implemented for integer type flags with the name `verbose` or `verbosity` *(regardless of casing),* this __cannot__ be customised yet.


### Windows styled syntax

This library also has support for enabling Windows styles CLI syntax, this means that there
is no distinction between short and long flags, and they both use `/` as the prefix, along with this the short help flag is renamed to `?`, ie. you would use the `/help` or `/?` flags.

This also means that repeat and chain flags are not allowed, as that would cause parsing ambiguities.

> [!NOTE]
> This feature is disabled by default, and must be explicitly enabled by user of the library,
> and even if they do enable it, it will by default only be enabled when running on the Windows platform.


## Arguments

Arguments do not require any special syntax to be parsed, but they do have to be in order
*(flags are allowed before, in-between and after the arguments).*


## Explicitly separating flags and arguments

Anything that starts with `-` or `--` will be considered a flag, however sometimes you may want to pass in those characters as an argument value, to do this, you can explicitly mark the end of flags by using `--` on it's own, and anything after it will be parsed as the command arguments, ie. `--help -- --help` would be parsed as `<flag> -- <argument>`.
