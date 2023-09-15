# EditorLabelAttribute

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public class EditorLabelAttribute : Attribute
```

Label that will show up for this field in the editor.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public EditorLabelAttribute(string label1, string label2)
```

**Parameters** \
`label1` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`label2` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

```csharp
public EditorLabelAttribute(string label1)
```

**Parameters** \
`label1` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Label1
```csharp
public readonly string Label1;
```

The content of the tooltip.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Label2
```csharp
public readonly string Label2;
```

[Optional] Secondary label.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡