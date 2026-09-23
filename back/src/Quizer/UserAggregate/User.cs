using Quizer.Common;

namespace Quizer.UserAggregate;

/// <summary>
/// Агрегат пользователя.
/// </summary>
public class User : AggregateRoot
{
    public string Name { get; private set; }
    
    public User(Guid id, string name) : base(id)
    {
        Name = name;
    }

    private User()
    {
        Name = string.Empty;
    }

    public void Update(string name)
    {
        Name = name;
    }
}
