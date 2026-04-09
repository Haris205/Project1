public class MyBST<T> : IMyCollection<T> where T : IComparable<T>
{
    internal class MyBSTNode<U>
    {
        public U Value;
        public MyBSTNode<U>? Left;
        public MyBSTNode<U>? Right;

        public MyBSTNode(U value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }
    private MyBSTNode<T>? _root;

    private int _count;

    public bool Dirty { get; set; }

    public int Count => _count;

    public MyBST()
    {
    }

    public MyBST(IMyCollection<T> collection)
    {
        var iterator = collection.GetIterator();
        while (iterator.HasNext())
        {
            Add(iterator.Next());
        }
    }

    public void Add(T item)
    {
        _root = AddRecursive(_root, item);
        _count += 1;
        Dirty = true;
    }
    private MyBSTNode<T> AddRecursive(MyBSTNode<T>? node, T item)
    {
        if (node == null)
            return new MyBSTNode<T>(item);

        if (item.CompareTo(node.Value) < 0)
            node.Left = AddRecursive(node.Left, item);
        else
            node.Right = AddRecursive(node.Right, item);

        return node;
    }

    public void Remove(T item)
    {
        bool removed = false;
        _root = RemoveRecursive(_root, item, ref removed);

        if (removed)
            _count -= 1;
            Dirty = true;
    }
    private MyBSTNode<T>? RemoveRecursive(MyBSTNode<T>? node, T item, ref bool removed)
    {
        if (node == null)
            return null;

        int compare = item.CompareTo(node.Value);

        if (compare < 0)
        {
            node.Left = RemoveRecursive(node.Left, item, ref removed);
            return node;
        }

        if (compare > 0)
        {
            node.Right = RemoveRecursive(node.Right, item, ref removed);
            return node;
        }

        removed = true;

        if (node.Left == null && node.Right == null)
            return null;

        if (node.Left == null)
            return node.Right;

        if (node.Right == null)
            return node.Left;

        MyBSTNode<T> successor = FindMinNode(node.Right);
        node.Value = successor.Value;

        bool removedSuccessor = false;
        node.Right = RemoveRecursive(node.Right, successor.Value, ref removedSuccessor);

        return node;
    }

    private MyBSTNode<T> FindMinNode(MyBSTNode<T> node)
    {
        MyBSTNode<T> current = node;
        while (current.Left != null)
            current = current.Left;

        return current;
    }
    
    public T? Find<K>(K key, Func<T, K, bool> comparer)
    {
        return FindRecursive(_root, key, comparer);
    }

    private T? FindRecursive<K>(MyBSTNode<T>? node, K key, Func<T, K, bool> comparer)
    {
        if (node == null)
            return default;

        if (comparer(node.Value, key))
            return node.Value;

        T? leftResult = FindRecursive(node.Left, key, comparer);
        if (!object.Equals(leftResult, default(T)))
            return leftResult;

        return FindRecursive(node.Right, key, comparer);
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        var result = new MyBST<T>();
        FilterRecursive(_root, predicate, result);
        return result;
    }

    private void FilterRecursive(MyBSTNode<T>? node, Func<T, bool> predicate, MyBST<T> result)
    {
        if (node == null)
            return;

        FilterRecursive(node.Left, predicate, result);

        if (predicate(node.Value))
            result.Add(node.Value);

        FilterRecursive(node.Right, predicate, result);
    }

    public void Sort(Comparison<T> comparison)
    {
        if (Count < 2)
            return;

        T[] values = new T[Count];
        int i = 0;
        foreach (var value in this)
        {
            values[i] = value;
            i += 1;
        }

        for (int x = 0; x < values.Length - 1; x++)
        {
            for (int y = 0; y < values.Length - x - 1; y++)
            {
                if (comparison(values[y], values[y + 1]) > 0)
                {
                    T temp = values[y];
                    values[y] = values[y + 1];
                    values[y + 1] = temp;
                }
            }
        }

        _root = null;
        _root = null;
        _count = 0;
        for (int z = 0; z < values.Length; z++)
        {
            Add(values[z]);
        }
        Dirty = true;
    }

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        return Reduce(default!, accumulator);
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        var iterator = GetIterator();

        while (iterator.HasNext())
        {
            result = accumulator(result, iterator.Next());
        }

        return result;
    }

    public T? FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return FindByRecursive(_root, key, comparer);
    }

    private T? FindByRecursive<K>(MyBSTNode<T>? node, K key, Func<T, K, int> comparer)
    {
        if (node == null)
            return default;

        int comparison = comparer(node.Value, key);
        if (comparison == 0)
            return node.Value;

        if (comparison < 0)
            return FindByRecursive(node.Right, key, comparer);
        else
            return FindByRecursive(node.Left, key, comparer);
    }

    public int IndexOf(Func<T, bool> predicate)
    {
        int index = 0;
        foreach (var item in this)
        {
            if (predicate(item))
                return index;
            index++;
        }
        return -1;
    }

    public T? GetAt(int index)
    {
        if (index < 0 || index >= Count)
            return default;

        int current = 0;
        foreach (var item in this)
        {
            if (current == index)
                return item;
            current++;
        }
        return default;
    }

    private IEnumerable<T> InOrderTraversal(MyBSTNode<T>? node)
    {
        if (node == null)
            yield break;

        foreach (var leftValue in InOrderTraversal(node.Left))
        {
            yield return leftValue;
        }

        yield return node.Value;

        foreach (var rightValue in InOrderTraversal(node.Right))
        {
            yield return rightValue;
        }
    }

    public IMyIterator<T> GetIterator()
    {
        return new MyBSTIterator<T>(this);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var value in InOrderTraversal(_root))
        {
            yield return value;
        }
    }
}
