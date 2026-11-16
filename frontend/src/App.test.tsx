import { afterEach, describe, expect, it, vi } from "vitest";
import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import App from "./App";
import type { Task } from "./types";

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  localStorage.clear();
});

function renderApp() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>,
  );
}

describe("task workspace", () => {
  it("creates a task and sends the selected demo user to the API", async () => {
    const tasks: Task[] = [];
    const fetchMock = vi.fn(async (_input: RequestInfo | URL, init?: RequestInit) => {
      if (init?.method === "POST") {
        const input = JSON.parse(String(init.body)) as Omit<Task, "id" | "createdAt" | "status">;
        const task: Task = {
          ...input,
          id: 1,
          createdAt: "2026-01-01T00:00:00Z",
          status: "ToDo",
        };
        tasks.push(task);
        return Response.json(task, { status: 201 });
      }

      return Response.json(tasks);
    });
    vi.stubGlobal("fetch", fetchMock);
    const user = userEvent.setup();
    renderApp();

    await user.click(await screen.findByRole("button", { name: /new task/i }));
    await user.type(screen.getByLabelText("Task name"), "Prepare project kickoff");
    await user.click(screen.getByRole("button", { name: "Create task" }));

    expect(await screen.findByText("Prepare project kickoff")).toBeInTheDocument();
    await waitFor(() => {
      const createCall = fetchMock.mock.calls.find(([, init]) => init?.method === "POST");
      expect(createCall).toBeDefined();
      expect(new Headers(createCall?.[1]?.headers).get("X-Current-User")).toBe("Alex");
    });
  });

  it("shows a server rejection when a task's dependencies are incomplete", async () => {
    const tasks: Task[] = [{
      id: 2,
      description: "Ship the release",
      priority: "High",
      createdAt: "2026-01-01T00:00:00Z",
      assignee: "Alex",
      status: "ToDo",
      dependencies: [1],
    }];
    vi.stubGlobal("fetch", vi.fn(async (_input: RequestInfo | URL, init?: RequestInit) => {
      if (init?.method === "PATCH") {
        return Response.json({
          title: "Complete this task's dependencies before marking it done.",
          status: 409,
        }, { status: 409 });
      }
      return Response.json(tasks);
    }));
    renderApp();

    fireEvent.change(await screen.findByLabelText("Status for Ship the release"), {
      target: { value: "Done" },
    });

    expect(await screen.findByRole("alert")).toHaveTextContent("Complete this task's dependencies");
  });

  it("persists priority changes when editing a task", async () => {
    let task: Task = {
      id: 1,
      description: "Prepare report",
      priority: "Medium",
      createdAt: "2026-01-01T00:00:00Z",
      assignee: "Alex",
      status: "ToDo",
      dependencies: [],
    };
    const fetchMock = vi.fn(async (_input: RequestInfo | URL, init?: RequestInit) => {
      if (init?.method === "PUT") {
        task = { ...task, ...JSON.parse(String(init.body)) as Partial<Task> };
        return Response.json(task);
      }
      return Response.json([task]);
    });
    vi.stubGlobal("fetch", fetchMock);
    const user = userEvent.setup();
    renderApp();

    await user.click(await screen.findByRole("button", { name: "Edit Prepare report" }));
    await user.selectOptions(screen.getByLabelText("Priority"), "Low");
    await user.click(screen.getByRole("button", { name: "Save changes" }));

    expect(await screen.findByText("Low")).toBeInTheDocument();
    await waitFor(() => {
      const updateCall = fetchMock.mock.calls.find(([, init]) => init?.method === "PUT");
      expect(updateCall).toBeDefined();
      expect(JSON.parse(String(updateCall?.[1]?.body)).priority).toBe("Low");
    });
  });

  it("allows the assigned user to edit when their name differs only by capitalization", async () => {
    let task: Task = {
      id: 1,
      description: "My assigned task",
      priority: "Medium",
      createdAt: "2026-01-01T00:00:00Z",
      assignee: "Haris",
      status: "ToDo",
      dependencies: [],
    };
    const fetchMock = vi.fn(async (_input: RequestInfo | URL, init?: RequestInit) => {
      if (init?.method === "PUT") {
        task = { ...task, ...JSON.parse(String(init.body)) as Partial<Task> };
        return Response.json(task);
      }
      return Response.json([task]);
    });
    vi.stubGlobal("fetch", fetchMock);
    const user = userEvent.setup();
    renderApp();

    await user.clear(await screen.findByRole("textbox", { name: "Current user" }));
    await user.type(screen.getByRole("textbox", { name: "Current user" }), "haris");
    const editButton = screen.getByRole("button", { name: "Edit My assigned task" });
    expect(editButton).toBeEnabled();
    await user.click(editButton);
    await user.click(screen.getByRole("button", { name: "Save changes" }));

    await waitFor(() => {
      const updateCall = fetchMock.mock.calls.find(([, init]) => init?.method === "PUT");
      expect(updateCall).toBeDefined();
      expect(new Headers(updateCall?.[1]?.headers).get("X-Current-User")).toBe("haris");
    });
  });
});
