public class MyLinkedList<T> : IMyCollection<T>
{
    private class Node
    {
        public T Value;
        public Node? Next;

        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }
    
    private Node? _head;
    public bool Dirty { get; set; }
    private int _count;
    public int Count => _count;

    public void Add(T item)
    {
        
    }
    public void Remove(T item)
    {
        while (_head != null && _head.Value!.Equals(item))
            _head = _head.Next;

        Node current = _head!;
        while (current != null)
        {
            if (current?.Next != null && current.Next!.Value!.Equals(item))
                current.Next = current!.Next!.Next!;
            else
                current = current!.Next!;
        }
    }
    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return default!;
    }
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyLinkedList<T> result = new MyLinkedList<T>();

        Node current = _head!;
        while (current != null)
        {
            if (predicate(current.Value))
                result.Add(current.Value);
            current = current.Next!;
        }
        return result;
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