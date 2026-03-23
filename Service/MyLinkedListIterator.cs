public class MyLinkedListIterator<T> : IMyIterator<T>
{
    private MyLinkedList<T>.Node _current;
    private MyLinkedList<T>.Node _head;

    public MyLinkedListIterator(MyLinkedList<T> list)
    {
        _head = list._head!;
        _current = null!;
    }
    public bool HasNext()
    {
        if (_head == null) return _head != null;
        return _current.Next != null;
    }
    public T Next()      
    {
        _current = _current == null ? _head : _current.Next!;

        if (_current == null)
            throw new InvalidOperationException("No more items left");

        return _current.Value;   
    }
    public void Reset()
    {
        _current = null!;
    }
}