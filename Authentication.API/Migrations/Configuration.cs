using System.Collections.Generic;
using Lily.Authentication.API.Entities;

namespace Lily.Authentication.API.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AuthContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AuthContext context)
        {
            if (context.Clients.Any())
            {
                return;
            }

            context.Clients.AddRange(BuildClientsList());
            context.SaveChanges();
        }

        private static IEnumerable<Client> BuildClientsList()
        {
            var clientsList = new List<Client>
            {
                new Client
                {
                    Id = "lily.shoppinglist",
                    Secret = Helper.GetHash("abc@123"),
                    Name = "Shopping list front-end application",
                    ApplicationType = Models.ApplicationTypes.JavaScript,
                    Active = true,
                    AllowedOrigin = "http://localhost:8000"
                }
            };

            return clientsList;
        }
    }
}
