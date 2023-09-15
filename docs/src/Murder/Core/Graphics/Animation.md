# Animation

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct Animation
```

### ⭐ Constructors
```csharp
public Animation()
```

```csharp
public Animation(Int32[] frames, Single[] framesDuration, Dictionary<TKey, TValue> events, T? sequence)
```

**Parameters** \
`frames` [int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`framesDuration` [float[]](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`events` [Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
`sequence` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

### ⭐ Properties
#### AnimationDuration
```csharp
public readonly float AnimationDuration;
```

The total duration of the animation, in seconds

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Events
```csharp
public readonly ImmutableDictionary<TKey, TValue> Events;
```

A dictionary associating integer indices with event strings

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### FrameCount
```csharp
public int FrameCount { get; }
```

A property representing the number of frames in the animation

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Frames
```csharp
public readonly ImmutableArray<T> Frames;
```

An array of integers representing the indices of the frames in the animation

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### FramesDuration
```csharp
public readonly ImmutableArray<T> FramesDuration;
```

An array of floats representing the duration of each frame in the animation, in milliseconds

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### NextAnimation
```csharp
public readonly T? NextAnimation;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
### ⭐ Methods
#### Evaluate(float, float, bool, float)
```csharp
public FrameInfo Evaluate(float time, float lastFrameTime, bool animationLoop, float forceAnimationDuration)
```

Evaluates the current frame of the animation, given a time value (in seconds)
            and an optional maximum animation duration (in seconds)

**Parameters** \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`lastFrameTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationLoop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`forceAnimationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[FrameInfo](../../../Murder/Core/FrameInfo.html) \

#### Evaluate(float, float, bool)
```csharp
public FrameInfo Evaluate(float time, float lastFrameTime, bool animationLoop)
```

Evaluates the current frame of the animation, given a time value (in seconds)
            and an optional maximum animation duration (in seconds)

**Parameters** \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`lastFrameTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationLoop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[FrameInfo](../../../Murder/Core/FrameInfo.html) \

#### EvaluatePreviousFrame(float, float, float)
```csharp
public int EvaluatePreviousFrame(float time, float animationDuration, float factor)
```

**Parameters** \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡