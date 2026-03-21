public class MyLinkedList<T> : IMyCollection<T>
{
    internal class Node
    {
        public T Value;
        public Node? Next;

        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }
    
    internal Node? _head;
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
        Dirty = true;
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
        _head = MergeSort(_head!, comparison);
        Dirty = true;
    }
    private Node MergeSort(Node node, Comparison<T> comparison)
    {
        if (node == null || node.Next == null) return node!;

        Node middle = GetMiddle(node);
        Node secondHalf = middle.Next!;
        middle.Next = null;

        Node left = MergeSort(node, comparison);
        Node right = MergeSort(secondHalf, comparison);
        
        return Merge(left, right, comparison);
    }
    private Node GetMiddle(Node node)
    {
        Node slow = node;
        Node fast = node.Next!;
        
        while (fast != null && fast.Next != null)
        {
            slow = slow.Next!;
            fast = fast.Next!.Next!;
        }
        return slow;
    }
    private Node Merge(Node left, Node right, Comparison<T> comparison)
    {
        if (left == null) return right;
        if (right == null) return left;

        if (comparison(left.Value ,right.Value) <= 0)
        {
            left.Next = Merge(left.Next!, right, comparison);
            return left;
        }
        else
        {
            right.Next = Merge(left, right.Next!, comparison);
            return right;
        }
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
        return new MyLinkedListIterator<T>(this);
    }
    public IEnumerator<T> GetEnumerator()
    {
        return default!;
    }
}