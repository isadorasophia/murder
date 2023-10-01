# OnTriggerEnteredMessage

**Namespace:** Murder.Messages.Physics \
**Assembly:** Murder.dll

```csharp
public sealed struct OnTriggerEnteredMessage : IMessage
```

Message sent to the ACTOR when touching a trigger area.

**Implements:** _[IMessage](../../../Bang/Components/IMessage.html)_

### ⭐ Constructors
```csharp
public OnTriggerEnteredMessage(int triggerId, CollisionDirection movement)
```

Message sent to the ACTOR when touching a trigger area.

**Parameters** \
`triggerId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`movement` [CollisionDirection](../../../Murder/Utilities/CollisionDirection.html) \

### ⭐ Properties
#### EntityId
```csharp
public readonly int EntityId;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Movement
```csharp
public readonly CollisionDirection Movement;
```

**Returns** \
[CollisionDirection](../../../Murder/Utilities/CollisionDirection.html) \


⚡