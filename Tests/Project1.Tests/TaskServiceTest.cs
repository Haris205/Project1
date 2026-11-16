[TestClass]
public sealed class TaskServiceTest
{
    [TestMethod]
    public void ToggleTaskState_RejectsUserWhoDoesNotOwnTask_Test()
    {
        var service = CreateService(
            CreateTask(1, "Alice"));

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.ToggleTaskState(1, "Bob", Status.Done));
    }

    [TestMethod]
    public void ToggleTaskState_CannotCompleteTaskBeforeDependencies_Test()
    {
        var service = CreateService(
            CreateTask(1, "Alice"),
            CreateTask(2, "Alice", 1));

        Assert.Throws<InvalidOperationException>(() =>
            service.ToggleTaskState(2, "Alice", Status.Done));
    }

    [TestMethod]
    public void UpdateTask_UpdatesPriority_Test()
    {
        var service = CreateService(CreateTask(1, "Alice"));

        service.UpdateTask(1, "Updated task", "Alice", "Alice", PriorityLevel.High);

        var updatedTask = FindTask(service, 1) ?? throw new AssertFailedException("Task 1 was not found.");
        Assert.AreEqual("Updated task", updatedTask.Description);
        Assert.AreEqual(PriorityLevel.High, updatedTask.Priority);
    }

    [TestMethod]
    public void UpdateTask_AllowsAssignedUserWithoutCaseSensitiveMatch_Test()
    {
        var service = CreateService(CreateTask(1, "Alice"));

        service.UpdateTask(1, "Updated by assignee", "Alice", " alice ");

        var updatedTask = FindTask(service, 1) ?? throw new AssertFailedException("Task 1 was not found.");
        Assert.AreEqual("Updated by assignee", updatedTask.Description);
    }

    [TestMethod]
    public void UpdateDependencies_RejectsCycles_Test()
    {
        var service = CreateService(
            CreateTask(1, "Alice"),
            CreateTask(2, "Alice", 1));
        var dependencies = new MyLinkedList<int>();
        dependencies.Add(2);

        Assert.Throws<InvalidOperationException>(() =>
            service.UpdateDependencies(1, dependencies, "Alice"));
        var unchangedTask = FindTask(service, 1) ?? throw new AssertFailedException("Task 1 was not found.");
        Assert.AreEqual(0, unchangedTask.Dependencies.Count);
    }

    [TestMethod]
    public void JsonTaskRepository_RoundTripsDependencies_Test()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"project1-{Guid.NewGuid():N}.json");
        try
        {
            var dependencies = new MyLinkedList<int>();
            dependencies.Add(4);
            dependencies.Add(5);
            var tasks = new MyArray<TaskItem>();
            tasks.Add(new TaskItem
            {
                Id = 2,
                Description = "Prepare release",
                Assignee = "Alice",
                Dependencies = dependencies
            });
            var repository = new JsonTaskRepository(filePath);

            repository.SaveTasks(tasks);
            var restored = repository.LoadTasks(CollectionType.Array)
                .FindBy(2, (task, id) => task.Id.CompareTo(id))
                ?? throw new AssertFailedException("The saved task was not restored.");

            CollectionAssert.AreEquivalent(new[] { 4, 5 }, restored.Dependencies.ToArray());
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    private static TaskService CreateService(params TaskItem[] tasks) =>
        new(new InMemoryTaskRepository(tasks), CollectionType.Array);

    private static TaskItem CreateTask(int id, string assignee, params int[] dependencies)
    {
        var dependencyList = new MyLinkedList<int>();
        foreach (var dependency in dependencies) dependencyList.Add(dependency);

        return new TaskItem
        {
            Id = id,
            Description = $"Task {id}",
            Assignee = assignee,
            Dependencies = dependencyList
        };
    }

    private static TaskItem? FindTask(ITaskService service, int id) =>
        service.GetAllTasks().FindBy(id, (task, taskId) => task.Id.CompareTo(taskId));

    private sealed class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks;

        public InMemoryTaskRepository(IEnumerable<TaskItem> tasks) => _tasks = tasks.ToList();

        public IMyCollection<TaskItem> LoadTasks(CollectionType type)
        {
            var result = CollectionFactory.Create<TaskItem>(type);
            foreach (var task in _tasks) result.Add(task);
            return result;
        }

        public void SaveTasks(IMyCollection<TaskItem> tasks)
        {
            _tasks.Clear();
            foreach (var task in tasks) _tasks.Add(task);
        }
    }
}
