using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XChange.Web.Filters
{
    public class RedirectIfNotLoginForMfaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            int? idUser = httpContext.Session.GetInt32("PendingUserId");
            string? email = httpContext.Session.GetString("PendingUserEmail");
            string? firstName = httpContext.Session.GetString("PendingUserName");

            var user = httpContext.User;

            if (idUser == null || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
