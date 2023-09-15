# StringHelper

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class StringHelper
```

### ⭐ Methods
#### FuzzyMatch(string, string)
```csharp
public bool FuzzyMatch(string searchTerm, string target)
```

**Parameters** \
`searchTerm` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`target` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LevenshteinDistance(string, string)
```csharp
public int LevenshteinDistance(string s, string t)
```

**Parameters** \
`s` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`t` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### CapitalizeFirstLetter(string)
```csharp
public string CapitalizeFirstLetter(string input)
```

**Parameters** \
`input` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Cleanup(string)
```csharp
public string Cleanup(string input)
```

Removes single returns, keeps doubles.

**Parameters** \
`input` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

#### GetDescription(T)
```csharp
public string GetDescription(T enumerationValue)
```

**Parameters** \
`enumerationValue` [T](../../) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ToHumanList(IEnumerable<T>, string, string)
```csharp
public string ToHumanList(IEnumerable<T> someStringArray, string separator, string lastItemSeparator)
```

**Parameters** \
`someStringArray` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`separator` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`lastItemSeparator` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetAttribute(T)
```csharp
public TAttr GetAttribute(T enumerationValue)
```

**Parameters** \
`enumerationValue` [T](../../) \

**Returns** \
[TAttr](../../) \



⚡