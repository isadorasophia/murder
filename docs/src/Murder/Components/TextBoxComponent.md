# TextBoxComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct TextBoxComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public TextBoxComponent(string text, int visibleCaracters, float fontSize, float sorting, Color color, Vector2 offset)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`visibleCaracters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`fontSize` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \

### ⭐ Properties
#### Color
```csharp
public readonly Color Color;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
#### FontSize
```csharp
public readonly float FontSize;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Offset
```csharp
public readonly Vector2 Offset;
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### Sorting
```csharp
public readonly float Sorting;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Text
```csharp
public readonly string Text;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### VisibleCharacters
```csharp
public readonly int VisibleCharacters;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### WithText(string)
```csharp
public TextBoxComponent WithText(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[TextBoxComponent](/Murder/Components/TextBoxComponent.html) \

#### WithVisibleCharacters(int)
```csharp
public TextBoxComponent WithVisibleCharacters(int visibleCaracters)
```

**Parameters** \
`visibleCaracters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[TextBoxComponent](/Murder/Components/TextBoxComponent.html) \



⚡