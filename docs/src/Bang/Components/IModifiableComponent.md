# IModifiableComponent

**Namespace:** Bang.Components \
**Assembly:** Bang.dll

```csharp
public abstract IModifiableComponent : IComponent
```

This is for a component that can be modified and is not an actual immutable.

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Methods
#### Subscribe(Action)
```csharp
public abstract void Subscribe(Action notification)
```

Subscribe to receive notifications when the component gets modified.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### Unsubscribe(Action)
```csharp
public abstract void Unsubscribe(Action notification)
```

Unsubscribe to receive notifications when the component gets modified.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \



⚡