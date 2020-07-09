# LiveLinq

[![Gitpod ready-to-code](https://img.shields.io/badge/Gitpod-ready--to--code-blue?logo=gitpod)](https://gitpod.io/#https://github.com/nloum/LiveLinq) ![dotnetcore](https://img.shields.io/github/workflow/status/nloum/LiveLinq/dotnetcore) ![nuget](https://img.shields.io/nuget/v/LiveLinq) ![license](https://img.shields.io/github/license/nloum/LiveLinq) [![Join the chat at https://gitter.im/LiveLinq/community](https://badges.gitter.im/LiveLinq/community.svg)](https://gitter.im/LiveLinq/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

LiveLinq is the streaming LINQ library for .NET. The syntax looks mostly like standard LINQ, but it efficiently recalculates the resulting collections when the source collection changes.

For example:

```
var source = new ObservableCollection<int>();
source.Add(3);
source.Add(4);

var evenNumbers = source.ToLiveLinq()
                        .Where(i => i % 2 == 0)
                        .ToReadOnlyObservableList();

// This will print out 4
Console.WriteLine(string.Join(", ", evenNumbers.Select(i => i.ToString()));

source.Add(5);
source.Add(6);

// This will print out 4, 6
Console.WriteLine(string.Join(", ", evenNumbers.Select(i => i.ToString()));
```

In this example, the lambda `i => i % 2 == 0` only runs four times: once with `i = 3`, once with `i = 4`, once with `i = 5`, and once with `i = 6`.

Here's a similar example with plain LINQ:

```
var source = new ObservableCollection<int>();
source.Add(3);
source.Add(4);

var evenNumbers = source.Where(i => i % 2 == 0)
                        .ToReadOnlyObservableList();

// This will print out 4
Console.WriteLine(string.Join(", ", evenNumbers.Select(i => i.ToString()));

source.Add(5);
source.Add(6);

// Now we need to recalculate evenNumbers, something we don't have to do with LiveLinq
evenNumbers = source.Where(i => i % 2 == 0)
                    .ToReadOnlyObservableList();

// This will print out 4, 6
Console.WriteLine(string.Join(", ", evenNumbers.Select(i => i.ToString()));
```

In this example, the lambda `i => i % 2 == 0` gets called 6 times. First it's called with `i = 3`, once with `i = 4` when we're first defining the `evenNumbers` variable. Then it's called with `i` being `3`, `4`, `5`, and `6` again when we recalculate the `evenNumbers` variable.

This is how LiveLinq lets you work much more efficiently with complex LINQ statements on changing collections.

Most of the LINQ operators are supported: `SelectMany`, `Where`, `GroupBy`, etc. Also, different types of collections are supported: hashes, lists, sets, and dictionaries.
