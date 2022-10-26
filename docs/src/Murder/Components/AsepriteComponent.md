# AsepriteComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct AsepriteComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AsepriteComponent()
```

```csharp
public AsepriteComponent(Guid guid, Vector2 offset, ImmutableArray<T> id, int ySortOffset, bool backAnim, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`id` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`ySortOffset` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`backAnim` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \

```csharp
public AsepriteComponent(Guid guid, Vector2 offset, ImmutableArray<T> id, int ySortOffset, bool backAnim, float time, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`id` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`ySortOffset` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`backAnim` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \

```csharp
public AsepriteComponent(Guid guid, Vector2 offset, string id, int ySortOffset, bool backAnim, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ySortOffset` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`backAnim` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \

```csharp
public AsepriteComponent(Guid guid, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \

### ⭐ Properties
#### AnimationGuid
```csharp
public readonly Guid AnimationGuid;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### AnimationId
```csharp
public readonly string AnimationId;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### AnimationStartedTime
```csharp
public readonly float AnimationStartedTime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### HasBackAnimations
```csharp
public readonly bool HasBackAnimations;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### NextAnimations
```csharp
public readonly ImmutableArray<T> NextAnimations;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Offset
```csharp
public readonly Vector2 Offset;
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### TargetSpriteBatch
```csharp
public readonly TargetSpriteBatches TargetSpriteBatch;
```

**Returns** \
[TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \
#### YSortOffset
```csharp
public readonly int YSortOffset;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Play(ImmutableArray<T>)
```csharp
public AsepriteComponent Play(ImmutableArray<T> id)
```

**Parameters** \
`id` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### Play(string)
```csharp
public AsepriteComponent Play(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### Play(String[])
```csharp
public AsepriteComponent Play(String[] id)
```

**Parameters** \
`id` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### PlayAfter(string)
```csharp
public AsepriteComponent PlayAfter(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### PlayOnce(string)
```csharp
public AsepriteComponent PlayOnce(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### WithSort(int)
```csharp
public AsepriteComponent WithSort(int sort)
```

**Parameters** \
`sort` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[AsepriteComponent](/Murder/Components/AsepriteComponent.html) \



⚡