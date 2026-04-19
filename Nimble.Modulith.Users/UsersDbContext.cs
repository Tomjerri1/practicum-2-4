using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Nimble.Modulith.Users;


public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        

        builder.HasDefaultSchema("users");
    }
}