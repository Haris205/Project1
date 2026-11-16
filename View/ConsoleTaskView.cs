public class ConsoleTaskView : ITaskView
{ 
  private readonly ITaskService _service; 
 
  public ConsoleTaskView(ITaskService service) { 
    _service = service; 
  } 
  void Display(IMyCollection<TaskItem> tasks)
  {
      Console.Clear();
      Console.WriteLine("==== Tasks ====\n");

      var todo = new MyLinkedList<TaskItem>();
      var inProgress = new MyLinkedList<TaskItem>();
      var done = new MyLinkedList<TaskItem>();

      var iterator = tasks.GetIterator();
      while (iterator.HasNext())
      {
          var task = iterator.Next();

          switch (task.Status)
          {
              case Status.ToDo:
                  todo.Add(task);
                  break;
              case Status.InProgress:
                  inProgress.Add(task);
                  break;
              case Status.Done:
                  done.Add(task);
                  break;
          }
      }

      Console.WriteLine($"{"TO DO",-30} | {"IN PROGRESS",-30} | {"DONE",-30}");
      Console.WriteLine(new string('-', 100));

      var todoIt = todo.GetIterator();
      var inProgressIt = inProgress.GetIterator();
      var doneIt = done.GetIterator();

      while (todoIt.HasNext() || inProgressIt.HasNext() || doneIt.HasNext())
      {
          string todoText = todoIt.HasNext() ? FormatTask(todoIt.Next()) : "";
          string inProgressText = inProgressIt.HasNext() ? FormatTask(inProgressIt.Next()) : "";
          string doneText = doneIt.HasNext() ? FormatTask(doneIt.Next()) : "";
          Console.WriteLine($"{todoText,-30} | {inProgressText,-30} | {doneText,-30}");
      }
  }
  
  string FormatTask(TaskItem task)
  {
      return $"{task.Id}: {task.Assignee} (deps: {FormatDeps(task)})";
  }
  void FilterDisplayTasks(IMyCollection<TaskItem> tasks) { 
    Console.Clear(); 
    Console.WriteLine("1. Priority\n2. Status\n3. Created at");
    string question = Console.ReadLine()!;
    Console.WriteLine("==== Filtered ToDo List ====");

    IMyCollection<TaskItem> filtered;

    if(question == "1")
    {
      filtered = _service.FilterTasks(tasks, _ => true);
      filtered.Sort((a, b) => b.Priority.CompareTo(a.Priority));
      var iterator = filtered.GetIterator();
      while (iterator.HasNext())
      {
        var task = iterator.Next();
        Console.WriteLine($"TaskItem: {task.Id} | Priority: {task.Priority}");
      }
    }
    else if(question == "2")
    {
      string input = Prompt("Enter status (1. ToDo, 2. InProgress, 3. Done):");

      Status selected = input switch
      {
          "1" => Status.ToDo,
          "2" => Status.InProgress,
          "3" => Status.Done,
          _ => Status.ToDo
      };

      filtered = _service.FilterTasks(tasks, t => t.Status == selected);

      var iterator = filtered.GetIterator();
      while (iterator.HasNext())
      {
          var task = iterator.Next();
          Console.WriteLine($"TaskItem: {task.Id} | Status: {task.Status}");
      }
    }
    else if(question == "3")
    {
      filtered = _service.FilterTasks(tasks, _ => true);
      filtered.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
      var iterator = filtered.GetIterator();
      while (iterator.HasNext())
      {
        var task = iterator.Next();
        Console.WriteLine($"TaskItem: {task.Id} | Created at: {task.CreatedAt:yyyy-MM-dd HH:mm}");
      }    
    }
    else { 
      Console.WriteLine("Invalid option. Press any key to go back to the menu");
      Console.ReadKey();
      return; 
    }
    Console.WriteLine("\nPress any key to go back to the menu");
    Console.ReadKey();
  }
  string Prompt(string prompt) { 
    Console.Write(prompt); 
    return Console.ReadLine()!.Trim(); 
  }
  string FormatDeps(TaskItem task)
  {
      if (task.Dependencies == null)
          return "none";

      var iterator = task.Dependencies.GetIterator();
      string result = "";

      while (iterator.HasNext())
      {
          result += iterator.Next();

          if (iterator.HasNext())
              result += ", ";
      }

      return result == "" ? "none" : result;
  }
  void InvalidModifyRights()
  {
    Console.WriteLine("Invalid modify rights!");
    Console.WriteLine("Press any key to return");
    Console.ReadKey();
    return;
  }
  bool ContainsDigit(string input)
  {
    foreach (char c in input)
    {
      if (char.IsDigit(c))
      {
        return true;
      }
    }
    return false;
  }
  public void Run() {
    string user = Prompt("Enter user name: ");

    while (string.IsNullOrEmpty(user) || ContainsDigit(user))
    {
      user = Prompt("Enter a valid user name (No digits allowed): ");
    }

    Console.WriteLine("Choose a data structure:");
    Console.WriteLine("1. Array\n2. LinkedList\n3. Dictionary");

    CollectionType chosenType = Console.ReadLine() switch
    {
      "1" => CollectionType.Array,
      "2" => CollectionType.LinkedList,
      "3" => CollectionType.Dictionary,
      _ => CollectionType.Array
    };

    _service.SetCollectionType(chosenType);

    while (true) { 
      var tasks = _service.GetAllTasks();
      Display(tasks);
      Console.WriteLine($"\nCurrent user: {user}");
      Console.WriteLine("Options:"); 
      Console.WriteLine("1. Add Task");
      Console.WriteLine("2. Update Task"); 
      Console.WriteLine("3. Remove Task"); 
      Console.WriteLine("4. Toggle Task State");
      Console.WriteLine("5. Filter Task");
      Console.WriteLine("6. Change User");
      Console.WriteLine("7. Change Dependencies"); 
      Console.WriteLine("8. Exit"); 
 
      string option = Prompt("\nSelect an option: ");
      switch (option)
      {
        case "1":
              string addDescription = Prompt("\nEnter task description: ");
              string priorityInput = Prompt("\nEnter task priority:\n1.Low\n2.Medium\n3.High\n");
              PriorityLevel priority;
              switch (priorityInput)
              {
                  case "1":
                      priority = PriorityLevel.Low;
                      break;
                  case "2":
                      priority = PriorityLevel.Medium;
                      break;
                  case "3":
                      priority = PriorityLevel.High;
                      break;
                  default:
                      Console.WriteLine("Invalid priority, defaulting priority to medium");
                      priority = PriorityLevel.Medium;
                      break;
              }
              string assignee = Prompt("Enter assignee name (Keep empty for none): ");

              MyLinkedList<int> dependencies = new MyLinkedList<int>();

              string depInput = Prompt("Enter dependencies (comma separated task IDs, empty for none): ");

              if (!string.IsNullOrWhiteSpace(depInput))
              {
                  var parts = depInput.Split(',');

                  foreach (var part in parts)
                  {
                      if (int.TryParse(part.Trim(), out int depId))
                      {
                          dependencies.Add(depId);
                      }
                  }
              }
              _service.AddTask(addDescription, priority, string.IsNullOrWhiteSpace(assignee) ? "none" : assignee, dependencies);
              break;
        case "2":
          string updateIdStr = Prompt ("\nEnter task id to update: ");
          if (int.TryParse(updateIdStr, out int updateId))
          {
            if (_service.CanUserModify(updateId, user))
            {
              if (!_service.TaskExists(updateId))
              {
                Console.WriteLine("Task not found\nPress any key to return");
                Console.ReadKey();
              }
              string updateDescription = Prompt ("Enter new task description: ");
              string updateAssignee = Prompt ("Enter assignee name (Keep empty for none): ");
              _service.UpdateTask(updateId, updateDescription, updateAssignee, user);
              break;
            }
            InvalidModifyRights();
          }
          break;
        case "3":
        string removeIdStr = Prompt ("Enter task id to remove: ");
         if (int.TryParse(removeIdStr, out int removeId)) {
            if (_service.CanUserModify(removeId, user))
            {
              _service.RemoveTask(removeId, user); 
              break;
            }
            InvalidModifyRights();
          } 
          break;
        case "4":
        string toggleIdStr = Prompt ("Enter task id to toggle: "); 
          if (int.TryParse(toggleIdStr, out int toggleId)) {
            if (!_service.CanUserModify(toggleId, user))
            {
              InvalidModifyRights();
              break;
            }
            if (!_service.AreDependenciesDone(toggleId))
            {
              Console.WriteLine($"\nTask {toggleId} is locked. Complete the dependent task first.");
              Console.WriteLine("\nPress any key to return");
              Console.ReadKey();
              break;
            }
            string statusInput = Prompt("\nEnter task status:\n1.To do\n2.In progress\n3.Done\n");
              Status status;
              switch (statusInput)
              {
                  case "1":
                      status = Status.ToDo;
                      break;
                  case "2":
                      status = Status.InProgress;
                      break;
                  case "3":
                      status = Status.Done;
                      break;
                  default:
                      Console.WriteLine("Invalid priority, defaulting priority to medium");
                      status = Status.ToDo;
                      break;
              }
            _service.ToggleTaskState(toggleId, user, status);
            break;
          } 
          break;
        case "5":
          Console.Clear();
          FilterDisplayTasks(tasks);
          break;
        case "6":
          Console.Clear();
          string newUser = Prompt("Enter user name: ");

          while (string.IsNullOrEmpty(newUser) || ContainsDigit(newUser))
          {
            newUser = Prompt("Enter a valid user name (No digits allowed): ");
          }
          user = newUser;
          break;
        case "7":
          string taskStr = Prompt ("\nEnter task id to update dependencies: ");
          if (int.TryParse(taskStr, out int taskId))
          {
            if (!_service.TaskExists(taskId))
            {
              Console.WriteLine("Task not found\nPress any key to return");
              Console.ReadKey();
            }
            if (_service.CanUserModify(taskId, user))
            {
              string depsInput = Prompt("Enter dependencies (comma separated task IDs, empty for none): ");

              var deps = new MyLinkedList<int>();

              if (!string.IsNullOrWhiteSpace(depsInput))
              {
                  var parts = depsInput.Split(',');

                  foreach (var p in parts)
                  {
                      if (int.TryParse(p.Trim(), out int depId))
                      {
                        if (!_service.TaskExists(depId))
                        {
                          Console.WriteLine($"Task {depId} not found.");
                          continue;
                        }
                        deps.Add(depId);
                      }
                  }
              }
              _service.UpdateDependencies(taskId, deps, user);
              Console.WriteLine("Dependencies updated.");
              Console.WriteLine("\nPress any key to continue.");
              Console.ReadKey();
              break;
            }
            InvalidModifyRights();
          }
          break;
        case "8":
          return; 
        default: 
          Console.WriteLine("Invalid option. Press any key to continue."); 
          Console.ReadKey(); 
          break; 
      } 
    } 
  } 
} 