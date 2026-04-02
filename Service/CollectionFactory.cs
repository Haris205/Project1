public enum CollectionType { Array, LinkedList, Dictionary }

public static class CollectionFactory
{
    public static IMyCollection<T> Create<T>(CollectionType type) => type switch
    {
        CollectionType.Array => new MyArray<T>(),
        CollectionType.LinkedList => new MyLinkedList<T>(),
        CollectionType.Dictionary => new MyDictionary<T>(),
        _ => throw new ArgumentException("Unknown collection type")
    };
}