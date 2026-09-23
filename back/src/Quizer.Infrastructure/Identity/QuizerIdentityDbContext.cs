using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Quizer.Infrastructure.Identity;

/// <summary>
/// Отдельный контекст базы данных для Identity.
/// </summary>
public class QuizerIdentityDbContext(DbContextOptions<QuizerIdentityDbContext> options)
    : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
}
