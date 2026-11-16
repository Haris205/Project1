using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var taskFile = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "tasks.json"));
builder.Services.AddSingleton<ITaskService>(_ =>
  new TaskService(new JsonTaskRepository(taskFile), CollectionType.Array));
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
  policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.MapTaskEndpoints();
app.Run();
