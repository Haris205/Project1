export type TaskStatus = "ToDo" | "InProgress" | "Done";
export type Priority = "Low" | "Medium" | "High";
export type ViewMode = "board" | "list";

export interface Task {
  id: number;
  description: string;
  priority: Priority;
  createdAt: string;
  assignee: string;
  status: TaskStatus;
  dependencies: number[];
}

export interface TaskInput {
  description: string;
  priority: Priority;
  assignee: string;
  dependencies: number[];
}

export const STATUSES: TaskStatus[] = ["ToDo", "InProgress", "Done"];
export const STATUS_LABELS: Record<TaskStatus, string> = {
  ToDo: "To do",
  InProgress: "In progress",
  Done: "Done",
};
