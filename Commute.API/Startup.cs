using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Lilybot.Commute.API;
using Lilybot.Commute.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Lilybot.Commute.API.Startup))]
namespace Lilybot.Commute.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            ConfigureOAuth(app);

            WebApiConfig.Register(config);

            // Autofac DI
            var builder = new ContainerBuilder();
            Bootstrapper.Bootstrap(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

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
    }
}