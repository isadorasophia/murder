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
public Animation(Int32[] frames, Single[] framesDuration)
```

**Parameters** \
`frames` [int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`framesDuration` [float[]](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### AnimationDuration
```csharp
public readonly float AnimationDuration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### FrameCount
```csharp
public int FrameCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Frames
```csharp
public readonly ImmutableArray<T> Frames;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### FramesDuration
```csharp
public readonly ImmutableArray<T> FramesDuration;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### Evaluate(float, float, float)
```csharp
public ValueTuple<T1, T2> Evaluate(float startTime, float currentTime, float forceAnimationDuration)
```

**Parameters** \
`startTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`currentTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`forceAnimationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### Evaluate(float, float)
```csharp
public ValueTuple<T1, T2> Evaluate(float startTime, float currentTime)
```

**Parameters** \
`startTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`currentTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \
The name of the current frame\



⚡