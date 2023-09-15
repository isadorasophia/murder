# InteractiveComponent\<T\>

**Namespace:** Bang.Interactions \
**Assembly:** Bang.dll

```csharp
public sealed struct InteractiveComponent<T> : IInteractiveComponent, IComponent, IModifiableComponent
```

Implements an interaction component which will be passed on to the entity.

**Implements:** _[IInteractiveComponent](../../Bang/Interactions/IInteractiveComponent.html), [IComponent](../../Bang/Components/IComponent.html), [IModifiableComponent](../../Bang/Components/IModifiableComponent.html)_

### ⭐ Constructors
```csharp
public InteractiveComponent<T>()
```

Default constructor, initializes a brand new interaction.

```csharp
public InteractiveComponent<T>(T interaction)
```

**Parameters** \
`interaction` [T](../../) \

### ⭐ Methods
#### Interact(World, Entity, Entity)
```csharp
public virtual void Interact(World world, Entity interactor, Entity interacted)
```

Calls the inner interaction component.

**Parameters** \
`world` [World](../../Bang/World.html) \
`interactor` [Entity](../../Bang/Entities/Entity.html) \
`interacted` [Entity](../../Bang/Entities/Entity.html) \

#### Subscribe(Action)
```csharp
public virtual void Subscribe(Action notification)
```

Subscribe for notifications on this component.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### Unsubscribe(Action)
```csharp
public virtual void Unsubscribe(Action notification)
```

Stop listening to notifications on this component.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \



⚡