[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void Constructor_Array()
    {
        var array = new MyArray<int>();
        Assert.AreEqual(0, array.Count);
        Assert.IsFalse(array.Dirty);
    }

    [TestMethod]
    public void Add_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        Assert.AreEqual(3, array.Count);
    }

    [TestMethod]
    public void Remove_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);

        array.Remove(2);

        Assert.AreEqual(2, array.Count);

        var enumerator = array.GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual(1, enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual(3, enumerator.Current);
    }

    [TestMethod]
    public void Filter_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        array.Add(4);
        var filtered = array.Filter(x => x % 2 == 0);
        Assert.AreEqual(2, filtered.Count);
        var iterator = filtered.GetIterator();
        Assert.IsTrue(iterator.HasNext());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(4, iterator.Next());
        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void Sort_Test()
    {
        var array = new MyArray<int>();
        array.Add(3);
        array.Add(1);
        array.Add(2);
        array.Sort((a, b) => a.CompareTo(b));
        var iterator = array.GetIterator();
        Assert.AreEqual(1, iterator.Next());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(3, iterator.Next());
    }

    [TestMethod]
    public void FindBy_Test()
    {
        var array = new MyArray<int>();
        array.Add(10);
        array.Add(20);
        array.Add(30);
        var result = array.FindBy(20, (item, key) => item.CompareTo(key));
        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void Reduce_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        var sum = array.Reduce<int>((acc, x) => acc + x);
        Assert.AreEqual(6, sum);
    }

    [TestMethod]
    public void Iterator_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        var iterator = array.GetIterator();
        Assert.IsTrue(iterator.HasNext());
        Assert.AreEqual(1, iterator.Next());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(3, iterator.Next());
        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void GetEnumerator_Test()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);

        int expected = 1;

        foreach (var item in array)
        {
            Assert.AreEqual(expected, item);
            expected++;
        }
    }
}




