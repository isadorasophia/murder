# MessagerAttribute

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public class MessagerAttribute : Attribute
```

Marks a messager attribute for a system.
            This must be implemented by all the systems that inherit [IMessagerSystem](../../Bang/Systems/IMessagerSystem.html).

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public MessagerAttribute(Type[] types)
```

Creates a new [MessagerAttribute](../../Bang/Systems/MessagerAttribute.html).

**Parameters** \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

### ⭐ Properties
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \
#### Types
```csharp
public Type[] Types { get; }
```

System will target all the entities that has all this set of components.

**Returns** \
[Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \


⚡