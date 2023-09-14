# InteractOnCollisionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct InteractOnCollisionComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public InteractOnCollisionComponent()
```

```csharp
public InteractOnCollisionComponent(bool playerOnly, bool sendMessageOnExit)
```

**Parameters** \
`playerOnly` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`sendMessageOnExit` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public InteractOnCollisionComponent(bool playerOnly)
```

**Parameters** \
`playerOnly` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### CustomEnterMessages
```csharp
public readonly ImmutableArray<T> CustomEnterMessages;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### CustomExitMessages
```csharp
public readonly ImmutableArray<T> CustomExitMessages;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### OnlyOnce
```csharp
public readonly bool OnlyOnce;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### PlayerOnly
```csharp
public readonly bool PlayerOnly;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### SendMessageOnExit
```csharp
public readonly bool SendMessageOnExit;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### SendMessageOnStay
```csharp
public readonly bool SendMessageOnStay;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \


⚡