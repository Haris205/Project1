public class MyArray<T> : IMyCollection<T>
{
    private T[] _items;
    private int _count;

    public bool Dirty { get; set; }

    public int Count => _count;

    public MyArray(int capacity = 10)
    {
        if (capacity <= 0)
            capacity = 4;
        _items = new T[capacity];
        _count = 0;
    }
    public void Add(T item)
    {
        if (_count == _items.Length)
        {
            T[] newArray = new T[_items.Length * 2];

            for (int i = 0; i < _items.Length; i++)
            {
                newArray[i] = _items[i];
            }
            _items = newArray;
        }
        
        _items[_count] = item;
        _count++;
        Dirty = true;
    }
    public void Remove(T item)
    {
        int index = -1;
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_items[i], item))
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            return;
        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }
        _count--;
        Dirty = true;
    }
    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        for (int i = 0; i < _count; i++)
        {
            if (comparer(_items[i], key) == 0)
            {
                return _items[i];
            }
        }
        return default!;
    }
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyArray<T> result = new MyArray<T>();

        for (int i = 0; i < _count; i++)
        {
            if (predicate(_items[i]))
            {
                result.Add(_items[i]);
            }
        }
        return result;
    }
    public void Sort(Comparison<T> comparison)
    {
        for (int i = 1; i < _count; i++)
        {
            T key = _items[i];
            int j = i - 1;

            while (j >= 0 && comparison(_items[j], key) > 0)
            {
                _items[j + 1] = _items[j];
                j--;
            }

            _items[j + 1] = key;
        }
        Dirty = true;
    }
    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        R result = default!;

        for (int i = 0; i < _count; i++)
        {
            result = accumulator(result, _items[i]);
        }
        return result;
    }
    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i < _count; i++)
        {
            result = accumulator(result, _items[i]);
        }
        return result;
    }
    public IMyIterator<T> GetIterator()
    {
        return new MyArrayIterator<T>(_items, _count);
    }
    public IEnumerator<T> GetEnumerator()
    {
        var iterator = GetIterator();
        while (iterator.HasNext())
        {
            yield return iterator.Next();
        }
    }

    public T[] ToArray()
    {
        T[] result = new T[_count];
        for (int i = 0; i < _count; i++)
        { 
            result[i] = _items[i];
        }
        return result;
    }
}