using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Configuration;
using Lilybot.Authentication.API.Entities;

namespace Lilybot.Authentication.API.Migrations
{
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
                    Id = "lilybot.shopping",
                    Secret = Helper.GetHash("abc@123"),
                    Name = "Shopping list front-end application",
                    ApplicationType = Models.ApplicationTypes.JavaScript,
                    Active = true,
                    AllowedOrigin = WebConfigurationManager.AppSettings["SeedAllowedOrigin"]
                }
            };

            return clientsList;
        }
    }
}
