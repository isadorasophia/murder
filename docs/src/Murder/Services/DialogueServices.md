# DialogueServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class DialogueServices
```

### ⭐ Methods
#### HasDialogue(World, Entity, SituationComponent)
```csharp
public bool HasDialogue(World world, Entity e, SituationComponent situation)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`e` [Entity](../../Bang/Entities/Entity.html) \
`situation` [SituationComponent](../../Murder/Components/SituationComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasNewDialogue(World, Entity, SituationComponent)
```csharp
public bool HasNewDialogue(World world, Entity e, SituationComponent situation)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`e` [Entity](../../Bang/Entities/Entity.html) \
`situation` [SituationComponent](../../Murder/Components/SituationComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CreateCharacterFrom(Guid, int)
```csharp
public CharacterRuntime CreateCharacterFrom(Guid character, int situation)
```

**Parameters** \
`character` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`situation` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[CharacterRuntime](../../Murder/Core/Dialogs/CharacterRuntime.html) \

#### FetchAllLines(World, Entity, SituationComponent)
```csharp
public Line[] FetchAllLines(World world, Entity target, SituationComponent situation)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`target` [Entity](../../Bang/Entities/Entity.html) \
`situation` [SituationComponent](../../Murder/Components/SituationComponent.html) \

**Returns** \
[Line[]](../../Murder/Core/Dialogs/Line.html) \

#### CreateLine(Line)
```csharp
public LineComponent CreateLine(Line line)
```

**Parameters** \
`line` [Line](../../Murder/Core/Dialogs/Line.html) \

**Returns** \
[LineComponent](../../Murder/Components/LineComponent.html) \

#### FetchFirstLine(World, Entity, SituationComponent)
```csharp
public string FetchFirstLine(World world, Entity target, SituationComponent situation)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`target` [Entity](../../Bang/Entities/Entity.html) \
`situation` [SituationComponent](../../Murder/Components/SituationComponent.html) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡