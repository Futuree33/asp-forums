using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Data.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeSession : ActionFilterAttribute
{
   public override void OnActionExecuting(ActionExecutingContext context)
   {
      if (context.HttpContext.Session.GetInt32("user") is null)
         context.Result = new UnauthorizedResult();
      
      base.OnActionExecuting(context);
   }
}