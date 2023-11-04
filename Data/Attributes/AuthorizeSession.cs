using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication1.Helpers;

namespace WebApplication1.Data.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeSession : ActionFilterAttribute
{
   public override void OnActionExecuting(ActionExecutingContext context)
   {
      if (context.HttpContext.Session.GetInt32("user") is null)
      {
         context.Result = new UnauthorizedResult();
         return;
      }

      if (context.HttpContext.Session.GetString("userHash") != context.HttpContext.UserHash())
      {
         context.HttpContext.Session.Clear();
         context.Result = new UnauthorizedResult();
         return;
      }
      
      base.OnActionExecuting(context);
   }
}