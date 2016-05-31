using System.Data.Entity;
using Lily.Authentication.API.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Lily.Authentication.API
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("DefaultConnection") { }

        public DbSet<Client> Clients { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}