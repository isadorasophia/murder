# CutsceneAnchorsEditorComponent

**Namespace:** Murder.Components.Serialization \
**Assembly:** Murder.dll

```csharp
public sealed struct CutsceneAnchorsEditorComponent : IComponent
```

**Implements:** _[IComponent](../../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CutsceneAnchorsEditorComponent()
```

```csharp
public CutsceneAnchorsEditorComponent(ImmutableArray<T> anchors)
```

**Parameters** \
`anchors` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

### ⭐ Properties
#### Anchors
```csharp
public readonly ImmutableArray<T> Anchors;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### FindAnchor(string)
```csharp
public Anchor FindAnchor(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Anchor](../../../Murder/Core/Cutscenes/Anchor.html) \

#### AddAnchorAt(Vector2)
```csharp
public CutsceneAnchorsEditorComponent AddAnchorAt(Vector2 newPosition)
```

**Parameters** \
`newPosition` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[CutsceneAnchorsEditorComponent](../../../Murder/Components/Serialization/CutsceneAnchorsEditorComponent.html) \

#### WithAnchorAt(string, Vector2)
```csharp
public CutsceneAnchorsEditorComponent WithAnchorAt(string name, Vector2 newPosition)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`newPosition` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[CutsceneAnchorsEditorComponent](../../../Murder/Components/Serialization/CutsceneAnchorsEditorComponent.html) \

#### WithoutAnchorAt(string)
```csharp
public CutsceneAnchorsEditorComponent WithoutAnchorAt(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[CutsceneAnchorsEditorComponent](../../../Murder/Components/Serialization/CutsceneAnchorsEditorComponent.html) \



⚡