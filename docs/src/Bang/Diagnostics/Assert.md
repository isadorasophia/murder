# Assert

**Namespace:** Bang.Diagnostics \
**Assembly:** Bang.dll

```csharp
public static class Assert
```

Helper class for asserting and throwing exceptions on unexpected scenarios.

### ⭐ Methods
#### Verify(bool, string)
```csharp
public void Verify(bool condition, string text)
```

Verify whether a condition is valid. If not, throw a [InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/System.InvalidOperationException?view=net-7.0).
            This throws regardless if it's on release on debug binaries.

**Parameters** \
`condition` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡