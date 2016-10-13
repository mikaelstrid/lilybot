using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Linq;

namespace Lilybot.Positioning.Infrastructure
{
    public class BodyApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (AuthorizeRequest(actionContext)) return;

            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        private static bool AuthorizeRequest(HttpActionContext actionContext)
        {
            try
            {
                var httpBody = actionContext.Request.Content.ReadAsStringAsync().Result;
                dynamic dynamicBody = JObject.Parse(httpBody);
                var actualApiKey = (string) dynamicBody.apiKey;
                var allowedApiKeys = ConfigurationManager.AppSettings["ApiKeys"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());
                return allowedApiKeys.Contains(actualApiKey);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
