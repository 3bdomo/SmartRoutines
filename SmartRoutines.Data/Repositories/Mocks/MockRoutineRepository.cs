using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Data;

namespace SmartRoutines.Data.Repositories
{
    public class MockRoutineRepository : IRoutineRepository
    {
        private readonly List<Routine> _routines = new();

        public MockRoutineRepository()
        {
            // Add a default routine for demonstration
            var dummyRoutine = new Routine("Morning Launch", "", "icon.png", TriggerType.Time, "{\"Time\":\"09:00\"}");
            dummyRoutine.AddAction(new ActionEntry(new Guid(), ActionType.LaunchApp, "notepad.exe", 1));
            _routines.Add(dummyRoutine);
        }

        public Task AddAsync(Routine routine)
        {
            _routines.Add(routine);
            return Task.CompletedTask;
        }

        public void Delete(Routine routine)
        {
            if (routine != null)
            {
                routine.SoftDelete();
            }
        }

        public Task<IEnumerable<Routine>> GetActiveNotDeletedWithActionsAsync()
        {
            var result = _routines.Where(r => !r.IsDeleted && r.IsActive);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<IReadOnlyList<Routine>> GetAllAsync()
        {
            return Task.FromResult((IReadOnlyList<Routine>)_routines.Where(r => !r.IsDeleted).ToList());
        }

        public Task<Routine?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_routines.FirstOrDefault(r => r.Id == id && !r.IsDeleted));
        }

        public Task<Routine?> GetByIdWithActionsAsync(Guid id)
        {
            var routine = _routines.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            return Task.FromResult(routine);
        }

        public Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            var exists = _routines.Any(r => !r.IsDeleted && string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase) && (excludeId == null || r.Id != excludeId.Value));
            return Task.FromResult(!exists);
        }

        public void Update(Routine routine)
        {
            var existing = _routines.FirstOrDefault(r => r.Id == routine.Id);
            if (existing != null)
            {
                existing.UpdateDetails(routine.Name, routine.Description, routine.IconPath, routine.TriggerType, routine.TriggerConfig);
            }
        }
    }
}
