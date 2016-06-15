using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Lily.ShoppingList.Application;

namespace Lily.ShoppingList.API.Filters
{
    public class CheckIfFriendActionFilter : ActionFilterAttribute, IAutofacActionFilter
    {
        private readonly IProfileRepository _repository;

        public CheckIfFriendActionFilter(IProfileRepository repository)
        {
            _repository = repository;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var username = actionContext.RequestContext.Principal.Identity.Name;
            var friend = Task.Run(() => _repository.GetFriend(username)).Result;
            actionContext.Request.Properties["username"] = friend?.Username ?? username;
            base.OnActionExecuting(actionContext);
        }
    }
}