# AnimationOverloadComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct AnimationOverloadComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AnimationOverloadComponent()
```

```csharp
public AnimationOverloadComponent(ImmutableArray<T> animationId, Guid customSprite, float start, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`customSprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`start` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public AnimationOverloadComponent(ImmutableArray<T> animations, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite, float start)
```

**Parameters** \
`animations` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`current` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sortOffset` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`customSprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`start` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public AnimationOverloadComponent(ImmutableArray<T> animations, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite)
```

**Parameters** \
`animations` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`current` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sortOffset` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`customSprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`current` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sortOffset` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`customSprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, Guid customSprite, float start, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`customSprite` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`start` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### AnimationCount
```csharp
public int AnimationCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### AnimationId
```csharp
public string AnimationId { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### AtLast
```csharp
public bool AtLast { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Current
```csharp
public readonly int Current;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### CurrentAnimation
```csharp
public string CurrentAnimation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### CustomSprite
```csharp
public SpriteAsset CustomSprite { get; }
```

**Returns** \
[SpriteAsset](../../Murder/Assets/Graphics/SpriteAsset.html) \
#### Duration
```csharp
public readonly float Duration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### IgnoreFacing
```csharp
public readonly bool IgnoreFacing;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Loop
```csharp
public readonly bool Loop;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### NoLoop
```csharp
public AnimationOverloadComponent NoLoop { get; }
```

**Returns** \
[AnimationOverloadComponent](../../Murder/Components/AnimationOverloadComponent.html) \
#### Now
```csharp
public AnimationOverloadComponent Now { get; }
```

**Returns** \
[AnimationOverloadComponent](../../Murder/Components/AnimationOverloadComponent.html) \
#### SortOffset
```csharp
public readonly float SortOffset;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Start
```csharp
public readonly float Start;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### PlayNext()
```csharp
public AnimationOverloadComponent PlayNext()
```

**Returns** \
[AnimationOverloadComponent](../../Murder/Components/AnimationOverloadComponent.html) \



⚡