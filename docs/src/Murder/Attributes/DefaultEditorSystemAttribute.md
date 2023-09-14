# DefaultEditorSystemAttribute

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public class DefaultEditorSystemAttribute : Attribute
```

Attributes for fields that should always show up in the editor.
            Commonly used for private fields.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public DefaultEditorSystemAttribute()
```

```csharp
public DefaultEditorSystemAttribute(bool startActive)
```

**Parameters** \
`startActive` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### StartActive
```csharp
public readonly bool StartActive;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡