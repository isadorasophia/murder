# BatchMode

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed enum BatchMode : Enum, IComparable, IFormattable, IConvertible
```

How SpriteBatch rendering should behave.

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### DepthSortAscending
```csharp
public static const BatchMode DepthSortAscending;
```

Sort batches by Layer Depth using ascending order, but still trying to group as many batches as possible to reduce draw calls.

**Returns** \
[BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
#### DepthSortDescending
```csharp
public static const BatchMode DepthSortDescending;
```

Sort batches by Layer Depth using descending order, but still trying to group as many batches as possible to reduce draw calls.

**Returns** \
[BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
#### DrawOrder
```csharp
public static const BatchMode DrawOrder;
```

Standard way. Will respect SpriteBatch.Draw*() call order, ignoring Layer Depth, but still trying to group as many batches as possible to reduce draw calls.

**Returns** \
[BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
#### Immediate
```csharp
public static const BatchMode Immediate;
```

Every SpriteBatch.Draw*() will result in an isolate draw call. No batching will be made, so be careful.

**Returns** \
[BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \


⚡