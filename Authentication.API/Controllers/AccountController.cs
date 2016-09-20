using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Lilybot.Authentication.API.Models;
using Lilybot.Authentication.API.Results;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;

namespace Lilybot.Authentication.API.Controllers
{
    [RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private readonly AuthRepository _repo;

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [Route("login")]
        [HttpGet]
        public async Task<IHttpActionResult> Login(string provider)
        {
            if (!User.Identity.IsAuthenticated) return new ChallengeResult(provider, this);

            var redirectUri = ParseRedirectUri(Request);
            if (!ValidateClient(Request, redirectUri, _repo)) return BadRequest("The client id or redirect URI is not allowed.");

            var loginData = LoginData.FromIdentity(User.Identity as ClaimsIdentity);
            if (loginData == null) return InternalServerError();

            if (loginData.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var identityUser = await _repo.FindAsync(new UserLoginInfo(loginData.LoginProvider, loginData.ProviderKey));
            var hasRegistered = identityUser != null;

            var resultRedirectUri =
                $"{redirectUri.AbsoluteUri}#external_access_token={loginData.ExternalAccessToken}&provider={loginData.LoginProvider}&haslocalaccount={hasRegistered}&external_user_name={loginData.UserName}";

            return Redirect(resultRedirectUri);
        }

        [Route("register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterApiModel model)
        {
            var verifiedToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedToken == null) return BadRequest("Invalid external access token");

            var existingUser = await _repo.FindAsync(new UserLoginInfo(model.Provider, verifiedToken.user_id));
            if (existingUser != null) return BadRequest("User is already registered");

            var newUser = new IdentityUser { UserName = verifiedToken.user_id };
            await _repo.CreateAsync(newUser);
            await _repo.AddLoginAsync(newUser.Id, new UserLoginInfo(model.Provider, verifiedToken.user_id));

            var newAccessToken = GenerateAccessToken(verifiedToken.user_id, verifiedToken.expires_at);
            return Ok(newAccessToken);
        }

        [Route("access-token")]
        [HttpGet]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {
            var verifiedToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedToken == null) return BadRequest("Invalid external access token");

            var identityUser = await _repo.FindAsync(new UserLoginInfo(provider, verifiedToken.user_id));
            if (identityUser == null) return BadRequest("External user is not registered");

            return Ok(GenerateAccessToken(identityUser.UserName, verifiedToken.expires_at));
        }



        private static Uri ParseRedirectUri(HttpRequestMessage request)
        {
            Uri redirectUri;
            return Uri.TryCreate(GetQueryStringValue(request, "redirect_uri"), UriKind.Absolute, out redirectUri) 
                ? redirectUri 
                : null;
        }

        private static bool ValidateClient(HttpRequestMessage request, Uri redirectUri, AuthRepository authRepository)
        {
            var clientId = GetQueryStringValue(request, "client_id");
            if (string.IsNullOrWhiteSpace(clientId)) return false;

            var client = authRepository.FindClient(clientId);
            if (client == null) return false;

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        private static string GetQueryStringValue(HttpRequestMessage request, string key)
        {
            var keyValues = request.GetQueryNameValuePairs();
            if (keyValues == null) return null;

            var match = keyValues.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);
            return !string.IsNullOrEmpty(match.Value) ? match.Value : null;
        }

        private static async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            if (provider != "Facebook") return null;
            
            //You can get it from here: https://developers.facebook.com/tools/accesstoken/
            //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
            var appToken = WebConfigurationManager.AppSettings["FacebookAppToken"];
            string verifyTokenEndPoint = 
                $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appToken}";

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            dynamic jObject = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

            var parsedToken = new ParsedExternalAccessToken
            {
                user_id = jObject["data"]["user_id"],
                app_id = jObject["data"]["app_id"],
                expires_at = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(jObject["data"]["expires_at"]))
            };
            
            if (!string.Equals(Startup.FacebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                return null;

            return parsedToken;
        }

        private static JObject GenerateAccessToken(string userName, DateTimeOffset expiresAt)
        {
            var claimsIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userName));
            claimsIdentity.AddClaim(new Claim("role", "user"));

            var authenticationProperties = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = expiresAt.UtcDateTime
            };

            var ticket = new AuthenticationTicket(claimsIdentity, authenticationProperties);
            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            var tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );

            return tokenResponse;
        }


        private class LoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static LoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null) return null; 

                var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(providerKeyClaim?.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value)) return null;

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer) return null;

                return new LoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }
    }
}
