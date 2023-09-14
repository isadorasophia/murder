# MenuInfo

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public sealed struct MenuInfo
```

### ⭐ Constructors
```csharp
public MenuInfo()
```

```csharp
public MenuInfo(MenuOption[] options)
```

**Parameters** \
`options` [MenuOption[]](../..//Murder/Services/MenuOption.html) \

```csharp
public MenuInfo(IEnumerable<T> options)
```

**Parameters** \
`options` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

```csharp
public MenuInfo(int size)
```

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public MenuInfo(String[] options)
```

**Parameters** \
`options` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Canceled
```csharp
public bool Canceled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Disabled
```csharp
public bool Disabled;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### HasOptions
```csharp
public bool HasOptions { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Icons
```csharp
public Portrait[] Icons;
```

Optional icons to be displayed near the options.

**Returns** \
[Portrait[]](../..//Murder/Core/Portrait.html) \
#### JustMoved
```csharp
public bool JustMoved;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### LargestOptionText
```csharp
public float LargestOptionText { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LastMoved
```csharp
public float LastMoved;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LastPressed
```csharp
public float LastPressed;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Length
```csharp
public int Length { get; }
```

Number of options in this menu

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Options
```csharp
public MenuOption[] Options;
```

**Returns** \
[MenuOption[]](../..//Murder/Services/MenuOption.html) \
#### Overflow
```csharp
public int Overflow;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### PreviousSelection
```csharp
public int PreviousSelection;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Scroll
```csharp
public int Scroll;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Selection
```csharp
public int Selection { get; private set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Sounds
```csharp
public MenuSounds Sounds;
```

**Returns** \
[MenuSounds](../..//Murder/Core/Sounds/MenuSounds.html) \
#### VisibleItems
```csharp
public int VisibleItems;
```

Number of visible options on the screen, 8 is the default.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### IsOptionAvailable(int)
```csharp
public bool IsOptionAvailable(int option)
```

**Parameters** \
`option` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### NextAvailableOption(int, int)
```csharp
public int NextAvailableOption(int option, int direction)
```

**Parameters** \
`option` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`direction` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

#### Disable(bool)
```csharp
public MenuInfo Disable(bool disabled)
```

**Parameters** \
`disabled` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[MenuInfo](../..//Murder/Core/Input/MenuInfo.html) \

#### GetOptionText(int)
```csharp
public string GetOptionText(int index)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetSelectedOptionText()
```csharp
public string GetSelectedOptionText()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Cancel()
```csharp
public void Cancel()
```

#### Clamp(int)
```csharp
public void Clamp(int max)
```

**Parameters** \
`max` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Press()
```csharp
public void Press()
```

#### Reset()
```csharp
public void Reset()
```

Resets the menu info selector to the first available option.

#### Resize(int)
```csharp
public void Resize(int size)
```

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Select(int, float)
```csharp
public void Select(int index, float now)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`now` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Select(int)
```csharp
public void Select(int index)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SnapLeft(int)
```csharp
public void SnapLeft(int width)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SnapRight(int)
```csharp
public void SnapRight(int width)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡