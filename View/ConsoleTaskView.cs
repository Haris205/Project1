public class ConsoleTaskView : ITaskView
{ 
  private readonly ITaskService _service; 
 
  public ConsoleTaskView(ITaskService service) { 
    _service = service; 
  } 
  void DisplayTasks(IMyCollection<TaskItem> tasks) { 
    Console.Clear(); 
    Console.WriteLine("==== ToDo List ====");
    var iterator = tasks.GetIterator();
    while (iterator.HasNext())
    {
      var task = iterator.Next();
      Console.WriteLine($"TaskItem: {task.Id} | Completed: {task.Completed}"); 
    }
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
      filtered = _service.FilterTasks(tasks, _ => true);
      filtered.Sort((a, b) => a.Completed.CompareTo(b.Completed));
      var iterator = filtered.GetIterator();
      while (iterator.HasNext())
      {
        var task = iterator.Next();
        Console.WriteLine($"TaskItem: {task.Id} | Status: {task.Completed}");
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
    return Console.ReadLine()!; 
  } 
  public void Run() {

    Console.WriteLine("Choose a data structure:");
    Console.WriteLine("1. Array\n2. LinkedList\n");

    CollectionType chosenType = Console.ReadLine() switch
    {
      "1" => CollectionType.Array,
      "2" => CollectionType.LinkedList,
      _ => CollectionType.Array
    };

    _service.SetCollectionType(chosenType);

    while (true) { 
      var tasks = _service.GetAllTasks();
      DisplayTasks(tasks);
      Console.WriteLine("\nOptions:"); 
      Console.WriteLine("1. Add Task");
      Console.WriteLine("2. Update Task"); 
      Console.WriteLine("3. Remove Task"); 
      Console.WriteLine("4. Toggle Task State");
      Console.WriteLine("5. Filter Task"); 
      Console.WriteLine("6. Exit"); 
 
      string option = Prompt("Select an option: ");
      switch (option)
      {
        case "1":
              string addDescription = Prompt("\nEnter task description:");
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
            _service.AddTask(addDescription, priority);
            break;
        case "2":
          string updateIdStr = Prompt ("Enter task id to update: ");
          if (int.TryParse(updateIdStr, out int updateId)) { 
            string updateDescription = Prompt ("Enter new task description: ");
            _service.UpdateTask(updateId, updateDescription); 
          }
          break;
        case "3":
        string removeIdStr = Prompt ("Enter task id to remove: ");
         if (int.TryParse(removeIdStr, out int removeId)) { 
            _service.RemoveTask(removeId); 
          } 
          break;
        case "4":
        string toggleIdStr = Prompt ("Enter task id to toggle: "); 
          if (int.TryParse(toggleIdStr, out int toggleId)) { 
            _service.ToggleTaskCompletion(toggleId); 
          } 
          break;
        case "5":
          Console.Clear();
          Console.WriteLine("Enter task id to filter: ");
          FilterDisplayTasks(tasks);
          break;
        case "6":
          return; 
        default: 
          Console.WriteLine("Invalid option. Press any key to continue..."); 
          Console.ReadKey(); 
          break; 
      } 
    } 
  } 
} 