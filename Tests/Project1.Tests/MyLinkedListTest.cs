[TestClass]
public sealed class MyLinkedListTest
{
    [TestMethod]
    public void AddAndCount_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        Assert.AreEqual(3, list.Count);
        Assert.IsTrue(list.Dirty);
    }

    [TestMethod]
    public void GetEnumerator_OrderIsLifo_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var expected = 3;
        foreach (var item in list)
        {
            Assert.AreEqual(expected, item);
            expected--;
        }
    }

    [TestMethod]
    public void Remove_ExistingItem_DecreasesCount_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        list.Remove(2);

        Assert.AreEqual(2, list.Count);
        var found = list.FindBy(3, (value, key) => value.CompareTo(key));
        Assert.AreEqual(3, found);
    }

    [TestMethod]
    public void Remove_NonExistingItem_NoChange_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(1);

        list.Remove(999);

        Assert.AreEqual(1, list.Count);
    }

    [TestMethod]
    public void Filter_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var evens = list.Filter(x => x % 2 == 0);

        Assert.AreEqual(1, evens.Count);
        Assert.AreEqual(2, evens.FindBy(2, (value, key) => value.CompareTo(key)));
    }

    [TestMethod]
    public void Sort_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(5);
        list.Add(2);
        list.Add(8);

        list.Sort((a, b) => a.CompareTo(b));

        var iterator = list.GetIterator();
        Assert.IsTrue(iterator.HasNext());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(5, iterator.Next());
        Assert.AreEqual(8, iterator.Next());
    }

    [TestMethod]
    public void Reduce_Test()
    {
        var list = new MyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var sum = list.Reduce(0, (acc, value) => acc + value);

        Assert.AreEqual(6, sum);
    }
}
