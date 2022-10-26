# GridCollisionType

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public sealed enum GridCollisionType : Enum, IComparable, IFormattable, IConvertible
```

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### BlockVision
```csharp
public static const GridCollisionType BlockVision;
```

These should block any line of sight from going trough them

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \
#### Carve
```csharp
public static const GridCollisionType Carve;
```

These are the "carve" types.
            This will be any grid occupied by an entity which has a collider of a "carve" type.

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \
#### IsObstacle
```csharp
public static const GridCollisionType IsObstacle;
```

Whether this is an obstacle for pathfinding.

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \
#### None
```csharp
public static const GridCollisionType None;
```

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \
#### Static
```csharp
public static const GridCollisionType Static;
```

These are static collision grids.
            This will be any grid occupied by a tile (e.g. wall), for example.

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \


⚡