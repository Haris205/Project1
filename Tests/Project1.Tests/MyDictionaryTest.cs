[TestClass]
public sealed class MyDictionaryTest
{
    public void Constructor_Dict()
    {
        var array = new MyArray<int>();
        Assert.AreEqual(0, array.Count);
        Assert.IsFalse(array.Dirty);
    }

    [TestMethod]
    public void AddSingleTask_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        Assert.AreEqual(1, array.Count);
    }

    [TestMethod]
    public void AddMultipleTask_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        array.Add("Task 2");
        array.Add("Task 3");
        Assert.AreEqual(3, array.Count);
    }

    [TestMethod]
    public void AddMoreThanCapacity_Dict()
    {
        var array = new MyArray<string>(2);
        array.Add("Task 1");
        array.Add("Task 2");
        array.Add("Task 3");
        Assert.AreEqual(3, array.Count);
    }

    [TestMethod]
    public void AddSetsDirtyToTrue_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        Assert.IsTrue(array.Dirty);
    }

    [TestMethod]
    public void FilterByMatchingPredicate_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        array.Add("Wash your car");
        array.Add("Buy clothes");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void FilterNoMatchingPredicate_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Wash your car");
        array.Add("Clean room");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FilterAllMatching_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Buy groceries");
        array.Add("Buy clothes");

        var result = array.Filter(t => t.StartsWith("Buy"));
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SortIntegersAscending_Dict()
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
    public void SortSetsDirtyTrue_Dict()
    {
        var array = new MyArray<int>();
        array.Add(3);
        array.Add(1);

        array.Dirty = false;
        array.Sort((a, b) => a.CompareTo(b));

        Assert.IsTrue(array.Dirty);
    }

    [TestMethod]
    public void GetIteratorOneItem_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");

        var iterator = array.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorMultipleItems_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");
        array.Add("Task 2");

        var iterator = array.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
        Assert.AreEqual("Task 2", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorReturnsFalse_Dict()
    {
        var array = new MyArray<string>();
        array.Add("Task 1");

        var iterator = array.GetIterator();
        iterator.Next();

        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void GetIteratorReset_Dict()
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
    public void Remove_Test_Dict()
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
    public void FindBy_Test_Dict()
    {
        var array = new MyArray<int>();
        array.Add(10);
        array.Add(20);
        array.Add(30);
        var result = array.FindBy(20, (item, key) => item.CompareTo(key));
        Assert.AreEqual(20, result);
    }
    
    [TestMethod]
    public void Reduce_Test_Dict()
    {
        var array = new MyArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);
        var sum = array.Reduce<int>((acc, x) => acc + x);
        Assert.AreEqual(6, sum);
    }

    [TestMethod]
    public void GetEnumerator_Test_Dict()
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
