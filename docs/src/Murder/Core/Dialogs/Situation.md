# Situation

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct Situation
```

### ⭐ Constructors
```csharp
public Situation()
```

```csharp
public Situation(int id, string name, ImmutableArray<T> dialogs)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`dialogs` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

```csharp
public Situation(int id, string name)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Dialogs
```csharp
public readonly ImmutableArray<T> Dialogs;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Id
```csharp
public readonly int Id;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### RemoveDialogAt(int)
```csharp
public Situation RemoveDialogAt(int index)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Situation](/Murder/Core/Dialogs/Situation.html) \

#### ReorderDialogAt(int, int)
```csharp
public Situation ReorderDialogAt(int previousIndex, int newIndex)
```

**Parameters** \
`previousIndex` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`newIndex` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Situation](/Murder/Core/Dialogs/Situation.html) \

#### WithDialogAt(int, Dialog)
```csharp
public Situation WithDialogAt(int index, Dialog dialog)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`dialog` [Dialog](/Murder/Core/Dialogs/Dialog.html) \

**Returns** \
[Situation](/Murder/Core/Dialogs/Situation.html) \

#### WithName(string)
```csharp
public Situation WithName(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Situation](/Murder/Core/Dialogs/Situation.html) \

#### WithNewDialog(Dialog)
```csharp
public Situation WithNewDialog(Dialog dialog)
```

**Parameters** \
`dialog` [Dialog](/Murder/Core/Dialogs/Dialog.html) \

**Returns** \
[Situation](/Murder/Core/Dialogs/Situation.html) \



⚡