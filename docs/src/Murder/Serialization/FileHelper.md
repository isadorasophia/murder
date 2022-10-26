# FileHelper

**Namespace:** Murder.Serialization \
**Assembly:** Murder.dll

```csharp
public static class FileHelper
```

FileHelper which will do OS operations. This is system-agnostic.

### ⭐ Methods
#### DeleteDirectoryIfExists(String&)
```csharp
public bool DeleteDirectoryIfExists(String& path)
```

**Parameters** \
`path` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeleteFileIfExists(String&)
```csharp
public bool DeleteFileIfExists(String& path)
```

**Parameters** \
`path` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Exists(String&)
```csharp
public bool Exists(String& path)
```

**Parameters** \
`path` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ExistsFromRelativePath(String&)
```csharp
public bool ExistsFromRelativePath(String& relativePath)
```

**Parameters** \
`relativePath` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FileExists(String&)
```csharp
public bool FileExists(String& path)
```

**Parameters** \
`path` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetOrCreateDirectory(String&)
```csharp
public DirectoryInfo GetOrCreateDirectory(String& path)
```

**Parameters** \
`path` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[DirectoryInfo](https://learn.microsoft.com/en-us/dotnet/api/System.IO.DirectoryInfo?view=net-7.0) \

#### GetAllFilesInFolder(string, bool, String[])
```csharp
public IEnumerable<T> GetAllFilesInFolder(string path, bool recursive, String[] filters)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`recursive` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`filters` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetAllFilesInFolder(string, string, bool)
```csharp
public IEnumerable<T> GetAllFilesInFolder(string path, string filter, bool recursive)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`filter` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`recursive` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### ListAllDirectories(string)
```csharp
public IEnumerable<T> ListAllDirectories(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### DirectoryCopy(string, string, bool)
```csharp
public int DirectoryCopy(string sourceDirPath, string destDirPath, bool copySubDirs)
```

**Parameters** \
`sourceDirPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`destDirPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`copySubDirs` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### EscapePath(string)
```csharp
public string EscapePath(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetPath(String[])
```csharp
public string GetPath(String[] paths)
```

Gets the rooted path from a relative one

**Parameters** \
`paths` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

#### GetPathWithoutExtension(string)
```csharp
public string GetPathWithoutExtension(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SaveSerialized(T, string, bool)
```csharp
public string SaveSerialized(T value, string path, bool isCompressed)
```

**Parameters** \
`value` [T]() \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`isCompressed` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SaveSerializedFromRelativePath(T, String&)
```csharp
public string SaveSerializedFromRelativePath(T value, String& relativePath)
```

**Parameters** \
`value` [T]() \
`relativePath` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### DeserializeAsset(string)
```csharp
public T DeserializeAsset(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T]() \

#### DeserializeGeneric(string)
```csharp
public T DeserializeGeneric(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T]() \

#### TryGetLastWrite(string)
```csharp
public T? TryGetLastWrite(string path)
```

This will iterate recursively over all files in <paramref name="path" /> and return
            the latest modified date.

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### DeleteContent(String&, bool)
```csharp
public void DeleteContent(String& fullpath, bool deleteRootFiles)
```

**Parameters** \
`fullpath` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`deleteRootFiles` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### OpenFolder(string)
```csharp
public void OpenFolder(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SaveText(String&, String&)
```csharp
public void SaveText(String& fullpath, String& content)
```

**Parameters** \
`fullpath` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`content` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SaveTextFromRelativePath(String&, String&)
```csharp
public void SaveTextFromRelativePath(String& relativePath, String& content)
```

**Parameters** \
`relativePath` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`content` [string&](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡