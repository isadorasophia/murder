# MatchKind

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed enum MatchKind : Enum, IComparable, IFormattable, IConvertible
```

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### Choice
```csharp
public static const MatchKind Choice;
```

Choice dialogs (&gt;) that the player can pick.

**Returns** \
[MatchKind](../../../Murder/Core/Dialogs/MatchKind.html) \
#### HighestScore
```csharp
public static const MatchKind HighestScore;
```

This will pick the dialog with the highest score.
            This is when dialogs are listed with -/+.

**Returns** \
[MatchKind](../../../Murder/Core/Dialogs/MatchKind.html) \
#### IfElse
```csharp
public static const MatchKind IfElse;
```

All the blocks that are next are subjected to an "else" relationship.

**Returns** \
[MatchKind](../../../Murder/Core/Dialogs/MatchKind.html) \
#### Next
```csharp
public static const MatchKind Next;
```

This will pick in consecutive order, whatever matches first.

**Returns** \
[MatchKind](../../../Murder/Core/Dialogs/MatchKind.html) \
#### Random
```csharp
public static const MatchKind Random;
```

This will pick random dialogs.

**Returns** \
[MatchKind](../../../Murder/Core/Dialogs/MatchKind.html) \


⚡