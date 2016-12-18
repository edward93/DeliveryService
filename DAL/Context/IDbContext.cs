using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Context
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
        DbSet<T> Set<T>() where T : class;
    }
}