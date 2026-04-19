# SmartRoutines

SmartRoutines is a Windows desktop automation app that lets users build routines made of:
- a **trigger** (when to run), and
- one or more **actions** (what to run, in sequence).

The goal is to make repetitive tasks easy to automate from a simple WinForms interface, while keeping the codebase modular and maintainable.

## Project Idea

The application helps users define daily automations such as launching apps, running task flows, and tracking execution history.

Core concepts:
- **Routine**: The automation definition users create and manage.
- **Trigger**: The condition/time/event that starts a routine.
- **ActionEntry**: A single executable step inside a routine.
- **Automation Engine**: Monitors triggers and executes actions.
- **Activity Log**: Stores routine execution status and messages.

## Architecture Overview

The solution follows an N-Tier style architecture to separate domain, data, business logic, and UI concerns:

- **`SmartRoutines.Core`**: Domain entities, DTOs, enums, exceptions, interfaces, and events.
- **`SmartRoutines.Data`**: EF Core DbContext, configurations, migrations, repositories, and unit of work.
- **`SmartRoutines.Logic`**: Services, action executors, trigger monitors, and automation engine orchestration.
- **`SmartRoutines.UI`**: WinForms entry point, forms, and user controls.
- **`Test`**: Console-style playground/integration harness for testing flows.

## File Structure

```text
SmartRoutines/
|-- README.md
|-- code_visualization.md
|-- TriggerConfiguration.cs
|-- SmartRoutines.Core/
|   |-- Domain/
|   |   |-- Entities/
|   |   |-- Enums/
|   |   `-- Models/
|   |-- DTOs/
|   |-- Events/
|   |-- Exceptions/
|   `-- Interfaces/
|       |-- Data/
|       `-- Logic/
|-- SmartRoutines.Data/
|   |-- Context/
|   |   |-- SmartRoutinesDbContext.cs
|   |   `-- SmartRoutinesDbContextFactory.cs
|   |-- Configurations/
|   |-- Migrations/
|   |-- Repositories/
|   `-- UnitOfWork/
|-- SmartRoutines.Logic/
|   |-- ActionExecutors/
|   |-- AutomationEngine/
|   |-- Services/
|   |-- TriggerMonitors/
|   `-- Utilities/
|-- SmartRoutines.UI/
|   |-- Program.cs
|   |-- appsettings.json
|   |-- Forms/
|   `-- Controls/
`-- Test/
	|-- Program.cs
	`-- appsettings.json
```

## Important Files

- **`SmartRoutines.UI/Program.cs`**: App startup, dependency injection, and main form bootstrapping.
- **`SmartRoutines.Data/Context/SmartRoutinesDbContext.cs`**: EF Core context and model configuration.
- **`SmartRoutines.Data/Context/SmartRoutinesDbContextFactory.cs`**: Design-time context factory for migrations.
- **`SmartRoutines.Logic/AutomationEngine/`**: Runtime orchestration of trigger monitoring and action execution.

## Notes

- Connection string is read from **`SmartRoutines.UI/appsettings.json`** (`DefaultConnection`).
- EF Core migrations are under **`SmartRoutines.Data/Migrations/`**.
- See **`code_visualization.md`** for a class-level diagram.
