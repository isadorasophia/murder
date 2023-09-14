# SliderAttribute

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public class SliderAttribute : Attribute
```

A slider attribute used when setting values in the editor.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public SliderAttribute(float minimum, float maximum)
```

Creates a new [SliderAttribute](../../Murder/Attributes/SliderAttribute.html).

**Parameters** \
`minimum` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`maximum` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

### ⭐ Properties
#### Maximum
```csharp
public readonly float Maximum;
```

Maximum value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Minimum
```csharp
public readonly float Minimum;
```

Minimum value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡