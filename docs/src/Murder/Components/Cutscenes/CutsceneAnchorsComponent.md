# CutsceneAnchorsComponent

**Namespace:** Murder.Components.Cutscenes \
**Assembly:** Murder.dll

```csharp
public sealed struct CutsceneAnchorsComponent : IComponent
```

This is a list of anchor points of cutscene.

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public CutsceneAnchorsComponent()
```

```csharp
public CutsceneAnchorsComponent(ImmutableDictionary<TKey, TValue> anchors)
```

**Parameters** \
`anchors` [ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

### ⭐ Properties
#### Anchors
```csharp
public readonly ImmutableDictionary<TKey, TValue> Anchors;
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
### ⭐ Methods
#### AddAnchorAt(Vector2)
```csharp
public CutsceneAnchorsComponent AddAnchorAt(Vector2 newPosition)
```

**Parameters** \
`newPosition` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[CutsceneAnchorsComponent](/Murder/Components/Cutscenes/CutsceneAnchorsComponent.html) \

#### WithAnchorAt(string, Vector2)
```csharp
public CutsceneAnchorsComponent WithAnchorAt(string name, Vector2 newPosition)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`newPosition` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[CutsceneAnchorsComponent](/Murder/Components/Cutscenes/CutsceneAnchorsComponent.html) \

#### WithoutAnchorAt(string)
```csharp
public CutsceneAnchorsComponent WithoutAnchorAt(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[CutsceneAnchorsComponent](/Murder/Components/Cutscenes/CutsceneAnchorsComponent.html) \



⚡