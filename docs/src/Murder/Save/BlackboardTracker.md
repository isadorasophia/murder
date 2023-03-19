# BlackboardTracker

**Namespace:** Murder.Save \
**Assembly:** Murder.dll

```csharp
public class BlackboardTracker
```

Track variables that contain the state of the world.

### ⭐ Constructors
```csharp
public BlackboardTracker()
```

### ⭐ Methods
#### GetBool(string, string, T?)
```csharp
public bool GetBool(string name, string fieldName, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasPlayed(Guid, int, int)
```csharp
public bool HasPlayed(Guid guid, int situationId, int dialogId)
```

Returns whether a particular dialog option has been played.

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`situationId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`dialogId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasVariable(string, string)
```csharp
public bool HasVariable(string blackboardName, string fieldName)
```

Return whether a <paramref name="fieldName" /> exists on <paramref name="blackboardName" />.

**Parameters** \
`blackboardName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Matches(Criterion, T?, World, T?, out Int32&)
```csharp
public bool Matches(Criterion criterion, T? character, World world, T? entityId, Int32& weight)
```

**Parameters** \
`criterion` [Criterion](/Murder/Core/Dialogs/Criterion.html) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`world` [World](/Bang/World.html) \
`entityId` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`weight` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetInt(string, string, T?)
```csharp
public int GetInt(string name, string fieldName, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### PlayCount(Guid, int, int)
```csharp
public int PlayCount(Guid guid, int situationId, int dialogId)
```

Returns whether how many times a dialog has been executed.

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`situationId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`dialogId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### GetString(string, string, T?)
```csharp
public string GetString(string name, string fieldName, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetValueAsString(string)
```csharp
public string GetValueAsString(string fieldName)
```

Get a blackboard value as a string. This returns the first blackboard that has the field.

**Parameters** \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### FetchBlackboards()
```csharp
public virtual ImmutableDictionary<TKey, TValue> FetchBlackboards()
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### FindBlackboard(string, T?)
```csharp
public virtual ValueTuple<T1, T2> FindBlackboard(string name, T? guid)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`guid` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### Track(Guid, int, int)
```csharp
public virtual void Track(Guid character, int situationId, int dialogId)
```

Track that a particular dialog option has been played.

**Parameters** \
`character` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`situationId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`dialogId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### OnModified()
```csharp
public void OnModified()
```

Notify that the blackboard has been changed (externally or internally).

#### ResetWatchers()
```csharp
public void ResetWatchers()
```

This will reset all watchers of trackers.

#### SetBool(string, string, bool, T?)
```csharp
public void SetBool(string name, string fieldName, bool value, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`value` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### SetInt(string, string, BlackboardActionKind, int, T?)
```csharp
public void SetInt(string name, string fieldName, BlackboardActionKind kind, int value, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`kind` [BlackboardActionKind](/Murder/Core/Dialogs/BlackboardActionKind.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### SetString(string, string, string, T?)
```csharp
public void SetString(string name, string fieldName, string value, T? character)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`value` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`character` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Watch(Action)
```csharp
public void Watch(Action notification)
```

This will watch any chages to any of the blackboard properties.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \



⚡