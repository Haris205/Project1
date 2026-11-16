import type { Priority, Task, TaskInput, TaskStatus } from "./types";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5000/api";

async function request<T>(
  path: string,
  options: RequestInit = {},
  currentUser?: string,
): Promise<T> {
  const headers = new Headers(options.headers);
  if (options.body) headers.set("Content-Type", "application/json");
  if (currentUser) headers.set("X-Current-User", currentUser);

  const response = await fetch(`${API_URL}${path}`, { ...options, headers });
  if (!response.ok) {
    const body = await response.json().catch(() => null) as { title?: string; detail?: string } | null;
    throw new Error(body?.detail ?? body?.title ?? `Request failed (${response.status})`);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export const taskApi = {
  getTasks: () => request<Task[]>("/tasks"),
  create: (input: TaskInput, user: string) =>
    request<Task>("/tasks", { method: "POST", body: JSON.stringify(input) }, user),
  update: (id: number, input: TaskInput, user: string) =>
    request<Task>(`/tasks/${id}`, { method: "PUT", body: JSON.stringify(input) }, user),
  remove: (id: number, user: string) =>
    request<void>(`/tasks/${id}`, { method: "DELETE" }, user),
  setStatus: (id: number, status: TaskStatus, user: string) =>
    request<Task>(`/tasks/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify({ status }),
    }, user),
};

export const priorities: Priority[] = ["Low", "Medium", "High"];
