public class MyDictionary<K, T> : IMyCollection<T>
{
    private class Entry
    {
        public K Key;
        public T Value;
        public Entry? Next;

        public Entry(K key, T value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    private Entry?[] _buckets;
    private int _count;

    public bool Dirty { get; set; }
    public int Count => _count;

    public MyDictionary(int capacity = 16)
    {
        if (capacity <= 0)
            capacity = 16;

        _buckets = new Entry[capacity];
        _count = 0;
    }

    private int GetIndex(K key)
    {
        return Math.Abs(key!.GetHashCode()) % _buckets.Length;
    }

    public void Add(T item)
    {
        if (item is TaskItem task)
        {
            Add((K)(object)task.Id, item);
        }
    }
    public void Add(K key, T value)
    {
        int index = GetIndex(key);
        Entry? current = _buckets[index];

        while (current != null)
        {
            if (Equals(current.Key, key))
            {
                current.Value = value;
                Dirty = true;
                return;
            }
            current = current.Next;
        }

        Entry newEntry = new Entry(key, value);
        newEntry.Next = _buckets[index];
        _buckets[index] = newEntry;

        _count++;
        Dirty = true;
    }

    public void Remove(T item)
    {
        return;
    }
    public void Remove(K key)
    {
        int index = GetIndex(key);
        Entry? current = _buckets[index];
        Entry? prev = null;

        while (current != null)
        {
            if (Equals(current.Key, key))
            {
                if (prev == null)
                    _buckets[index] = current.Next;
                else
                    prev.Next = current.Next;

                _count--;
                Dirty = true;
                return;
            }

            prev = current;
            current = current.Next;
        }
    }

    public T FindBy<K2>(K2 key, Func<T, K2, int> comparer)
    {
        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                if (comparer(current.Value, key) == 0)
                    return current.Value;

                current = current.Next;
            }
        }
        return default!;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyDictionary<K, T> result = new MyDictionary<K, T>();

        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                if (predicate(current.Value))
                    result.Add(current.Key, current.Value);

                current = current.Next;
            }
        }

        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        List<(K key, T value)> items = new List<(K, T)>();

        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                items.Add((current.Key, current.Value));
                current = current.Next;
            }
        }

        items.Sort((a, b) => comparison(a.value, b.value));

        _buckets = new Entry[_buckets.Length];
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
            Entry? current = bucket;
            while (current != null)
            {
                result = accumulator(result, current.Value);
                current = current.Next;
            }
        }

        return result;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                result = accumulator(result, current.Value);
                current = current.Next;
            }
        }

        return result;
    }

    public int IndexOf(Func<T, bool> predicate)
    {
        int index = 0;

        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                if (predicate(current.Value))
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
            Entry? current = bucket;
            while (current != null)
            {
                if (currentIndex == index)
                    return current.Value;

                currentIndex++;
                current = current.Next;
            }
        }

        return default!;
    }
    public IMyIterator<T> GetIterator()
    {
        List<T> values = new List<T>();

        foreach (var bucket in _buckets)
        {
            Entry? current = bucket;
            while (current != null)
            {
                values.Add(current.Value);
                current = current.Next;
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
}