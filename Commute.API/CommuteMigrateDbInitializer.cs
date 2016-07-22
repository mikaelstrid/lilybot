using System.Data.Entity;
using Lilybot.Commute.API.Migrations;
using Lilybot.Commute.Infrastructure;

namespace Lilybot.Commute.API
{
    public class CommuteMigrateDbInitializer : MigrateDatabaseToLatestVersion<CommuteDbContext, Configuration>
    {
    }
}
