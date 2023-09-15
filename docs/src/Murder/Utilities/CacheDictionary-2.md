# CacheDictionary\<TKey, TValue\>

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public sealed class CacheDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<T>, IEnumerable<T>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback, IDisposable
```

A dictionary that has a maximum amount of entries and discards old entries as new ones are added

**Implements:** _[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0), [IDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2?view=net-7.0), [ICollection\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ICollection-1?view=net-7.0), [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable?view=net-7.0), [IDictionary](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.IDictionary?view=net-7.0), [ICollection](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.ICollection?view=net-7.0), [IReadOnlyDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyDictionary-2?view=net-7.0), [IReadOnlyCollection\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyCollection-1?view=net-7.0), [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/System.Runtime.Serialization.ISerializable?view=net-7.0), [IDeserializationCallback](https://learn.microsoft.com/en-us/dotnet/api/System.Runtime.Serialization.IDeserializationCallback?view=net-7.0), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public CacheDictionary<TKey, TValue>(int size)
```

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Comparer
```csharp
public IEqualityComparer<T> Comparer { get; }
```

**Returns** \
[IEqualityComparer\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEqualityComparer-1?view=net-7.0) \
#### Count
```csharp
public virtual int Count { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Item
```csharp
public TValue Item { get; public set; }
```

**Returns** \
[TValue](../../) \
#### Keys
```csharp
public KeyCollection<TKey, TValue> Keys { get; }
```

**Returns** \
[KeyCollection\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.KeyCollection-KeyCollection?view=net-7.0) \
#### Values
```csharp
public ValueCollection<TKey, TValue> Values { get; }
```

**Returns** \
[ValueCollection\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.ValueCollection-ValueCollection?view=net-7.0) \
### ⭐ Methods
#### ContainsValue(TValue)
```csharp
public bool ContainsValue(TValue value)
```

**Parameters** \
`value` [TValue](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Remove(TKey, out TValue&)
```csharp
public bool Remove(TKey key, TValue& value)
```

**Parameters** \
`key` [TKey](../../) \
`value` [TValue&](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryAdd(TKey, TValue)
```csharp
public bool TryAdd(TKey key, TValue value)
```

**Parameters** \
`key` [TKey](../../) \
`value` [TValue](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetEnumerator()
```csharp
public Enumerator<TKey, TValue> GetEnumerator()
```

**Returns** \
[Enumerator\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Enumerator-Enumerator?view=net-7.0) \

#### EnsureCapacity(int)
```csharp
public int EnsureCapacity(int capacity)
```

**Parameters** \
`capacity` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ContainsKey(TKey)
```csharp
public virtual bool ContainsKey(TKey key)
```

**Parameters** \
`key` [TKey](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Remove(TKey)
```csharp
public virtual bool Remove(TKey key)
```

**Parameters** \
`key` [TKey](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetValue(TKey, out TValue&)
```csharp
public virtual bool TryGetValue(TKey key, TValue& value)
```

**Parameters** \
`key` [TKey](../../) \
`value` [TValue&](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Add(TKey, TValue)
```csharp
public virtual void Add(TKey key, TValue value)
```

**Parameters** \
`key` [TKey](../../) \
`value` [TValue](../../) \

#### Clear()
```csharp
public virtual void Clear()
```

#### Dispose()
```csharp
public virtual void Dispose()
```

#### GetObjectData(SerializationInfo, StreamingContext)
```csharp
public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
```

**Parameters** \
`info` [SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/System.Runtime.Serialization.SerializationInfo?view=net-7.0) \
`context` [StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/System.Runtime.Serialization.StreamingContext?view=net-7.0) \

#### OnDeserialization(Object)
```csharp
public virtual void OnDeserialization(Object sender)
```

**Parameters** \
`sender` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

#### TrimExcess()
```csharp
public void TrimExcess()
```

#### TrimExcess(int)
```csharp
public void TrimExcess(int capacity)
```

**Parameters** \
`capacity` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡