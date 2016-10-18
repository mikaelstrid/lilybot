using Autofac;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<Functions>();
            builder.RegisterType<Bot>().As<IBot>();
            builder.RegisterType<SlackMessageSender>().As<ISlackMessageSender>();
            builder.RegisterType<FamilyRepository>().As<IFamilyRepository>();
            builder.RegisterType<HotspotEventRepository>().As<IHotspotEventRepository>();
        }
    }
}
