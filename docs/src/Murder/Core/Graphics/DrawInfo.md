# DrawInfo

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct DrawInfo
```

Generic struct for drawing things without cluttering methods full of arguments.
            Note that not all fields are supported by all methods.
            Tip: Create a new one like this: <code>new DrawInfo(){ Color = Color.Red, Sort = 0.2f}</code>

### ⭐ Constructors
```csharp
public DrawInfo()
```

```csharp
public DrawInfo(Color color, float sort)
```

**Parameters** \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public DrawInfo(float sort)
```

**Parameters** \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### BlendMode
```csharp
public BlendStyle BlendMode { get; public set; }
```

**Returns** \
[BlendStyle](../../../Murder/Core/Graphics/BlendStyle.html) \
#### Clip
```csharp
public Rectangle Clip { get; public set; }
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
#### Color
```csharp
public Color Color { get; public set; }
```

**Returns** \
[Color](../../../Murder/Core/Graphics/Color.html) \
#### Debug
```csharp
public bool Debug { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Default
```csharp
public static DrawInfo Default { get; }
```

**Returns** \
[DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \
#### FlippedHorizontal
```csharp
public bool FlippedHorizontal { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Offset
```csharp
public Vector2 Offset { get; public set; }
```

An offset to draw this image. In pixels

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Origin
```csharp
public Vector2 Origin { get; public set; }
```

The origin of the image. From 0 to 1. Vector2Helper.Center is the center.

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Outline
```csharp
public T? Outline { get; public set; }
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### OutlineStyle
```csharp
public OutlineStyle OutlineStyle { get; public set; }
```

**Returns** \
[OutlineStyle](../../../Murder/Core/Graphics/OutlineStyle.html) \
#### Rotation
```csharp
public float Rotation { get; public set; }
```

In degrees.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Scale
```csharp
public Vector2 Scale { get; public set; }
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Shadow
```csharp
public T? Shadow { get; public set; }
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Sort
```csharp
public float Sort { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### WithSort(float)
```csharp
public DrawInfo WithSort(float sort)
```

**Parameters** \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \

#### GetBlendMode()
```csharp
public Vector3 GetBlendMode()
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \



⚡