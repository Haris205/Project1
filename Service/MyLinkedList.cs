using System.Diagnostics;

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
        if(item == null)
            return;
        Node newNode = new Node(item); 
        newNode.Next = _head;
        _head = newNode;      
        _count++;
        Dirty = true;   
    }
    public void Remove(T item)
    {
        while (_head != null && _head.Value!.Equals(item))
        {
            _head = _head.Next;
            _count--;
            Dirty = true;
        }

        Node? current = _head;
        Node? previous = null;

        while (current != null)
        {
            if (current.Value!.Equals(item))
            {
                previous!.Next = current.Next;
                _count--;
                Dirty = true;
                current = current.Next;
            }
            else
            {
                previous = current;
                current = current.Next;
            }
        }
    }
    public T FindBy<K>(K key, Func<T, K, int> comparer)
    {
        Node? current = _head;
        while (current != null)
        {
            if (comparer(current.Value, key) == 0)
                return current.Value;
            current = current.Next;
        }
        throw new InvalidOperationException("Element not found");
    }
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyLinkedList<T> result = new MyLinkedList<T>();

        Node? current = _head;
        while (current != null)
        {
            if (predicate(current.Value))
                result.Add(current.Value);
            current = current.Next;
        }
        return result;
    }
    public void Sort(Comparison<T> comparison)
    {
        if (_head == null)
            return;

        _head = MergeSort(_head, comparison);
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
        Node? current = _head;
        R initial = default!;
        while(current != null)
        {
            initial = accumulator(initial, current.Value);
            current = current.Next;
        }
        return initial;
    }
    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        Node? current = _head;
        R result = initial;
        while(current != null)
        {
            result = accumulator(result, current.Value);
            current = current.Next;
        }
        return result;
    }
    public IMyIterator<T> GetIterator()
    {
        return new MyLinkedListIterator<T>(this);
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
        var array = new T[_count];
        int index = 0;
        foreach (var item in this)
        {
            array[index++] = item;
        }
        return array;
    }
    public int IndexOf(Func<T, bool> predicate)
    {
        int index = 0;

        var current = _head;
        while (current != null)
        {
            if (predicate(current.Value))
                return index;
            
            current = current.Next;
            index++;
        }
        return -1;
    }
    public T GetAt(int index)
    {
        if (index < 0)
            return default!;
        
        var current = _head;
        int currentIndex = 0;
        
        while (current != null)
        {
            if (currentIndex == index)
                return current.Value;

            current = current.Next;
            currentIndex++;
        }
        return default!;
    }
}