using System.Text;
using Microsoft.Net.Http.Headers;

namespace WebApplication1.Helpers;

public static class HttpHelper
{
    public static string UserAgent(this HttpContext context)
    {
         return context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgent) 
             ? userAgent.ToString() 
             : "";
    }
    
    public static string? IpAddress(this HttpContext context) 
        => context.Connection.RemoteIpAddress?.ToString();
    
    public static string UserHash(this HttpContext context)
    {
        var userAgent= UserAgent(context);
        var ipAddress = IpAddress(context);
        
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(userAgent + ipAddress));
    }
}