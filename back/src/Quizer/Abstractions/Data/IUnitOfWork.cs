namespace Quizer.Abstractions.Data;

/// <summary>
/// Единица работы для сохранения изменений.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
