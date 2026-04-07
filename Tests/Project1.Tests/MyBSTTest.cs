namespace Project1.Tests;

[TestClass]
public sealed class MyBSTTest
{
    [TestMethod]
    public void Constructor_BST()
    {
        var bst = new MyBST<int>();
        Assert.AreEqual(0, bst.Count);
        Assert.IsFalse(bst.Dirty);
    }

    [TestMethod]
    public void AddSingleItem_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        Assert.AreEqual(1, bst.Count);
    }

    [TestMethod]
    public void AddMultipleItems_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);
        bst.Add(7);
        Assert.AreEqual(3, bst.Count);
    }

    [TestMethod]
    public void AddSetsDirtyToTrue_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        Assert.IsTrue(bst.Dirty);
    }

    [TestMethod]
    public void FilterByMatchingPredicate_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);
        bst.Add(7);
        bst.Add(1);
        bst.Add(9);

        var result = bst.Filter(x => x > 3);
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void FilterNoMatchingPredicate_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(1);
        bst.Add(2);

        var result = bst.Filter(x => x > 10);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FilterAllMatching_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);

        var result = bst.Filter(x => x < 10);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SortIntegersAscending_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(3);
        bst.Add(1);
        bst.Add(2);

        bst.Sort((a, b) => a.CompareTo(b));

        var iterator = bst.GetIterator();
        Assert.AreEqual(1, iterator.Next());
        Assert.AreEqual(2, iterator.Next());
        Assert.AreEqual(3, iterator.Next());
    }

    [TestMethod]
    public void SortSetsDirtyTrue_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(3);
        bst.Add(1);

        bst.Dirty = false;
        bst.Sort((a, b) => a.CompareTo(b));

        Assert.IsTrue(bst.Dirty);
    }

    [TestMethod]
    public void GetIteratorOneItem_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);

        var iterator = bst.GetIterator();

        Assert.AreEqual(5, iterator.Next());
    }

    [TestMethod]
    public void GetIteratorMultipleItems_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);
        bst.Add(7);

        var iterator = bst.GetIterator();

        Assert.AreEqual(3, iterator.Next());
        Assert.AreEqual(5, iterator.Next());
        Assert.AreEqual(7, iterator.Next());
    }

    [TestMethod]
    public void GetIteratorReturnsFalse_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);

        var iterator = bst.GetIterator();
        iterator.Next();

        Assert.IsFalse(iterator.HasNext());
    }

    [TestMethod]
    public void GetIteratorReset_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);

        var iterator = bst.GetIterator();
        iterator.Next();
        iterator.Next();
        iterator.Reset();

        Assert.AreEqual(3, iterator.Next());
    }

    [TestMethod]
    public void Remove_Test_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(5);
        bst.Add(3);
        bst.Add(7);

        bst.Remove(3);

        Assert.AreEqual(2, bst.Count);

        var enumerator = bst.GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual(5, enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual(7, enumerator.Current);
    }

    [TestMethod]
    public void FindBy_Test_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(10);
        bst.Add(20);
        bst.Add(30);
        var result = bst.FindBy(20, (item, key) => item.CompareTo(key));
        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void Reduce_Test_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(1);
        bst.Add(2);
        bst.Add(3);
        var sum = bst.Reduce(0, (acc, x) => acc + x);
        Assert.AreEqual(6, sum);
    }

    [TestMethod]
    public void GetEnumerator_Test_BST()
    {
        var bst = new MyBST<int>();
        bst.Add(1);
        bst.Add(2);
        bst.Add(3);

        int expected = 1;

        foreach (var item in bst)
        {
            Assert.AreEqual(expected, item);
            expected++;
        }
    }
}
