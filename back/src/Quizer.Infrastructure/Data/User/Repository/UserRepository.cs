using Microsoft.EntityFrameworkCore;
using Quizer.UserAggregate;


namespace Quizer.Infrastructure.Data.User.Repository;

public class UserRepository : IUserRepository
{
    private readonly QuizerDbContext _dbContext;

    public UserRepository(QuizerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAggregate.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(UserAggregate.User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(UserAggregate.User user)
    {
        _dbContext.Users.Update(user);
    }
}
