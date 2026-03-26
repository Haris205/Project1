[TestClass]
public sealed class MyArrayTest
{
    public void Constructor_Array()
    {
        var array = new MyArray<int>();
        Assert.AreEqual(0, array.Count);
        Assert.IsFalse(array.Dirty);
    }

    [TestMethod]
    public void AddSingleTask()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        Assert.AreEqual(1, array.Count);
    }

    [TestMethod]
    public void AddMultipleTask()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        array.Add("Task 2");
        array.Add("Task 3");
        Assert.AreEqual(3, array.Count);
    }

    [TestMethod]
    public void AddMoreThanCapacity()
    {
        var array = new MyArray<string>(2);
        array.Add("Task 1");
        array.Add("Task 2");
        array.Add("Task 3");
        Assert.AreEqual(3, array.Count);
    }

    [TestMethod]
    public void AddSetsDirtyToTrue()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        Assert.IsTrue(array.Dirty);
    }

    [TestMethod]
    public void FilterByMatchingPredicate()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        array.Add("Wash your car");
        array.Add("Buy clothes");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void FilterNoMatchingPredicate()
    {
        var array = new MyArray<string>();
        array.Add("Wash your car");
        array.Add("Clean room");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FilterAllMatching()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        array.Add("Buy clothes");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SortIntegersAscending()
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
    public void SortSetsDirtyTrue()
    {
        var array = new MyArray<int>();
        array.Add(3);
        array.Add(1);

        array.Dirty = false;
        array.Sort((a, b) => a.CompareTo(b));

        Assert.IsTrue(array.Dirty);
    }

    [TestMethod]
    public void GetIteratorOneItem()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");

        var iterator = array.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorMultipleItems()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        array.Add("Task 2");

        var iterator = array.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
        Assert.AreEqual("Task 2", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorReturnsFalse()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");

        var iterator = array.GetIterator();
        iterator.Next();

        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void GetIteratorReset()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        array.Add("Task 2");

        var iterator = array.GetIterator();
        iterator.Next();
        iterator.Next();
        iterator.Reset();

        Assert.AreEqual("Task 1", iterator.Next());
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

    [TestMethod]
    public void MyLinkedList_GetEnumerator_Test()
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
    public void MyLinkedList_Remove_OnEmpty_DoesNotThrow()
    {
        var list = new MyLinkedList<int>();
        list.Remove(42);
        Assert.AreEqual(0, list.Count);
    }
}
