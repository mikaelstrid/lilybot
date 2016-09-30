using System.Configuration;
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

        private bool AuthorizeRequest(HttpActionContext actionContext)
        {
            try
            {
                var content = actionContext.Request.Content.ReadAsStringAsync().Result;
                var actualToken = HttpUtility.ParseQueryString(content)["token"];
                var expectedToken = ConfigurationManager.AppSettings["SlackToken"];
                return actualToken == expectedToken;
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
