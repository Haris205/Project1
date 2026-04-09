public enum CollectionType { Array, LinkedList, Dictionary, BST }

public static class CollectionFactory
{
    public static IMyCollection<T> Create<T>(CollectionType type) where T : IComparable<T> => type switch
    {
        CollectionType.Array => new MyArray<T>(),
        CollectionType.LinkedList => new MyLinkedList<T>(),
        CollectionType.Dictionary => new MyDictionary<int, T>(),
        CollectionType.BST => new MyBST<T>(),
        _ => throw new ArgumentException("Unknown collection type")
    };
}