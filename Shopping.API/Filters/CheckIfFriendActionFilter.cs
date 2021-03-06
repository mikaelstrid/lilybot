using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.API.Filters
{
    public class CheckIfFriendActionFilter : ActionFilterAttribute, IAutofacActionFilter
    {
        private static object _lock = new object();

        private readonly IShoppingProfileRepository _repository;

        public CheckIfFriendActionFilter(IShoppingProfileRepository repository)
        {
            _repository = repository;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var username = actionContext.RequestContext.Principal.Identity.Name;
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException($"The user with username '{username}' is not authenticted.");

            lock (_lock)
            {
                var existingProfile = _repository.Get(username, p => true).SingleOrDefault();
                if (existingProfile == null)
                {
                    _repository.InsertOrUpdate(username, new ShoppingProfile(username));
                }
            }

            var friend = _repository.GetFriend(username);
            actionContext.Request.Properties["username"] = friend?.Username ?? username;
            base.OnActionExecuting(actionContext);
        }
    }
}