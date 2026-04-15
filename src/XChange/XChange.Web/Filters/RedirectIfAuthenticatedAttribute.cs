using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XChange.Web.Filters
{
    // Atributo propio para regresar a un usuario si ya está logueado.
    public class RedirectIfAuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Dashboard", "User", null);
            }

            base.OnActionExecuting(context);
        }
    }
}