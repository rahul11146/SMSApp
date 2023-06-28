using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SMSApp.Controllers
{
    public class AccessAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Module { get; set; } //Permission string to get from controller

        public AccessAuthorizationAttribute(string module)
        {
            Module = module;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(Module))
            {
                //Validation cannot take place without any permissions so returning unauthorized
                context.Result = new UnauthorizedResult();
                return;
            }

            //if (hasAccess)
            //{
            //    return;
            //}

            context.Result = new UnauthorizedResult();
            return;

        }
    }
}