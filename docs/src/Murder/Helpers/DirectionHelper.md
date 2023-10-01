# DirectionHelper

**Namespace:** Murder.Helpers \
**Assembly:** Murder.dll

```csharp
public static class DirectionHelper
```

### ⭐ Properties
#### Cardinal
```csharp
public static ImmutableArray<T> Cardinal;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Cardinal4Flipped
```csharp
public static ImmutableArray<T> Cardinal4Flipped;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### CardinalFlipped
```csharp
public static ImmutableArray<T> CardinalFlipped;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### Flipped(Direction)
```csharp
public bool Flipped(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FromAngle(float)
```csharp
public Direction FromAngle(float angle)
```

**Parameters** \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### FromVector(Vector2)
```csharp
public Direction FromVector(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### FromVectorWith4Directions(Vector2)
```csharp
public Direction FromVectorWith4Directions(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### Invert(Direction)
```csharp
public Direction Invert(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### LookAtEntity(Entity, Entity)
```csharp
public Direction LookAtEntity(Entity e, Entity target)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`target` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### LookAtPosition(Entity, Vector2)
```csharp
public Direction LookAtPosition(Entity e, Vector2 target)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`target` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### Random()
```csharp
public Direction Random()
```

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### RandomCardinal()
```csharp
public Direction RandomCardinal()
```

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### Reverse(Direction)
```csharp
public Direction Reverse(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### RoundTo4Directions(Direction, Orientation)
```csharp
public Direction RoundTo4Directions(Direction direction, Orientation bias)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \
`bias` [Orientation](../../Murder/Core/Orientation.html) \

**Returns** \
[Direction](../../Murder/Helpers/Direction.html) \

#### Angle(Direction)
```csharp
public float Angle(Direction direction)
```

The angle of the direction, in radians.

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### GetFlipped(Direction)
```csharp
public SpriteEffects GetFlipped(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[SpriteEffects](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteEffects.html) \

#### ToCardinal(Direction, string, string, string, string)
```csharp
public string ToCardinal(Direction direction, string n, string e, string s, string w)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \
`n` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`e` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`s` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`w` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ToCardinal(Direction)
```csharp
public string ToCardinal(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ToCardinal4(Direction, string, string, string, bool)
```csharp
public string ToCardinal4(Direction direction, string n, string e, string s, bool verticalPriority)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \
`n` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`e` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`s` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`verticalPriority` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetSuffixFromAngle(AgentSpriteComponent, float)
```csharp
public ValueTuple<T1, T2> GetSuffixFromAngle(AgentSpriteComponent sprite, float angle)
```

Get the suffix from a suffix list based on an angle

**Parameters** \
`sprite` [AgentSpriteComponent](../../Murder/Components/AgentSpriteComponent.html) \
\
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \
\

#### ToCardinalFlipped(Direction, string, string, string)
```csharp
public ValueTuple<T1, T2> ToCardinalFlipped(Direction direction, string n, string e, string s)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \
`n` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`e` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`s` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### ToCardinalFlipped(Direction)
```csharp
public ValueTuple<T1, T2> ToCardinalFlipped(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### ToVector(Direction)
```csharp
public Vector2 ToVector(Direction direction)
```

**Parameters** \
`direction` [Direction](../../Murder/Helpers/Direction.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \



⚡