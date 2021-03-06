using System;
using System.Data.Entity;
using System.Web.Configuration;
using System.Web.Http;
using Lilybot.Authentication.API;
using Lilybot.Authentication.API.Migrations;
using Lilybot.Authentication.API.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Lilybot.Authentication.API
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AuthContext, Configuration>());
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            // ReSharper disable once InconsistentNaming
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            FacebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = WebConfigurationManager.AppSettings["FacebookAppId"],
                AppSecret = WebConfigurationManager.AppSettings["FacebookAppSecret"],
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(FacebookAuthOptions);
        }
    }
}