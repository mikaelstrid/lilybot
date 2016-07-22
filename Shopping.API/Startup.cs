using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.API;
using Lilybot.Shopping.API.Controllers;
using Lilybot.Shopping.API.Filters;
using Lilybot.Shopping.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Lilybot.Shopping.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            ConfigureOAuth(app);

            WebApiConfig.Register(config);

            // Autofac DI
            var builder = new ContainerBuilder();
            Bootstrapper.Bootstrap(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            RegisterControllerActionFilters(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private static void ConfigureOAuth(IAppBuilder app)
        {
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private static void RegisterControllerActionFilters(ContainerBuilder builder)
        {
            builder.Register(c => new CheckIfFriendActionFilter(c.Resolve<IShoppingProfileRepository>()))
                .AsWebApiActionFilterFor<FriendsApiControllerBase>()
                .InstancePerRequest();
        }
    }
}