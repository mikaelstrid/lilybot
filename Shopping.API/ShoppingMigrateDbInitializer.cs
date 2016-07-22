using System.Data.Entity;
using Lilybot.Shopping.API.Migrations;
using Lilybot.Shopping.Infrastructure;

namespace Lilybot.Shopping.API
{
    public class ShoppingMigrateDbInitializer : MigrateDatabaseToLatestVersion<ShoppingDbContext, Configuration>
    {
    }
}
