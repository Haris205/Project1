public class MyArrayIterator<T> : IMyIterator<T>
{
    private T[] _items;
    private int _count;
    private int _pos = 0;

    public MyArrayIterator(T[] items, int count)
    {
        _items = items;
        _count = count;
    }
    public bool HasNext()
    {
        return _pos < _count;
    }
    public T Next()      
    {
        return _items[_pos++];
    } 
    public void Reset()
    {
        _pos = 0;
    }
}