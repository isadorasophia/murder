# AgentSpeedMultiplier

**Namespace:** Murder.Components.Agents \
**Assembly:** Murder.dll

```csharp
public sealed struct AgentSpeedMultiplier : IComponent
```

**Implements:** _[IComponent](../../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AgentSpeedMultiplier()
```

```csharp
public AgentSpeedMultiplier(float speedMultiplier)
```

**Parameters** \
`speedMultiplier` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### SpeedMultiplier
```csharp
public readonly float SpeedMultiplier;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### WithMultiplier(float)
```csharp
public AgentSpeedMultiplier WithMultiplier(float multiplier)
```

Increases the current multiplier by <paramref name="multiplier" />.

**Parameters** \
`multiplier` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[AgentSpeedMultiplier](../../../Murder/Components/Agents/AgentSpeedMultiplier.html) \



⚡