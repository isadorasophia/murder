# BlackboardInfo

**Namespace:** Murder.Data \
**Assembly:** Murder.dll

```csharp
public class BlackboardInfo : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
protected BlackboardInfo(BlackboardInfo original)
```

**Parameters** \
`original` [BlackboardInfo](../../Murder/Data/BlackboardInfo.html) \

```csharp
public BlackboardInfo(Type Type, IBlackboard Blackboard)
```

**Parameters** \
`Type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`Blackboard` [IBlackboard](../../Murder/Core/Dialogs/IBlackboard.html) \

### ⭐ Properties
#### Blackboard
```csharp
public IBlackboard Blackboard { get; public set; }
```

**Returns** \
[IBlackboard](../../Murder/Core/Dialogs/IBlackboard.html) \
#### EqualityContract
```csharp
protected virtual Type EqualityContract { get; }
```

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
#### Type
```csharp
public Type Type { get; public set; }
```

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
### ⭐ Methods
#### PrintMembers(StringBuilder)
```csharp
protected virtual bool PrintMembers(StringBuilder builder)
```

**Parameters** \
`builder` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/System.Text.StringBuilder?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(BlackboardInfo)
```csharp
public virtual bool Equals(BlackboardInfo other)
```

**Parameters** \
`other` [BlackboardInfo](../../Murder/Data/BlackboardInfo.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Object)
```csharp
public virtual bool Equals(Object obj)
```

**Parameters** \
`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode()
```csharp
public virtual int GetHashCode()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Deconstruct(out Type&, out IBlackboard&)
```csharp
public void Deconstruct(Type& Type, IBlackboard& Blackboard)
```

**Parameters** \
`Type` [Type&](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`Blackboard` [IBlackboard&](../../Murder/Core/Dialogs/IBlackboard.html) \



⚡