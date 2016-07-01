using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;

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
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException($"The user with username '{username}' is not authenticted.");

            var existingProfile = _repository.Get(username, p => true).SingleOrDefault();
            if (existingProfile == null)
            {
                _repository.InsertOrUpdate(username, new Profile(username));
            } 

            var friend = _repository.GetFriend(username);
            actionContext.Request.Properties["username"] = friend?.Username ?? username;
            base.OnActionExecuting(actionContext);
        }
    }
}