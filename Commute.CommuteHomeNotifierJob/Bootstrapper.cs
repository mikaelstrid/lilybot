using Autofac;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<Functions>();
        }
    }
}
