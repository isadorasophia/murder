# Wait

**Namespace:** Bang.StateMachines \
**Assembly:** Bang.dll

```csharp
public class Wait : IEquatable<T>
```

A message fired to communicate the current state of the state machine.

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
protected Wait(Wait original)
```

**Parameters** \
`original` [Wait](../../Bang/StateMachines/Wait.html) \

### ⭐ Properties
#### Component
```csharp
public Type Component;
```

Used for [WaitKind.Message](../../Bang/StateMachines/WaitKind.html#message).

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
#### EqualityContract
```csharp
protected virtual Type EqualityContract { get; }
```

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
#### Kind
```csharp
public readonly WaitKind Kind;
```

When should the state machine be called again.

**Returns** \
[WaitKind](../../Bang/StateMachines/WaitKind.html) \
#### NextFrame
```csharp
public static Wait NextFrame { get; }
```

Wait until the next frame.

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \
#### Routine
```csharp
public IEnumerator<T> Routine;
```

Used for [WaitKind.Routine](../../Bang/StateMachines/WaitKind.html#routine).

**Returns** \
[IEnumerator\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerator-1?view=net-7.0) \
#### Stop
```csharp
public readonly static Wait Stop;
```

No longer execute the state machine.

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \
#### Target
```csharp
public Entity Target;
```

Used for [WaitKind.Message](../../Bang/StateMachines/WaitKind.html#message) when waiting on another entity that is not the owner of the state machine.

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
#### Value
```csharp
public T? Value;
```

Integer value, if kind is [WaitKind.Frames](../../Bang/StateMachines/WaitKind.html#frames).

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
### ⭐ Methods
#### PrintMembers(StringBuilder)
```csharp
protected virtual bool PrintMembers(StringBuilder builder)
```

**Parameters** \
`builder` [StringBuilder](https://learn.microsoft.com/en-us/dotnet/api/System.Text.StringBuilder?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Wait)
```csharp
public virtual bool Equals(Wait other)
```

**Parameters** \
`other` [Wait](../../Bang/StateMachines/Wait.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Object)
```csharp
public virtual bool Equals(Object obj)
```

**Parameters** \
`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode()
```csharp
public virtual int GetHashCode()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ForFrames(int)
```csharp
public Wait ForFrames(int frames)
```

Wait until <paramref name="frames" /> have occurred.

**Parameters** \
`frames` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### ForMessage()
```csharp
public Wait ForMessage()
```

Wait until message of type <typeparamref name="T" /> is fired.

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### ForMessage(Entity)
```csharp
public Wait ForMessage(Entity target)
```

Wait until message of type <typeparamref name="T" /> is fired from <paramref name="target" />.

**Parameters** \
`target` [Entity](../../Bang/Entities/Entity.html) \

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### ForMs(int)
```csharp
public Wait ForMs(int ms)
```

Wait for <paramref name="ms" />.

**Parameters** \
`ms` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### ForRoutine(IEnumerator<T>)
```csharp
public Wait ForRoutine(IEnumerator<T> routine)
```

Wait until <paramref name="routine" /> finishes.

**Parameters** \
`routine` [IEnumerator\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerator-1?view=net-7.0) \

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \

#### ForSeconds(float)
```csharp
public Wait ForSeconds(float seconds)
```

Wait for <paramref name="seconds" />.

**Parameters** \
`seconds` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Wait](../../Bang/StateMachines/Wait.html) \



⚡