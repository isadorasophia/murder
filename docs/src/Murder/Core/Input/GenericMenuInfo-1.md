# GenericMenuInfo\<T\>

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public sealed struct GenericMenuInfo<T>
```

### ⭐ Constructors
```csharp
public GenericMenuInfo<T>(T[] options)
```

**Parameters** \
`options` [T[]](../../../) \

### ⭐ Properties
#### Canceled
```csharp
public bool Canceled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Disabled
```csharp
public bool Disabled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### JustMoved
```csharp
public bool JustMoved;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### LastMoved
```csharp
public float LastMoved;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LastPressed
```csharp
public float LastPressed;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Length
```csharp
public int Length { get; }
```

Number of options in this menu

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Options
```csharp
public T[] Options;
```

**Returns** \
[T[]](../../../) \
#### Overflow
```csharp
public int Overflow;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### PreviousSelection
```csharp
public int PreviousSelection;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Scroll
```csharp
public int Scroll;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Selection
```csharp
public int Selection { get; private set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### VisibleItems
```csharp
public int VisibleItems;
```

Number of visible options on the screen, 8 is the default.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### NextAvailableOption(int, int)
```csharp
public int NextAvailableOption(int option, int direction)
```

**Parameters** \
`option` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`direction` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

#### Select(int, float)
```csharp
public void Select(int index, float now)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`now` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Select(int)
```csharp
public void Select(int index)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡