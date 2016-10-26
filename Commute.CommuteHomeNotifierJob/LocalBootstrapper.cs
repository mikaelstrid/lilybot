using Autofac;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class LocalBootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<Functions>();
            builder.RegisterType<Bot>().As<IBot>();
            builder.RegisterType<StateRepository>().As<IStateRepository>();
        }
    }
}
