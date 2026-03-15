public class MyArray<T> : IMyCollection<T>
{
    private T[] _items;
    private int _count;

    public bool Dirty { get; set; }

    public int Count => _count;

    public MyArray(int capacity = 10)
    {
        _items = new T[capacity];
        _count = 0;
    }
    public void Add(T item)
    {
        if (_count == _items.Length)
        {
            T[] newArray = new T[_items.Length + 1];

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
        
    }
    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return default;
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
        R result = default;

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
    } // Custom Iterator - Since we are not using System.Collections.Generic 
    public IEnumerator<T> GetEnumerator()
    {
        return default;
    } // Extra foreach lookup. 
}