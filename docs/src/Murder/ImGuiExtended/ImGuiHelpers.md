# ImGuiHelpers

**Namespace:** Murder.ImGuiExtended \
**Assembly:** Murder.dll

```csharp
public static class ImGuiHelpers
```

### ⭐ Methods
#### BeginPopupModal(string, ImGuiWindowFlags)
```csharp
public bool BeginPopupModal(string name, ImGuiWindowFlags flags)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`flags` [ImGuiWindowFlags]() \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### BeginPopupModalCentered(string)
```csharp
public bool BeginPopupModalCentered(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ColoredIconButton(char, string, bool)
```csharp
public bool ColoredIconButton(char icon, string id, bool isActive)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`isActive` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeleteButton(string)
```csharp
public bool DeleteButton(string id)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DrawEnumField(string, T&)
```csharp
public bool DrawEnumField(string id, T& fieldValue)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`fieldValue` [T&]() \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FadedSelectableWithIcon(string, char, bool)
```csharp
public bool FadedSelectableWithIcon(string text, char icon, bool selected)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`selected` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IconButton(char, string, T?, bool)
```csharp
public bool IconButton(char icon, string id, T? color, bool sameLine)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`sameLine` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LowButton(string)
```csharp
public bool LowButton(string label)
```

**Parameters** \
`label` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SelectableWithIcon(string, char, bool)
```csharp
public bool SelectableWithIcon(string text, char icon, bool selected)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`selected` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SelectableWithIconColor(string, char, Vector4, Vector4, bool)
```csharp
public bool SelectableWithIconColor(string text, char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`selectedColor` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
`unselectedColor` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
`selected` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SelectedButton(string, T?)
```csharp
public bool SelectedButton(string label, T? color)
```

**Parameters** \
`label` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SelectedIconButton(char, T?)
```csharp
public bool SelectedIconButton(char icon, T? color)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ShowIcon(char, Vector4, Vector4, bool)
```csharp
public bool ShowIcon(char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`selectedColor` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
`unselectedColor` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
`selected` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TreeNodeWithIcon(char, string, ImGuiTreeNodeFlags)
```csharp
public bool TreeNodeWithIcon(char icon, string label, ImGuiTreeNodeFlags flags)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`label` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`flags` [ImGuiTreeNodeFlags]() \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### MakeColor32(byte, byte, byte, byte)
```csharp
public uint MakeColor32(byte r, byte g, byte b, byte a)
```

**Parameters** \
`r` [byte](https://learn.microsoft.com/en-us/dotnet/api/System.Byte?view=net-7.0) \
`g` [byte](https://learn.microsoft.com/en-us/dotnet/api/System.Byte?view=net-7.0) \
`b` [byte](https://learn.microsoft.com/en-us/dotnet/api/System.Byte?view=net-7.0) \
`a` [byte](https://learn.microsoft.com/en-us/dotnet/api/System.Byte?view=net-7.0) \

**Returns** \
[uint](https://learn.microsoft.com/en-us/dotnet/api/System.UInt32?view=net-7.0) \

#### MakeColor32(Vector4)
```csharp
public uint MakeColor32(Vector4 vector)
```

**Parameters** \
`vector` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \

**Returns** \
[uint](https://learn.microsoft.com/en-us/dotnet/api/System.UInt32?view=net-7.0) \

#### DrawEnumField(string, Type, int)
```csharp
public ValueTuple<T1, T2> DrawEnumField(string id, Type enumType, int fieldValue)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`enumType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`fieldValue` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### ColorIcon(char, Vector4)
```csharp
public void ColorIcon(char icon, Vector4 color)
```

**Parameters** \
`icon` [char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
`color` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \

#### DisabledButton(Action)
```csharp
public void DisabledButton(Action button)
```

**Parameters** \
`button` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### DisabledButton(string)
```csharp
public void DisabledButton(string text)
```

**Parameters** \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### HelpTooltip(string)
```csharp
public void HelpTooltip(string description)
```

**Parameters** \
`description` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Image(string, float, TextureAtlas, float)
```csharp
public void Image(string id, float maxSize, TextureAtlas atlas, float scale)
```

**Parameters** \
`id` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`maxSize` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`atlas` [TextureAtlas](/Murder/Core/Graphics/TextureAtlas.html) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡