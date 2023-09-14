# AnimationEventMessage

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct AnimationEventMessage : IMessage
```

**Implements:** _[IMessage](../../../Bang/Components/IMessage.html)_

### ⭐ Constructors
```csharp
public AnimationEventMessage(string eventId)
```

**Parameters** \
`eventId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Event
```csharp
public readonly string Event;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### Is(ReadOnlySpan<T>)
```csharp
public bool Is(ReadOnlySpan<T> eventId)
```

**Parameters** \
`eventId` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Is(string)
```csharp
public bool Is(string eventId)
```

**Parameters** \
`eventId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡