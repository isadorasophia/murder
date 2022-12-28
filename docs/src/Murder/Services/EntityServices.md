# EntityServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class EntityServices
```

### ⭐ Methods
#### IsChildOf(World, Entity, Entity)
```csharp
public bool IsChildOf(World world, Entity parent, Entity child)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`parent` [Entity](/Bang/Entities/Entity.html) \
`child` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FindRootEntity(Entity)
```csharp
public Entity FindRootEntity(Entity e)
```

**Parameters** \
`e` [Entity](/Bang/Entities/Entity.html) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### PlayAsepriteAnimation(Entity, ImmutableArray<T>)
```csharp
public T? PlayAsepriteAnimation(Entity entity, ImmutableArray<T> animations)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`animations` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### PlayAsepriteAnimation(Entity, String[])
```csharp
public T? PlayAsepriteAnimation(Entity entity, String[] nextAnimations)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`nextAnimations` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### PlayAsepriteAnimationNext(Entity, string)
```csharp
public T? PlayAsepriteAnimationNext(Entity entity, string animationName)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`animationName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### TryPlayAsepriteAnimation(Entity, String[])
```csharp
public T? TryPlayAsepriteAnimation(Entity entity, String[] nextAnimations)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`nextAnimations` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### TryPlayAsepriteAnimationNext(Entity, string)
```csharp
public T? TryPlayAsepriteAnimationNext(Entity entity, string animationName)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`animationName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### RotateChildPositions(World, Entity, float)
```csharp
public void RotateChildPositions(World world, Entity entity, float angle)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`entity` [Entity](/Bang/Entities/Entity.html) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### RotatePosition(Entity, float)
```csharp
public void RotatePosition(Entity entity, float angle)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### RotatePositionAround(Entity, Vector2, float)
```csharp
public void RotatePositionAround(Entity entity, Vector2 center, float angle)
```

**Parameters** \
`entity` [Entity](/Bang/Entities/Entity.html) \
`center` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡