# WaitKind

**Namespace:** Bang.StateMachines \
**Assembly:** Bang.dll

```csharp
public sealed enum WaitKind : Enum, IComparable, IFormattable, IConvertible
```

Wait between state machine calls.

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### Frames
```csharp
public static const WaitKind Frames;
```

Wait for 'x' frames.

**Returns** \
[WaitKind](../..//Bang/StateMachines/WaitKind.html) \
#### Message
```csharp
public static const WaitKind Message;
```

Wait for a message to be fired.

**Returns** \
[WaitKind](../..//Bang/StateMachines/WaitKind.html) \
#### Ms
```csharp
public static const WaitKind Ms;
```

Wait for 'x' ms.

**Returns** \
[WaitKind](../..//Bang/StateMachines/WaitKind.html) \
#### Routine
```csharp
public static const WaitKind Routine;
```

Redirect execution to another routine. This will resume once that's finished.

**Returns** \
[WaitKind](../..//Bang/StateMachines/WaitKind.html) \
#### Stop
```csharp
public static const WaitKind Stop;
```

Stops the state machine execution.

**Returns** \
[WaitKind](../..//Bang/StateMachines/WaitKind.html) \


⚡