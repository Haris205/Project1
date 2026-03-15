[TestClass]
public sealed class TestMyArray
{
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
}
