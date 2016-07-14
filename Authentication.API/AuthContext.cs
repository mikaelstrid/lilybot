using System.Data.Entity;
using Lilybot.Authentication.API.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Lilybot.Authentication.API
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("DefaultConnection") { }

        public DbSet<Client> Clients { get; set; }
    }
}