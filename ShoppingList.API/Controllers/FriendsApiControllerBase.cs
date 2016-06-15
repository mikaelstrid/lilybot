using System.Web.Http;

namespace Lily.ShoppingList.Api.Controllers
{
    public abstract class FriendsApiControllerBase : ApiController
    {
        protected string Username => ActionContext.Request.Properties["username"].ToString();
    }
}