# SoundParameterAttribute

**Namespace:** Murder.Utilities.Attributes \
**Assembly:** Murder.dll

```csharp
public class SoundParameterAttribute : Attribute
```

Attribute used for IComponent structs that will change according to 
            a "story". This is used for debugging and filtering in editor.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public SoundParameterAttribute(SoundParameterKind kind)
```

**Parameters** \
`kind` [SoundParameterKind](../../../Murder/Utilities/Attributes/SoundParameterKind.html) \

### ⭐ Properties
#### Kind
```csharp
public readonly SoundParameterKind Kind;
```

**Returns** \
[SoundParameterKind](../../../Murder/Utilities/Attributes/SoundParameterKind.html) \
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡