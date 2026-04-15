using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XChange.Web.Filters
{
    public class RedirectIfNotVerifyEmailAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            
            int? idUser = httpContext.Session.GetInt32("PendingUserId");
            string? email = httpContext.Session.GetString("EmailVerificationToken");
            string? token = httpContext.Session.GetString("PendingSecurityToken");

            var user = httpContext.User;

            if (idUser == null || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                httpContext.Session.Clear();
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
