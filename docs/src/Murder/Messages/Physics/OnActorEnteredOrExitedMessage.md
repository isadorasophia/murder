# OnActorEnteredOrExitedMessage

**Namespace:** Murder.Messages.Physics \
**Assembly:** Murder.dll

```csharp
public sealed struct OnActorEnteredOrExitedMessage : IMessage
```

Message sent to the TRIGGER when touching an actor touches it.

**Implements:** _[IMessage](../../../Bang/Components/IMessage.html)_

### ⭐ Constructors
```csharp
public OnActorEnteredOrExitedMessage(int actorId, CollisionDirection movement)
```

Message sent to the TRIGGER when touching an actor touches it.

**Parameters** \
`actorId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
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