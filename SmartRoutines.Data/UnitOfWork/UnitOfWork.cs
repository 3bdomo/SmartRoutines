using Microsoft.EntityFrameworkCore.Storage;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;
using SmartRoutines.Data.Repositories;

namespace SmartRoutines.Data.UnitOfWork
{
    /// <summary>
    /// Concrete Unit of Work implementation coordinating repositories and transactions.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartRoutinesDbContext _ctx;
        private IDbContextTransaction? _transaction;

        private RoutineRepository? _routines;
        private ActivityLogRepository? _activityLogs;
        private AppSettingsRepository? _appSettings;

        public UnitOfWork(SmartRoutinesDbContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public IRoutineRepository Routines => _routines ??= new RoutineRepository(_ctx);

        public IActivityLogRepository ActivityLogs => _activityLogs ??= new ActivityLogRepository(_ctx);

        public IAppSettingsRepository AppSettings => _appSettings ??= new AppSettingsRepository(_ctx);

        public async Task<int> SaveChangesAsync()
        {
            return await _ctx.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return;
            _transaction = await _ctx.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _transaction?.Dispose();
                _ctx.Dispose();
            }
            _disposed = true;
        }

        #endregion
    }
}
