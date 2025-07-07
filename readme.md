# OwlDomain.CommandLine

[![Build](https://github.com/Owl-Domain/CommandLine/actions/workflows/build.yml/badge.svg)](https://github.com/Owl-Domain/CommandLine/actions/workflows/build.yml)
[![Test](https://github.com/Owl-Domain/CommandLine/actions/workflows/test.yml/badge.svg)](https://github.com/Owl-Domain/CommandLine/actions/workflows/test.yml)

---

> [!IMPORTANT]
> This project will eventually be in-lined into the [`OwlDomain.Console`](https://github.com/Owl-Domain/Console) project *(which does not exist yet)*, so prepare for this project to eventually be deprecated.

This project focuses on experimenting with a "zero boilerplate" approach to making command line tools.

*(In reality it's a "zero or the minimal amount of necessary boilerplate" approach but that isn't quite as catchy).*

Which will theoretically result in regular C# code like this:

```cs
namespace ListManager;

class Program
{
	static void Main(string[] args)
	{
		CommandLineEngine.New()
			.From<Commands>()
			.Run(args);
	}
}

class Commands
{
	/// <summary>The list to manage.</summary>
	public string List { get; init; } = "main";

	/// <summary>Adds a new item to the list.</summary>
	/// <param name="item">The item to add to the list.</param>
	public void Add(string item) => throw new NotImplementedException();

	/// <summary>Removes an existing item from the list.</summary>
	/// <param name="item">The item to remove from the list.</param>
	public void Remove(string item) => throw new NotImplementedException();
}
```

Turning into a CLI that looks a little like this:

```
> list-manager -h

NAME
	list-manager									A CLI that manages lists.

SUMMARY
	list-manager add <item>							Adds a new item to the list.
	list-manager remove <item>						Removes an existing item from the list.

GLOBAL FLAGS
	-l --list										The list to manage. [Default: main]

COMMANDS
	list-manager add <item>							Adds a new item to the list.
		ARGUMENTS
			item									The item to add to the list.

	list-manager remove <item>						Removes an existing item from the list.
		ARGUMENTS
			item									The item to remove from the list.
```

## Features

### Supported

Currently there's no supported features since I didn't even write any code yet, I'm just making the initial readme.


### Planned

- Support for short flags:
	- Customisable prefix, defaults to `-`.
	- Boolean toggle flags (on/off) `-a -b -c`. (explicit values not allowed).
	- Chaining boolean toggle flags `-abc`. (explicit values not allowed).
	- Integer repeat type flags (longer repeat result in bigger value) `-v` or `-vv` or `-vvvvv`. (explicit values not allowed).
	- Allow specifying explicit values such as `-f file` or `-f=file`, or `-f=file1,fil2`.

- Support for long flags:
	- Customisable prefix, defaults to `--`.
	- Allows longer names such as `--dostuff` or `--do-stuff`.
	- Allows specifying values such as `--files file` or `--files=file`, or `--files=file1,file2`.

- Windows style syntax for flags *(which is quite limiting)*:
	- Uses `/` instead of `-` and `--` for specifying flags.
	- No separation between short and long flags.
	- No chaining.
	- No integer repeat type flags.
	- Use `:` instead of `=` for specifying flag values.

- Support for explicitly separation flags and arguments:
	- Customisable character, defaults to `--` *(alone on it's own, not as a prefix like the long flags)*.
	- e.g. `foo --some-flag -- --this-is-an-argument-not-a-flag`.

- File/directory/path support:
	- can parse the paths into [`FileInfo`](https://learn.microsoft.com/dotnet/api/system.io.fileinfo), [`DirectoryInfo`](https://learn.microsoft.com/dotnet/api/system.io.directoryinfo) and [`FileSystemInfo`](https://learn.microsoft.com/dotnet/api/system.io.filesysteminfo).
	- Can parse the path into a regular `string` value (attribute will be required to specify it's a path).
	- Validators for ensuring that the file/directory path exists, or does not exist.
	- Automatic conversion of special directories like `.` *(current directory)* and `..` *(parent directory)*.
	- Support for absolute and relative paths.

- Implicit help flags and commands:
	- Implicit `-h` and `--help` commands (`-?` and `/?` for Windows style syntax).
	- Implicit `help` command in every command group (customisable).

- String parsing support:
	- `"` quoted strings.
	- `'` quoted strings.
	- Unquoted strings.
	- Regex parsing & regex validators.

- Boolean parsing support:
	- Strict `true` / `false` support.
	- Loose `t` /  `f` support.
	- Strict `yes` / `no` support.
	- Loose `y` / `n` support.
	- Fun `ye+a*h*` `na+h*` support *(not enabled by default, don't worry)*.

- Numeric parsing support:
	- Binary and hexadecimal prefixes `0b` and `0x`.
	- Octal prefix `0o` *(does anyone even use octal anymore?)*.
	- Number separators (defaults to `_`, customisable).
	- File size suffix parsing, (`B` or `KB` or `KiB` e.t.c).
	- Time span suffix parsing, (`s` or `m` or `h` e.t.c).

- [`IPAddress`](https://learn.microsoft.com/dotnet/api/system.net.ipaddress) parsing:
	- IPv4 support.
	- IPv6 support.
	- [`IPEndPoint`](https://learn.microsoft.com/dotnet/api/system.net.ipendpoint) support (IP + port).

- sub-commands & command groups.
- Type parsers (customisable).
- Value validators (customisable).

## Contributions

Code contributions will not be accepted, however feel free to provide feedback / suggestions
by creating a [new issue](https://github.com/Owl-Domain/CommandLine/issues/new), or look at
the [existing issues](https://github.com/Owl-Domain/CommandLine/issues?q=) to see if your
concern / suggestion has already been raised.


## License

This project (the source, the release files, e.t.c) is released under the
[OwlDomain License](https://github.com/Owl-Domain/CommandLine/blob/master/license.md).
