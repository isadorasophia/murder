# WritablePropertiesOnlyResolver

**Namespace:** Murder.Serialization \
**Assembly:** Murder.dll

```csharp
public class WritablePropertiesOnlyResolver : DefaultContractResolver, IContractResolver
```

Custom contract resolver for serializing our game assets.
            This currently filters out getters and filters in readonly fields.

**Implements:** _[DefaultContractResolver](../../), [IContractResolver](../../)_

### ⭐ Constructors
```csharp
public WritablePropertiesOnlyResolver()
```

### ⭐ Properties
#### DefaultMembersSearchFlags
```csharp
public BindingFlags DefaultMembersSearchFlags { get; public set; }
```

**Returns** \
[BindingFlags](https://learn.microsoft.com/en-us/dotnet/api/System.Reflection.BindingFlags?view=net-7.0) \
#### DynamicCodeGeneration
```csharp
public bool DynamicCodeGeneration { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IgnoreIsSpecifiedMembers
```csharp
public bool IgnoreIsSpecifiedMembers { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IgnoreSerializableAttribute
```csharp
public bool IgnoreSerializableAttribute { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IgnoreSerializableInterface
```csharp
public bool IgnoreSerializableInterface { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IgnoreShouldSerializeMembers
```csharp
public bool IgnoreShouldSerializeMembers { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### NamingStrategy
```csharp
public NamingStrategy NamingStrategy { get; public set; }
```

**Returns** \
[NamingStrategy](../../) \
#### SerializeCompilerGeneratedMembers
```csharp
public bool SerializeCompilerGeneratedMembers { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### CreateConstructorParameters(ConstructorInfo, JsonPropertyCollection)
```csharp
protected virtual IList<T> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
```

**Parameters** \
`constructor` [ConstructorInfo](https://learn.microsoft.com/en-us/dotnet/api/System.Reflection.ConstructorInfo?view=net-7.0) \
`memberProperties` [JsonPropertyCollection](../../) \

**Returns** \
[IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \

#### CreateProperties(Type, MemberSerialization)
```csharp
protected virtual IList<T> CreateProperties(Type type, MemberSerialization memberSerialization)
```

Only create properties that are able to be set.
            See: https://stackoverflow.com/a/18548894.

**Parameters** \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`memberSerialization` [MemberSerialization](../../) \

**Returns** \
[IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \

#### CreateMemberValueProvider(MemberInfo)
```csharp
protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
```

**Parameters** \
`member` [MemberInfo](https://learn.microsoft.com/en-us/dotnet/api/System.Reflection.MemberInfo?view=net-7.0) \

**Returns** \
[IValueProvider](../../) \

#### CreateArrayContract(Type)
```csharp
protected virtual JsonArrayContract CreateArrayContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonArrayContract](../../) \

#### CreateContract(Type)
```csharp
protected virtual JsonContract CreateContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonContract](../../) \

#### ResolveContractConverter(Type)
```csharp
protected virtual JsonConverter ResolveContractConverter(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonConverter](../../) \

#### CreateDictionaryContract(Type)
```csharp
protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonDictionaryContract](../../) \

#### CreateDynamicContract(Type)
```csharp
protected virtual JsonDynamicContract CreateDynamicContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonDynamicContract](../../) \

#### CreateISerializableContract(Type)
```csharp
protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonISerializableContract](../../) \

#### CreateLinqContract(Type)
```csharp
protected virtual JsonLinqContract CreateLinqContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonLinqContract](../../) \

#### CreateObjectContract(Type)
```csharp
protected virtual JsonObjectContract CreateObjectContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonObjectContract](../../) \

#### CreatePrimitiveContract(Type)
```csharp
protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonPrimitiveContract](../../) \

#### CreateProperty(MemberInfo, MemberSerialization)
```csharp
protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
```

While we ignore getter properties, we do not want to ignore readonly fields.

**Parameters** \
`member` [MemberInfo](https://learn.microsoft.com/en-us/dotnet/api/System.Reflection.MemberInfo?view=net-7.0) \
`memberSerialization` [MemberSerialization](../../) \

**Returns** \
[JsonProperty](../../) \

#### CreatePropertyFromConstructorParameter(JsonProperty, ParameterInfo)
```csharp
protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
```

**Parameters** \
`matchingMemberProperty` [JsonProperty](../../) \
`parameterInfo` [ParameterInfo](https://learn.microsoft.com/en-us/dotnet/api/System.Reflection.ParameterInfo?view=net-7.0) \

**Returns** \
[JsonProperty](../../) \

#### CreateStringContract(Type)
```csharp
protected virtual JsonStringContract CreateStringContract(Type objectType)
```

**Parameters** \
`objectType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonStringContract](../../) \

#### GetSerializableMembers(Type)
```csharp
protected virtual List<T> GetSerializableMembers(Type t)
```

Make sure we fetch all the fields that define [ShowInEditor] in addition to JsonProperty.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### ResolveDictionaryKey(string)
```csharp
protected virtual string ResolveDictionaryKey(string dictionaryKey)
```

**Parameters** \
`dictionaryKey` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ResolveExtensionDataName(string)
```csharp
protected virtual string ResolveExtensionDataName(string extensionDataName)
```

**Parameters** \
`extensionDataName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ResolvePropertyName(string)
```csharp
protected virtual string ResolvePropertyName(string propertyName)
```

**Parameters** \
`propertyName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetResolvedPropertyName(string)
```csharp
public string GetResolvedPropertyName(string propertyName)
```

**Parameters** \
`propertyName` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### ResolveContract(Type)
```csharp
public virtual JsonContract ResolveContract(Type type)
```

**Parameters** \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[JsonContract](../../) \



⚡