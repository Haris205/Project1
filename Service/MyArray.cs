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
        
    }
    public void Remove(T item)
    {
        
    }
    public T? FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return default;
    }
    public IMyCollection<T>? Filter(Func<T, bool> predicate)
    {
        return default;
    }
    public void Sort(Comparison<T> comparison)
    {
        
    }
    public R? Reduce<R>(Func<R, T, R> accumulator)
    {
        return default;
    }
    // OR 
    public R? Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        return default;
    }
    public IMyIterator<T>? GetIterator()
    {
        return default;
    } // Custom Iterator - Since we are not using System.Collections.Generic 
    public IEnumerator<T>? GetEnumerator()
    {
        return default;
    } // Extra foreach lookup. 
}