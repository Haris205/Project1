public class MyDictionary<T> : IMyCollection<T>
{
    private T[] _items;
    private int _count;

    public bool Dirty { get; set; }

    public int Count => _count;

    public MyDictionary(int capacity)
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
        
    }
    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return default!;
    }
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        return default!;
    }
    public void Sort(Comparison<T> comparison)
    {
        
    }
    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        return default!;
    }
    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        return default!;
    }
    public IMyIterator<T> GetIterator()
    {
        return default!;
    }
    public IEnumerator<T> GetEnumerator()
    {
        return default!;
    }
}