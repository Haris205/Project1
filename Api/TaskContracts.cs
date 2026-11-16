public sealed record TaskResponse(
  int Id,
  string Description,
  PriorityLevel Priority,
  DateTime CreatedAt,
  string Assignee,
  Status Status,
  int[] Dependencies);

public sealed record CreateTaskRequest(
  string Description,
  PriorityLevel Priority = PriorityLevel.Medium,
  string? Assignee = null,
  int[]? Dependencies = null);

public sealed record UpdateTaskRequest(
  string Description,
  string? Assignee,
  PriorityLevel? Priority = null,
  int[]? Dependencies = null);

public sealed record UpdateStatusRequest(Status Status);

public sealed record UpdateDependenciesRequest(int[] DependencyIds);
