using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Data.Repositories
{
    public class MockRoutineRepository : IRoutineRepository
    {
        private readonly List<Routine> _routines = new();

        public MockRoutineRepository()
        {
            // Add a default routine for demonstration
            var dummyRoutine = new Routine("Morning Launch", "icon.png", Core.Enums.TriggerType.Time, "{\"Time\":\"09:00\"}");
            dummyRoutine.AddAction(new ActionEntry(Core.Enums.ActionType.LaunchApp, "notepad.exe", 1));
            _routines.Add(dummyRoutine);
        }
      
        public Task AddAsync(Routine routine)
        {
            _routines.Add(routine);
            return Task.CompletedTask;
        }
       
        public Task DeleteAsync(Guid id)
        {
            var routine = _routines.FirstOrDefault(r => r.Id == id);
            if (routine != null)
            {
                routine.SoftDelete();
            }
            return Task.CompletedTask;
        }

        public Task<List<Routine>> GetAllAsync()
        {
            return Task.FromResult(_routines.Where(r => !r.IsDeleted).ToList());
        }

        public Task<Routine?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_routines.FirstOrDefault(r => r.Id == id && !r.IsDeleted));
        }

        public Task UpdateAsync(Routine routine)
        {
            var existing = _routines.FirstOrDefault(r => r.Id == routine.Id);
            if (existing != null)
            {
                existing.UpdateDetails(routine.Name, routine.IconPath);
                // In a real DB we'd update other properties too
            }
            return Task.CompletedTask;
        }
    }
}
