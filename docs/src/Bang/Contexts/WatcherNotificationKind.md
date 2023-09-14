# WatcherNotificationKind

**Namespace:** Bang.Contexts \
**Assembly:** Bang.dll

```csharp
public sealed enum WatcherNotificationKind : Enum, IComparable, IFormattable, IConvertible
```

When a system is watching for a component, this is the kind of notification currently fired.

**Implements:** _[Enum](https://learn.microsoft.com/en-us/dotnet/api/System.Enum?view=net-7.0), [IComparable](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable?view=net-7.0), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/System.IFormattable?view=net-7.0), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/System.IConvertible?view=net-7.0)_

### ⭐ Properties
#### Added
```csharp
public static const WatcherNotificationKind Added;
```

Component has been added. It is not called if the entity is dead.

**Returns** \
[WatcherNotificationKind](../../Bang/Contexts/WatcherNotificationKind.html) \
#### Disabled
```csharp
public static const WatcherNotificationKind Disabled;
```

Entity has been disabled, hence all its components.

**Returns** \
[WatcherNotificationKind](../../Bang/Contexts/WatcherNotificationKind.html) \
#### Enabled
```csharp
public static const WatcherNotificationKind Enabled;
```

Entity has been enabled, hence all its components. Called if an entity was
            previously disabled.

**Returns** \
[WatcherNotificationKind](../../Bang/Contexts/WatcherNotificationKind.html) \
#### Modified
```csharp
public static const WatcherNotificationKind Modified;
```

Component was modified. It is not called if the entity is dead.

**Returns** \
[WatcherNotificationKind](../../Bang/Contexts/WatcherNotificationKind.html) \
#### Removed
```csharp
public static const WatcherNotificationKind Removed;
```

Component was removed.

**Returns** \
[WatcherNotificationKind](../../Bang/Contexts/WatcherNotificationKind.html) \


⚡