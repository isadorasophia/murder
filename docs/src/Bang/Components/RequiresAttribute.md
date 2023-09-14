# RequiresAttribute

**Namespace:** Bang.Components \
**Assembly:** Bang.dll

```csharp
public class RequiresAttribute : Attribute
```

Marks a component as requiring other components when being added to an entity.
            This is an attribute that tells that a given data requires another one of the same type.
            For example: a component requires another component when adding it to the entity,
            or a system requires another system when adding it to a world.
            If this is for a system, it assumes that the system that depends on the other one comes first.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public RequiresAttribute(Type[] types)
```

Creates a new [RequiresAttribute](../../Bang/Components/RequiresAttribute.html).

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
public Type[] Types { get; public set; }
```

System will target all entities that have this set of components.

**Returns** \
[Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \


⚡