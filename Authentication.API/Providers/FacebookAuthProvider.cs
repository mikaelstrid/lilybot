using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;

namespace Lily.Authentication.API.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        // http://stackoverflow.com/questions/25646055/facebook-popup-login-with-owin
        public override void ApplyRedirect(FacebookApplyRedirectContext context) {
            context.Response.Redirect(context.RedirectUri + "&display=popup");
        }

        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}