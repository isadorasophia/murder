# IParentRelativeComponent

**Namespace:** Bang.Components \
**Assembly:** Bang.dll

```csharp
public abstract IParentRelativeComponent : IComponent
```

A component that is relative to the parent.
            It will be notified each time the tracking component of the parent changes.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Properties
#### HasParent
```csharp
public abstract virtual bool HasParent { get; }
```

Whether the component has a parent that it's tracking.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### WithoutParent()
```csharp
public abstract IParentRelativeComponent WithoutParent()
```

Creates a copy of the component without any parent.

**Returns** \
[IParentRelativeComponent](../../Bang/Components/IParentRelativeComponent.html) \

#### OnParentModified(IComponent, Entity)
```csharp
public abstract void OnParentModified(IComponent parentComponent, Entity childEntity)
```

Called when a parent modifies <paramref name="parentComponent" />.

**Parameters** \
`parentComponent` [IComponent](../../Bang/Components/IComponent.html) \
\
`childEntity` [Entity](../../Bang/Entities/Entity.html) \
\



⚡