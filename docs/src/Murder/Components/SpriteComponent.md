# SpriteComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct SpriteComponent : IComponent
```

**Implements:** _[IComponent](../..//Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public SpriteComponent()
```

```csharp
public SpriteComponent(Portrait portrait)
```

**Parameters** \
`portrait` [Portrait](../..//Murder/Core/Portrait.html) \

```csharp
public SpriteComponent(Guid guid, Vector2 offset, ImmutableArray<T> id, int ySortOffset, bool rotate, bool flip, OutlineStyle highlightStyle, float startTime, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`offset` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`id` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`ySortOffset` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`rotate` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`flip` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`highlightStyle` [OutlineStyle](../..//Murder/Core/Graphics/OutlineStyle.html) \
`startTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](../..//Murder/Core/Graphics/TargetSpriteBatches.html) \

```csharp
public SpriteComponent(Guid guid, Vector2 offset, string id, int ySortOffset, bool backAnim, bool flip, OutlineStyle highlightStyle, float startTime, TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`offset` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ySortOffset` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`backAnim` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`flip` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`highlightStyle` [OutlineStyle](../..//Murder/Core/Graphics/OutlineStyle.html) \
`startTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`targetSpriteBatch` [TargetSpriteBatches](../..//Murder/Core/Graphics/TargetSpriteBatches.html) \

### ⭐ Properties
#### AnimationGuid
```csharp
public readonly Guid AnimationGuid;
```

The Guid of the Aseprite file.

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### AnimationStartedTime
```csharp
public readonly float AnimationStartedTime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### CurrentAnimation
```csharp
public string CurrentAnimation { get; }
```

Current playing animation id.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FlipWithFacing
```csharp
public readonly bool FlipWithFacing;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### HighlightStyle
```csharp
public readonly OutlineStyle HighlightStyle;
```

**Returns** \
[OutlineStyle](../..//Murder/Core/Graphics/OutlineStyle.html) \
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

(0,0) is top left and (1,1) is bottom right

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### RotateWithFacing
```csharp
public readonly bool RotateWithFacing;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TargetSpriteBatch
```csharp
public readonly TargetSpriteBatches TargetSpriteBatch;
```

**Returns** \
[TargetSpriteBatches](../..//Murder/Core/Graphics/TargetSpriteBatches.html) \
#### UseUnscaledTime
```csharp
public readonly bool UseUnscaledTime;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### YSortOffset
```csharp
public readonly int YSortOffset;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### HasAnimation(string)
```csharp
public bool HasAnimation(string animationName)
```

**Parameters** \
`animationName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsPlaying(string)
```csharp
public bool IsPlaying(string animationName)
```

**Parameters** \
`animationName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsPlaying(String[])
```csharp
public bool IsPlaying(String[] animations)
```

**Parameters** \
`animations` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Play(bool, ImmutableArray<T>)
```csharp
public SpriteComponent Play(bool useScaledTime, ImmutableArray<T> id)
```

**Parameters** \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`id` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### Play(bool, String[])
```csharp
public SpriteComponent Play(bool useScaledTime, String[] id)
```

**Parameters** \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`id` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### PlayAfter(string)
```csharp
public SpriteComponent PlayAfter(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### PlayOnce(string, bool)
```csharp
public SpriteComponent PlayOnce(string id, bool useScaledTime)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### Reset()
```csharp
public SpriteComponent Reset()
```

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### SetBatch(TargetSpriteBatches)
```csharp
public SpriteComponent SetBatch(TargetSpriteBatches batch)
```

**Parameters** \
`batch` [TargetSpriteBatches](../..//Murder/Core/Graphics/TargetSpriteBatches.html) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \

#### WithSort(int)
```csharp
public SpriteComponent WithSort(int sort)
```

**Parameters** \
`sort` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[SpriteComponent](../..//Murder/Components/SpriteComponent.html) \



⚡