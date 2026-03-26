public enum CollectionType { Array, LinkedList }

public static class CollectionFactory
{
    public static IMyCollection<T> Create<T>(CollectionType type) => type switch
    {
        CollectionType.Array => new MyArray<T>(),
        CollectionType.LinkedList => new MyLinkedList<T>(),
        _ => throw new ArgumentException("Unknown collection type")
    };
}