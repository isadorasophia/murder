# Dialog

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct Dialog
```

### ⭐ Constructors
```csharp
public Dialog()
```

```csharp
public Dialog(int id, int playUntil, ImmutableArray<T> requirements, ImmutableArray<T> lines, T? actions, T? goto, bool isChoice)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`playUntil` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`lines` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`actions` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`goto` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`isChoice` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### Actions
```csharp
public readonly T? Actions;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### GoTo
```csharp
public readonly T? GoTo;
```

Go to another dialog with a specified id.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Id
```csharp
public readonly int Id;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### IsChoice
```csharp
public readonly bool IsChoice;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Lines
```csharp
public readonly ImmutableArray<T> Lines;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### PlayUntil
```csharp
public readonly int PlayUntil;
```

Stop playing this dialog until this number.
            If -1, this will play forever.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### WithActions(T?)
```csharp
public Dialog WithActions(T? actions)
```

**Parameters** \
`actions` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[Dialog](../../../Murder/Core/Dialogs/Dialog.html) \

#### WithLineAt(int, Line)
```csharp
public Dialog WithLineAt(int index, Line line)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`line` [Line](../../../Murder/Core/Dialogs/Line.html) \

**Returns** \
[Dialog](../../../Murder/Core/Dialogs/Dialog.html) \

#### DebuggerDisplay()
```csharp
public string DebuggerDisplay()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡