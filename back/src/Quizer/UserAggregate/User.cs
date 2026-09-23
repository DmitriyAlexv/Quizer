using Quizer.Common;

namespace Quizer.UserAggregate;

/// <summary>
/// Агрегат пользователя.
/// </summary>
public class User(string name) : AggregateRoot
{
    public string Name { get; private set; } = name;

    private User() : this(string.Empty)
    {
    }

    public void Update(string name)
    {
        Name = name;
    }
}
