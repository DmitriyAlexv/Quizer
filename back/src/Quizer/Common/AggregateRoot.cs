namespace Quizer.Common;

/// <summary>
/// Базовый класс для агрегатов.
/// </summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }
}
