# Project1

Gemaakt door Ernesto:
    Structures:
        Array:
            Remove
            FindBy
            Reduce
            GetIterator
            GetEnumerator

        Linked List:
            Reduce
            Sort
            GetIterator
            GetEnumerator
    
    Features:
        To-Do List (Base Functionality):
            Filter tasks

Gemaakt door Haris:
    Structures:
        Array:
            Add
            Filter
            Sort

        Linked List:
            Add
            Remove
            FindBy
            Filter
            Reduce

        Dictionary (Hash map)
    
    Features:
        To-Do List (Base Functionality):
            Toggle task state

        Kanban View (Enhancement):
            Tasks grouped in columns
            Toggle task (move across states)

        Task Assignment (Challenge):
            Assign task to users
            Enforce modification rights
        
        Task Dependencies (Challenge):
            Tasks can have dependencies

## Web frontend

The browser app is a React + TypeScript frontend backed by a separate ASP.NET Core API. The API continues to use the task service and stores tasks, including dependency IDs, in the root `tasks.json` file.

Requirements: .NET 8 SDK and Node.js 20 or newer.

From PowerShell, install dependencies once, then start the frontend and API together:

```powershell
cd frontend
npm.cmd install
npm.cmd run dev
```

Open the Vite URL shown in the terminal (normally `http://localhost:5173`). The command starts Vite on port 5173 and the API on port 5000. If an API is already running on port 5000, the frontend can use it and the new API process will report that the port is occupied. The app supports task creation, editing, deletion, search, list/board views, priorities, assignments, and dependencies. The current-user field is a demo identity selector, not authentication; the API currently enforces task ownership rules but is not intended for deployment as a secured multi-user service.

Run the frontend checks with `npm.cmd test` and `npm.cmd run build` in PowerShell. Run the .NET tests from the repository root with `dotnet test`.
