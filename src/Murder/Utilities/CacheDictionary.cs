namespace Murder.Utilities
{
    /// <summary>
    /// A dictionary that has a maximum amount of entries and discards old entries as new ones are added
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class CacheDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable where TKey : notnull
    {
        private readonly int _maxSize;
        private Queue<TKey> _keys;

        new public TValue this[TKey key]
        {
            get => base[key];
            set => Add(key, value);
        }

        public CacheDictionary(int size)
        {
            _maxSize = size;
            _keys = new Queue<TKey>();
        }

        new public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            base[key] = value;
            _keys.Enqueue(key);

            if (_keys.Count > _maxSize)
            {
                Remove(_keys.Dequeue());
            }
        }

        new public bool Remove(TKey key)
        {
            if (!_keys.Contains(key))
            {
                return false;
            }

            var newQueue = new Queue<TKey>();
            var alreadyAddedKeys = new HashSet<TKey>();

            while (_keys.Count > 0)
            {
                var thisKey = _keys.Dequeue();
                if (!thisKey.Equals(key) && !alreadyAddedKeys.Contains(thisKey))
                {
                    newQueue.Enqueue(thisKey);
                }

                alreadyAddedKeys.Add(thisKey);
            }

            _keys = newQueue;
            return base.Remove(key);
        }

        public void Dispose()
        {
            while (_keys.Count > 0)
            {
                if (TryGetValue(_keys.Dequeue(), out TValue? value))
                {
                    (value as IDisposable)?.Dispose();
                }
            }

            Clear();
        }
    }
}
