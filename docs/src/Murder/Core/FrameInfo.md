# FrameInfo

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public sealed struct FrameInfo
```

A struct representing information about a single animation frame, such as its index in the list and a flag indicating whether the animation is complete

### ⭐ Constructors
```csharp
public FrameInfo()
```

```csharp
public FrameInfo(Animation animation)
```

**Parameters** \
`animation` [Animation](../..//Murder/Core/Graphics/Animation.html) \

```csharp
public FrameInfo(int frame, bool animationComplete, Animation animation)
```

**Parameters** \
`frame` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`animationComplete` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`animation` [Animation](../..//Murder/Core/Graphics/Animation.html) \

```csharp
public FrameInfo(int frame, bool animationComplete, ImmutableArray<T> event, Animation animation)
```

**Parameters** \
`frame` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`animationComplete` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`event` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`animation` [Animation](../..//Murder/Core/Graphics/Animation.html) \

```csharp
public FrameInfo(int frame, bool animationComplete, ReadOnlySpan<T> event, Animation animation)
```

**Parameters** \
`frame` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`animationComplete` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`event` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \
`animation` [Animation](../..//Murder/Core/Graphics/Animation.html) \

### ⭐ Properties
#### Animation
```csharp
public readonly Animation Animation;
```

**Returns** \
[Animation](../..//Murder/Core/Graphics/Animation.html) \
#### Complete
```csharp
public readonly bool Complete;
```

Whether the animation is complete

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Event
```csharp
public readonly ImmutableArray<T> Event;
```

A string ID representing the events played since the last played frame (if any). Usually set in Aseprite.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Failed
```csharp
public bool Failed { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Frame
```csharp
public readonly int Frame;
```

The index of the current frame

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \


⚡