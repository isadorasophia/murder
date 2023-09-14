# CollectionHelper

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class CollectionHelper
```

### ⭐ Methods
#### ToStringDictionary(Dictionary`2&, IEnumerable<T>, Func<T, TResult>, Func<T, TResult>)
```csharp
public Dictionary<TKey, TValue> ToStringDictionary(Dictionary`2& existingDictionary, IEnumerable<T> collection, Func<T, TResult> toKey, Func<T, TResult> toValue)
```

**Parameters** \
`existingDictionary` [Dictionary\<TKey, TValue\>&](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
`collection` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`toKey` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \
`toValue` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \

#### ToStringDictionary(IEnumerable<T>, Func<T, TResult>, Func<T, TResult>)
```csharp
public Dictionary<TKey, TValue> ToStringDictionary(IEnumerable<T> collection, Func<T, TResult> toKey, Func<T, TResult> toValue)
```

**Parameters** \
`collection` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`toKey` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \
`toValue` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \



⚡