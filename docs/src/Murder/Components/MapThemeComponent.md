# MapThemeComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct MapThemeComponent : IComponent
```

This is a struct that points to a singleton class.
            Reactive systems won't be able to subscribe to this component.

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public MapThemeComponent(Guid tileset, Guid floor)
```

**Parameters** \
`tileset` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`floor` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### Floor
```csharp
public readonly Guid Floor;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Tileset
```csharp
public readonly Guid Tileset;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \


⚡