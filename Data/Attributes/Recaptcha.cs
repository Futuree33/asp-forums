using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Data.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class Recaptcha : ActionFilterAttribute
{
    private record RecaptchaResponse(bool success);
    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var recaptchaToken = context.HttpContext.Request.Query["recaptchaToken"];
        
        if (recaptchaToken.Count == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>()!;
        var recaptchaSecret = configuration["Recaptcha:Secret"];
        
        using var httpClient = new HttpClient();
        
        var queryParams = new Dictionary<string, string>
        {
            {"secret", recaptchaSecret!},
            {"response", recaptchaToken!}
        };
        
        var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(queryParams));
        
        if (!response.IsSuccessStatusCode) 
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<RecaptchaResponse>(content);
    
        if (json is null || !json.success)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        await base.OnActionExecutionAsync(context, next);
    }
}