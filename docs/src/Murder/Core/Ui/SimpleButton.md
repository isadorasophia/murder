# SimpleButton

**Namespace:** Murder.Core.Ui \
**Assembly:** Murder.dll

```csharp
public class SimpleButton
```

### ⭐ Constructors
```csharp
public SimpleButton(Guid images, Rectangle rectangle)
```

**Parameters** \
`images` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`rectangle` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

```csharp
public SimpleButton(Guid images, ButtonState state, Rectangle rectangle)
```

**Parameters** \
`images` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`state` [ButtonState](../../../Murder/Core/Ui/ButtonState.html) \
`rectangle` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

### ⭐ Properties
#### Images
```csharp
public Guid Images;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Rectangle
```csharp
public Rectangle Rectangle;
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
#### State
```csharp
public ButtonState State;
```

**Returns** \
[ButtonState](../../../Murder/Core/Ui/ButtonState.html) \
### ⭐ Methods
#### Draw(Batch2D, DrawInfo)
```csharp
public void Draw(Batch2D batch, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \

#### Update(Point, bool, bool, Action)
```csharp
public void Update(Point cursorPosition, bool cursorClicked, bool cursorDown, Action action)
```

**Parameters** \
`cursorPosition` [Point](../../../Murder/Core/Geometry/Point.html) \
`cursorClicked` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`cursorDown` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`action` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### UpdatePosition(Rectangle)
```csharp
public void UpdatePosition(Rectangle rectangle)
```

**Parameters** \
`rectangle` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \



⚡