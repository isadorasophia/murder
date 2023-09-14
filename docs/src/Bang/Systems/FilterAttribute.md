# FilterAttribute

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public class FilterAttribute : Attribute
```

Indicates characteristics of a system that was implemented on our ECS system.
            This must be implemented by all the systems that inherits from [ISystem](../../Bang/Systems/ISystem.html).

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public FilterAttribute(ContextAccessorFilter filter, ContextAccessorKind kind, Type[] types)
```

Creates a system filter with custom accessors.

**Parameters** \
`filter` [ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
`kind` [ContextAccessorKind](../../Bang/Contexts/ContextAccessorKind.html) \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

```csharp
public FilterAttribute(ContextAccessorFilter filter, Type[] types)
```

Create a system filter with default accessor of [FilterAttribute.Kind" /> for <paramref name="types](../../Bang/Systems/FilterAttribute.html#kind" /> for <paramref name="types).

**Parameters** \
`filter` [ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

```csharp
public FilterAttribute(ContextAccessorKind kind, Type[] types)
```

Create a system filter with default accessor of [FilterAttribute.Filter" /> for <paramref name="types](../../Bang/Systems/FilterAttribute.html#filter" /> for <paramref name="types).

**Parameters** \
`kind` [ContextAccessorKind](../../Bang/Contexts/ContextAccessorKind.html) \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

```csharp
public FilterAttribute(Type[] types)
```

Create a system filter with default accessors for <paramref name="types" />.

**Parameters** \
`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

### ⭐ Properties
#### Filter
```csharp
public ContextAccessorFilter Filter { get; public set; }
```

This is how the system will filter the entities. See [ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html).

**Returns** \
[ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
#### Kind
```csharp
public ContextAccessorKind Kind { get; public set; }
```

This is the kind of accessor that will be made on this component.
            This can be leveraged once we parallelize update frames (which we don't yet), so don't bother with this just yet.

**Returns** \
[ContextAccessorKind](../../Bang/Contexts/ContextAccessorKind.html) \
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

System will target all the entities that has all this set of components.

**Returns** \
[Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \


⚡