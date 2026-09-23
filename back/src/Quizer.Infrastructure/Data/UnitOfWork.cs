using Quizer.Abstractions.Data;

namespace Quizer.Infrastructure.Data;

public class UnitOfWork(QuizerDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
