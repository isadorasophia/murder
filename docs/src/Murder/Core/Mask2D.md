# Mask2D

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class Mask2D : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public Mask2D(Vector2 size, T? color)
```

**Parameters** \
`size` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

```csharp
public Mask2D(int width, int height, T? color)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

### ⭐ Properties
#### IsDisposed
```csharp
public bool IsDisposed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### RenderTarget
```csharp
public RenderTarget2D RenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### Size
```csharp
public readonly Vector2 Size;
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
### ⭐ Methods
#### Begin(bool)
```csharp
public Batch2D Begin(bool debug)
```

**Parameters** \
`debug` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \

#### Dispose()
```csharp
public virtual void Dispose()
```

#### End(Batch2D, Vector2, Vector2, DrawInfo)
```csharp
public void End(Batch2D targetBatch, Vector2 position, Vector2 camera, DrawInfo drawInfo)
```

**Parameters** \
`targetBatch` [Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`camera` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`drawInfo` [DrawInfo](../..//Murder/Core/Graphics/DrawInfo.html) \

#### End(Batch2D, Vector2, DrawInfo)
```csharp
public void End(Batch2D targetBatch, Vector2 position, DrawInfo drawInfo)
```

**Parameters** \
`targetBatch` [Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`drawInfo` [DrawInfo](../..//Murder/Core/Graphics/DrawInfo.html) \



⚡