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
public Dialog(bool playOnce, ImmutableArray<T> requirements, ImmutableArray<T> lines, T? actions, T? goto)
```

**Parameters** \
`playOnce` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`lines` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`actions` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`goto` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

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
#### Lines
```csharp
public readonly ImmutableArray<T> Lines;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### PlayOnce
```csharp
public readonly bool PlayOnce;
```

Stop playing this dialog after playing it for an amount of times.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### AddLine(Line)
```csharp
public Dialog AddLine(Line line)
```

**Parameters** \
`line` [Line](/Murder/Core/Dialogs/Line.html) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### AddRequirement(Criterion)
```csharp
public Dialog AddRequirement(Criterion requirement)
```

**Parameters** \
`requirement` [Criterion](/Murder/Core/Dialogs/Criterion.html) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### FlipPlayOnce()
```csharp
public Dialog FlipPlayOnce()
```

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### ReorderLineAt(int, int)
```csharp
public Dialog ReorderLineAt(int previousIndex, int newIndex)
```

**Parameters** \
`previousIndex` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`newIndex` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### SetLineAt(int, Line)
```csharp
public Dialog SetLineAt(int index, Line line)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`line` [Line](/Murder/Core/Dialogs/Line.html) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### WithActions(T?)
```csharp
public Dialog WithActions(T? actions)
```

**Parameters** \
`actions` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### WithGoTo(T?)
```csharp
public Dialog WithGoTo(T? goto)
```

**Parameters** \
`goto` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### WithLines(ImmutableArray<T>)
```csharp
public Dialog WithLines(ImmutableArray<T> lines)
```

**Parameters** \
`lines` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \

#### WithRequirements(ImmutableArray<T>)
```csharp
public Dialog WithRequirements(ImmutableArray<T> requirements)
```

**Parameters** \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[Dialog](/Murder/Core/Dialogs/Dialog.html) \



⚡