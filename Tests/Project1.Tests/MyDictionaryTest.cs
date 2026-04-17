[TestClass]
public sealed class MyDictionaryTest
{
    [TestMethod]
    public void Constructor_Dict()
    {
        var dict = new MyDictionary<int, string>();
        Assert.AreEqual(0, dict.Count);
        Assert.IsFalse(dict.Dirty);
    }

    [TestMethod]
    public void AddSingleItem_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Buy groceries");

        Assert.AreEqual(1, dict.Count);
    }

    [TestMethod]
    public void AddMultipleItems_Dict()
    {
        var dict = new MyDictionary<int, string>();

        dict.Add(1, "Task 1");
        dict.Add(2, "Task 2");
        dict.Add(3, "Task 3");

        Assert.AreEqual(3, dict.Count);
    }

    [TestMethod]
    public void AddSetsDirtyToTrue_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Task 1");

        Assert.IsTrue(dict.Dirty);
    }

    [TestMethod]
    public void FilterByMatchingPredicate_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Buy groceries");
        dict.Add(2, "Wash your car");
        dict.Add(3, "Buy clothes");

        var result = dict.Filter(v => v.StartsWith("Buy"));

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void FilterNoMatchingPredicate_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Wash your car");
        dict.Add(2, "Clean room");

        var result = dict.Filter(v => v.StartsWith("Buy"));

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FilterAllMatching_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Buy groceries");
        dict.Add(2, "Buy clothes");

        var result = dict.Filter(v => v.StartsWith("Buy"));

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SortByValueAscending_Dict()
    {
        var dict = new MyDictionary<int, int>();
        dict.Add(1, 3);
        dict.Add(2, 1);
        dict.Add(3, 2);

        dict.Sort((a, b) => a.CompareTo(b));

        var iterator = dict.GetIterator();

        Assert.AreEqual(1, iterator.Next());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(3, iterator.Next());
    }

    [TestMethod]
    public void GetIteratorOneItem_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Task 1");

        var iterator = dict.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorMultipleItems_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Task 1");
        dict.Add(2, "Task 2");

        var iterator = dict.GetIterator();

        Assert.AreEqual("Task 1", iterator.Next());
        Assert.AreEqual("Task 2", iterator.Next());
    }

    [TestMethod]
    public void GetIteratorReturnsFalse_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Task 1");

        var iterator = dict.GetIterator();
        iterator.Next();

        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void GetIteratorReset_Dict()
    {
        var dict = new MyDictionary<int, string>();
        dict.Add(1, "Task 1");
        dict.Add(2, "Task 2");

        var iterator = dict.GetIterator();
        iterator.Next();
        iterator.Next();
        iterator.Reset();

        Assert.AreEqual("Task 1", iterator.Next());
    }

    [TestMethod]
    public void Remove_Test_Dict()
    {
        var dict = new MyDictionary<int, int>();
        dict.Add(1, 1);
        dict.Add(2, 2);
        dict.Add(3, 3);

        dict.Remove(2);

        Assert.AreEqual(2, dict.Count);

        var enumerator = dict.GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual(1, enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual(3, enumerator.Current);
    }

    [TestMethod]
    public void FindBy_Test_Dict()
    {
        var dict = new MyDictionary<int, int>();
        dict.Add(1, 10);
        dict.Add(2, 20);
        dict.Add(3, 30);

        var result = dict.FindBy(20, (value, key) => value.CompareTo(key));

        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void Reduce_Test_Dict()
    {
        var dict = new MyDictionary<int, int>();
        dict.Add(1, 1);
        dict.Add(2, 2);
        dict.Add(3, 3);

        var sum = dict.Reduce<int>((acc, value) => acc + value);

        Assert.AreEqual(6, sum);
    }

    [TestMethod]
    public void GetEnumerator_Test_Dict()
    {
        var dict = new MyDictionary<int, int>();
        dict.Add(1, 1);
        dict.Add(2, 2);
        dict.Add(3, 3);

        int expected = 1;

        foreach (var item in dict)
        {
            Assert.AreEqual(expected, item);
            expected++;
        }
    }
}