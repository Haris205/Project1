import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { priorities, taskApi } from "./api";
import { STATUSES, STATUS_LABELS } from "./types";
import type { Priority, Task, TaskInput, TaskStatus, ViewMode } from "./types";

function App() {
  const queryClient = useQueryClient();
  const [currentUser, setCurrentUser] = useState(() => localStorage.getItem("project1-user") || "Alex");
  const [view, setView] = useState<ViewMode>("board");
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<TaskStatus | "All">("All");
  const [priorityFilter, setPriorityFilter] = useState<Priority | "All">("All");
  const [editing, setEditing] = useState<Task | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const tasksQuery = useQuery({ queryKey: ["tasks"], queryFn: taskApi.getTasks });
  const tasks = tasksQuery.data ?? [];
  const refreshTasks = () => queryClient.invalidateQueries({ queryKey: ["tasks"] });
  const saveTask = useMutation({
    mutationFn: ({ input, task }: { input: TaskInput; task: Task | null }) =>
      task ? taskApi.update(task.id, input, currentUser) : taskApi.create(input, currentUser),
    onSuccess: () => {
      setShowForm(false);
      setEditing(null);
      setErrorMessage("");
      void refreshTasks();
    },
    onError: (error: Error) => setErrorMessage(error.message),
  });
  const deleteTask = useMutation({
    mutationFn: (id: number) => taskApi.remove(id, currentUser),
    onSuccess: () => {
      setErrorMessage("");
      void refreshTasks();
    },
    onError: (error: Error) => setErrorMessage(error.message),
  });
  const changeStatus = useMutation({
    mutationFn: ({ id, status }: { id: number; status: TaskStatus }) =>
      taskApi.setStatus(id, status, currentUser),
    onSuccess: () => {
      setErrorMessage("");
      void refreshTasks();
    },
    onError: (error: Error) => setErrorMessage(error.message),
  });

  const filteredTasks = useMemo(() => {
    const query = search.trim().toLowerCase();
    return tasks.filter((task) => {
      const matchesSearch = !query
        || task.description.toLowerCase().includes(query)
        || task.assignee.toLowerCase().includes(query);
      return matchesSearch
        && (statusFilter === "All" || task.status === statusFilter)
        && (priorityFilter === "All" || task.priority === priorityFilter);
    });
  }, [tasks, search, statusFilter, priorityFilter]);

  function updateCurrentUser(value: string) {
    setCurrentUser(value);
    localStorage.setItem("project1-user", value);
  }

  function openCreateForm() {
    setEditing(null);
    setErrorMessage("");
    setShowForm(true);
  }

  function openEditForm(task: Task) {
    setEditing(task);
    setErrorMessage("");
    setShowForm(true);
  }

  function submitTask(input: TaskInput) {
    saveTask.mutate({ input, task: editing });
  }

  function removeTask(task: Task) {
    if (window.confirm(`Delete "${task.description}"?`)) deleteTask.mutate(task.id);
  }

  if (tasksQuery.isPending) {
    return <main className="loading-screen"><span className="spinner" /> Loading your workspace…</main>;
  }

  if (tasksQuery.isError) {
    return (
      <main className="loading-screen">
        <div className="error-card">
          <strong>Couldn’t load your tasks</strong>
          <p>{tasksQuery.error.message}</p>
          <button className="button button-primary" onClick={() => void tasksQuery.refetch()}>Try again</button>
        </div>
      </main>
    );
  }

  return (
    <div className="app-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="Taskflow home">
          <span className="brand-mark">T</span>
          <span>taskflow<span className="brand-dot">.</span></span>
        </a>
        <div className="topbar-right">
          <label className="user-control">
            <span className="avatar">{currentUser.trim().charAt(0).toUpperCase() || "?"}</span>
            <span className="user-label">Demo user</span>
            <input
              aria-label="Current user"
              value={currentUser}
              onChange={(event) => updateCurrentUser(event.target.value)}
              maxLength={40}
            />
          </label>
        </div>
      </header>

      <main className="workspace">
        <section className="welcome-row">
          <div>
            <p className="eyebrow">YOUR WORKSPACE</p>
            <h1>Good work starts here.</h1>
            <p className="subtitle">Keep your team aligned and your next step clear.</p>
          </div>
          <button className="button button-primary new-task-button" onClick={openCreateForm}>
            <span aria-hidden="true">＋</span> New task
          </button>
        </section>

        <section className="stats-grid" aria-label="Task summary">
          <StatCard label="Total tasks" value={tasks.length} icon="▤" tone="blue" />
          <StatCard label="To do" value={tasks.filter((task) => task.status === "ToDo").length} icon="◷" tone="amber" />
          <StatCard label="In progress" value={tasks.filter((task) => task.status === "InProgress").length} icon="↗" tone="violet" />
          <StatCard label="Completed" value={tasks.filter((task) => task.status === "Done").length} icon="✓" tone="green" />
        </section>

        <section className="task-section">
          <div className="section-heading">
            <div>
              <h2>Tasks</h2>
              <p>Plan, prioritize, and keep things moving.</p>
            </div>
            <div className="view-switch" role="group" aria-label="Task view">
              <button className={view === "board" ? "selected" : ""} onClick={() => setView("board")}>▦ <span>Board</span></button>
              <button className={view === "list" ? "selected" : ""} onClick={() => setView("list")}>☷ <span>List</span></button>
            </div>
          </div>

          <div className="filter-bar">
            <label className="search-field">
              <span aria-hidden="true">⌕</span>
              <input
                aria-label="Search tasks"
                placeholder="Search tasks or assignees…"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
              />
            </label>
            {view === "list" && (
              <>
                <select aria-label="Filter by status" value={statusFilter} onChange={(event) => setStatusFilter(event.target.value as TaskStatus | "All")}>
                  <option value="All">All statuses</option>
                  {STATUSES.map((status) => <option key={status} value={status}>{STATUS_LABELS[status]}</option>)}
                </select>
                <select aria-label="Filter by priority" value={priorityFilter} onChange={(event) => setPriorityFilter(event.target.value as Priority | "All")}>
                  <option value="All">All priorities</option>
                  {priorities.map((priority) => <option key={priority} value={priority}>{priority} priority</option>)}
                </select>
              </>
            )}
            <span className="result-count">{filteredTasks.length} {filteredTasks.length === 1 ? "task" : "tasks"}</span>
          </div>

          {errorMessage && (
            <div className="inline-error" role="alert">
              <span>{errorMessage}</span>
              <button aria-label="Dismiss error" onClick={() => setErrorMessage("")}>×</button>
            </div>
          )}

          {view === "board" ? (
            <div className="board-grid">
              {STATUSES.map((status) => (
                <section className={`board-column column-${status.toLowerCase()}`} key={status} aria-label={`${STATUS_LABELS[status]} tasks`}>
                  <div className="column-heading">
                    <span className={`status-dot dot-${status.toLowerCase()}`} />
                    <h3>{STATUS_LABELS[status]}</h3>
                    <span className="column-count">{filteredTasks.filter((task) => task.status === status).length}</span>
                  </div>
                  <div className="column-cards">
                    {filteredTasks.filter((task) => task.status === status).map((task) => (
                      <TaskCard
                        key={task.id}
                        task={task}
                        tasks={tasks}
                        currentUser={currentUser}
                        onEdit={openEditForm}
                        onDelete={removeTask}
                        onStatusChange={(nextStatus) => changeStatus.mutate({ id: task.id, status: nextStatus })}
                        busy={changeStatus.isPending || deleteTask.isPending}
                      />
                    ))}
                    {filteredTasks.every((task) => task.status !== status) && <p className="column-empty">Nothing here yet</p>}
                  </div>
                </section>
              ))}
            </div>
          ) : (
            <div className="list-panel">
              {filteredTasks.length === 0 ? (
                <EmptyState onCreate={openCreateForm} hasTasks={tasks.length > 0} />
              ) : filteredTasks.map((task) => (
                <TaskCard
                  key={task.id}
                  task={task}
                  tasks={tasks}
                  currentUser={currentUser}
                  onEdit={openEditForm}
                  onDelete={removeTask}
                  onStatusChange={(nextStatus) => changeStatus.mutate({ id: task.id, status: nextStatus })}
                  busy={changeStatus.isPending || deleteTask.isPending}
                />
              ))}
            </div>
          )}
        </section>
      </main>

      {showForm && (
        <TaskDialog
          task={editing}
          tasks={tasks}
          busy={saveTask.isPending}
          error={saveTask.error?.message ?? ""}
          onClose={() => {
            setShowForm(false);
            setEditing(null);
          }}
          onSubmit={submitTask}
        />
      )}
    </div>
  );
}

function StatCard({ label, value, icon, tone }: { label: string; value: number; icon: string; tone: string }) {
  return (
    <article className="stat-card">
      <span className={`stat-icon ${tone}`}>{icon}</span>
      <div><p>{label}</p><strong>{value}</strong></div>
    </article>
  );
}

function TaskCard({
  task,
  tasks,
  currentUser,
  onEdit,
  onDelete,
  onStatusChange,
  busy,
}: {
  task: Task;
  tasks: Task[];
  currentUser: string;
  onEdit: (task: Task) => void;
  onDelete: (task: Task) => void;
  onStatusChange: (status: TaskStatus) => void;
  busy: boolean;
}) {
  const assignee = task.assignee.trim().toLowerCase();
  const canEdit = assignee === "none" || assignee === currentUser.trim().toLowerCase();
  const dependencyNames = task.dependencies
    .map((id) => tasks.find((item) => item.id === id)?.description ?? `Task #${id}`);

  return (
    <article className={`task-card ${!canEdit ? "task-card-locked" : ""}`}>
      <div className="task-card-top">
        <span className={`priority-badge priority-${task.priority.toLowerCase()}`}>
          <span /> {task.priority}
        </span>
        <div className="card-actions">
          {!canEdit && <span className="lock-label" title={`Assigned to ${task.assignee}`}>🔒</span>}
          <button aria-label={`Edit ${task.description}`} title="Edit task" disabled={!canEdit} onClick={() => onEdit(task)}>✎</button>
          <button aria-label={`Delete ${task.description}`} title="Delete task" disabled={!canEdit || busy} onClick={() => onDelete(task)}>×</button>
        </div>
      </div>
      <h4>{task.description}</h4>
      {dependencyNames.length > 0 && (
        <div className="dependencies">
          <span aria-hidden="true">↳</span> Depends on {dependencyNames.join(", ")}
        </div>
      )}
      <div className="task-card-footer">
        <span className="assignee">
          <span className="assignee-avatar">{task.assignee.toLowerCase() === "none" ? "—" : task.assignee.charAt(0).toUpperCase()}</span>
          {task.assignee.toLowerCase() === "none" ? "Unassigned" : task.assignee}
        </span>
        <select
          aria-label={`Status for ${task.description}`}
          value={task.status}
          disabled={!canEdit || busy}
          onChange={(event) => onStatusChange(event.target.value as TaskStatus)}
        >
          {STATUSES.map((status) => <option key={status} value={status}>{STATUS_LABELS[status]}</option>)}
        </select>
      </div>
    </article>
  );
}

function TaskDialog({
  task,
  tasks,
  busy,
  error,
  onClose,
  onSubmit,
}: {
  task: Task | null;
  tasks: Task[];
  busy: boolean;
  error: string;
  onClose: () => void;
  onSubmit: (input: TaskInput) => void;
}) {
  const [description, setDescription] = useState(task?.description ?? "");
  const [priority, setPriority] = useState<Priority>(task?.priority ?? "Medium");
  const [assignee, setAssignee] = useState(task?.assignee === "none" ? "" : task?.assignee ?? "");
  const [dependencies, setDependencies] = useState<number[]>(task?.dependencies ?? []);

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    onSubmit({ description: description.trim(), priority, assignee: assignee.trim(), dependencies });
  }

  function toggleDependency(id: number) {
    setDependencies((current) => current.includes(id)
      ? current.filter((dependencyId) => dependencyId !== id)
      : [...current, id]);
  }

  return (
    <div className="dialog-backdrop" onMouseDown={(event) => {
      if (event.target === event.currentTarget && !busy) onClose();
    }}>
      <section className="task-dialog" role="dialog" aria-modal="true" aria-labelledby="dialog-title">
        <div className="dialog-heading">
          <div>
            <p className="eyebrow">{task ? "TASK DETAILS" : "NEW TASK"}</p>
            <h2 id="dialog-title">{task ? "Update task" : "Create a task"}</h2>
          </div>
          <button className="dialog-close" aria-label="Close dialog" disabled={busy} onClick={onClose}>×</button>
        </div>
        <form onSubmit={submit}>
          {error && <div className="dialog-error" role="alert">{error}</div>}
          <label className="form-field">
            <span>Task name</span>
            <input autoFocus required maxLength={160} value={description} onChange={(event) => setDescription(event.target.value)} placeholder="e.g. Prepare project kickoff" />
          </label>
          <div className="form-row">
            <label className="form-field">
              <span>Priority</span>
              <select value={priority} onChange={(event) => setPriority(event.target.value as Priority)}>
                {priorities.map((item) => <option key={item} value={item}>{item}</option>)}
              </select>
            </label>
            <label className="form-field">
              <span>Assignee</span>
              <input maxLength={40} value={assignee} onChange={(event) => setAssignee(event.target.value)} placeholder="Leave empty for unassigned" />
            </label>
          </div>
          {tasks.filter((item) => item.id !== task?.id).length > 0 && (
            <fieldset className="dependency-picker">
              <legend>Dependencies <span>Optional</span></legend>
              <p>This task can’t be completed until these are done.</p>
              <div className="dependency-options">
                {tasks.filter((item) => item.id !== task?.id).map((item) => (
                  <label key={item.id}>
                    <input type="checkbox" checked={dependencies.includes(item.id)} onChange={() => toggleDependency(item.id)} />
                    <span>{item.description}</span>
                    <small>#{item.id}</small>
                  </label>
                ))}
              </div>
            </fieldset>
          )}
          <div className="dialog-actions">
            <button type="button" className="button button-subtle" disabled={busy} onClick={onClose}>Cancel</button>
            <button className="button button-primary" disabled={busy || !description.trim()}>
              {busy ? "Saving…" : task ? "Save changes" : "Create task"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}

function EmptyState({ onCreate, hasTasks }: { onCreate: () => void; hasTasks: boolean }) {
  return (
    <div className="empty-state">
      <span className="empty-illustration">✓</span>
      <h3>{hasTasks ? "No tasks match those filters" : "Your task list is a blank canvas"}</h3>
      <p>{hasTasks ? "Try changing the search or filters." : "Create your first task and make progress visible."}</p>
      {!hasTasks && <button className="button button-primary" onClick={onCreate}>Create your first task</button>}
    </div>
  );
}

export default App;
