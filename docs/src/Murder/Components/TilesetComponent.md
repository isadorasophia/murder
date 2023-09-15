# TilesetComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct TilesetComponent : IComponent
```

This is a struct that points to a singleton class.
            Reactive systems won't be able to subscribe to this component.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public TilesetComponent()
```

```csharp
public TilesetComponent(ImmutableArray<T> tilesets)
```

**Parameters** \
`tilesets` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

### ⭐ Properties
#### Tilesets
```csharp
public readonly ImmutableArray<T> Tilesets;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### WithTile(Guid)
```csharp
public TilesetComponent WithTile(Guid tile)
```

**Parameters** \
`tile` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[TilesetComponent](../../Murder/Components/TilesetComponent.html) \

#### WithTiles(ImmutableArray<T>)
```csharp
public TilesetComponent WithTiles(ImmutableArray<T> tiles)
```

**Parameters** \
`tiles` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[TilesetComponent](../../Murder/Components/TilesetComponent.html) \



⚡