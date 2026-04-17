public class MyDictionary<K, T> : IMyCollection<T>
{
    private class Entry
    {
        public K Key;
        public T Value;

        public Entry(K key, T value)
        {
            Key = key;
            Value = value;
        }
    }

    private MyLinkedList<Entry>[] _buckets;
    private int _count;

    public bool Dirty { get; set; }
    public int Count => _count;

    public MyDictionary(int capacity = 16)
    {
        if (capacity <= 0)
            capacity = 16;

        _buckets = new MyLinkedList<Entry>[capacity];

        for (int i = 0; i < capacity; i++)
        {
            _buckets[i] = new MyLinkedList<Entry>();
        }
    }

    private int GetIndex(K key)
    {
        return Math.Abs(key!.GetHashCode()) % _buckets.Length;
    }

    public void Add(K key, T value)
    {
        int index = GetIndex(key);
        var bucket = _buckets[index];

        foreach (var entry in bucket)
        {
            if (Equals(entry.Key, key))
            {
                entry.Value = value;
                Dirty = true;
                return;
            }
        }

        bucket.Add(new Entry(key, value));
        _count++;
        Dirty = true;
    }

    public void Add(T item)
    {
        if (item is TaskItem task)
        {
            Add((K)(object)task.Id, item);
        }
    }

    public void Remove(T item)
    {
        if (item is TaskItem task)
        {
            RemoveKey((K)(object)task.Id);
        }
    }

    private void RemoveKey(K key)
    {
        int index = GetIndex(key);
        var bucket = _buckets[index];

        foreach (var entry in bucket)
        {
            if (Equals(entry.Key, key))
            {
                bucket.Remove(entry);
                _count--;
                Dirty = true;
                return;
            }
        }
    }
    
    public T FindBy<K2>(K2 key, Func<T, K2, int> comparer)
    {
        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                if (comparer(entry.Value, key) == 0)
                    return entry.Value;
            }
        }

        return default!;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        var result = new MyDictionary<K, T>();

        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                if (predicate(entry.Value))
                    result.Add(entry.Key, entry.Value);
            }
        }

        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        var items = new List<(K key, T value)>();

        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                items.Add((entry.Key, entry.Value));
            }
        }

        items.Sort((a, b) => comparison(a.value, b.value));

        foreach (var bucket in _buckets)
        {
            bucket.Clear();
        }

        _count = 0;

        foreach (var item in items)
        {
            Add(item.key, item.value);
        }

        Dirty = true;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        R result = default!;

        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                result = accumulator(result, entry.Value);
            }
        }

        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                result = accumulator(result, entry.Value);
            }
        }

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
        var values = new List<T>();

        foreach (var bucket in _buckets)
        {
            foreach (var entry in bucket)
            {
                values.Add(entry.Value);
            }
        }

        return new MyArrayIterator<T>(values.ToArray(), values.Count);
    }

    public IEnumerator<T> GetEnumerator()
    {
        var iterator = GetIterator();
        while (iterator.HasNext())
        {
            yield return iterator.Next();
        }
    }

    public int IndexOf(Func<T, bool> predicate)
    {
        int index = 0;

        foreach (var bucket in _buckets)
        {
            var current = bucket.Head;

            while (current != null)
            {
                var entry = current.Value;

                if (predicate(entry.Value))
                    return index;

                index++;
                current = current.Next;
            }
        }

        return -1;
    }

    public T GetAt(int index)
    {
        if (index < 0 || index >= _count)
            return default!;

        int currentIndex = 0;

        foreach (var bucket in _buckets)
        {
            var current = bucket.Head;
            while (current != null)
            {
                if (currentIndex == index)
                {
                    var entry = current.Value;
                    return entry.Value;
                }
                currentIndex++;
                current = current.Next;
            }
        }

        return default!;
    }
}