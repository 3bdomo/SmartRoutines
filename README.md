# SmartRoutines

## 🏗 Project Architecture
The project follows **N-Tier Architecture** to ensure separation of concerns:
- **SmartRoutines.Core**: Contains Entities, Interfaces, and Shared Logic.
- **SmartRoutines.Data**: Data Access Layer (EF Core & Repositories).
- **SmartRoutines.Logic**: Business Logic and Services.
- **SmartRoutines.UI**: Windows Forms Presentation Layer.