using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Lilybot.Shopping.Infrastructure
{
    public class SlackTokenAuthorizeAttribute : AuthorizeAttribute
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
                var content = actionContext.Request.Content.ReadAsStringAsync().Result;
                var actualToken = HttpUtility.ParseQueryString(content)["token"];
                var allowedTokens = ConfigurationManager.AppSettings["SlackToken"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());
                return allowedTokens.Contains(actualToken);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
