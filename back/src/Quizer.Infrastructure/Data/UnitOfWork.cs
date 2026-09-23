using Quizer.Abstractions.Data;

namespace Quizer.Infrastructure.Data;

public class UnitOfWork(QuizerDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
