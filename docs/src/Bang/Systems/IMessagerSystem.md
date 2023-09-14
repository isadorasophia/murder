# IMessagerSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IMessagerSystem : ISystem
```

A reactive system that reacts to messages getting added to an entity.

**Implements:** _[ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Methods
#### OnMessage(World, Entity, IMessage)
```csharp
public abstract void OnMessage(World world, Entity entity, IMessage message)
```

Called once a message is fired from <paramref name="entity" />.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entity` [Entity](../../Bang/Entities/Entity.html) \
`message` [IMessage](../../Bang/Components/IMessage.html) \



⚡