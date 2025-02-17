using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure;

public static class WorkContext
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static string? GetClaimByKey(string key)
    {
        var httpContext = _httpContextAccessor?.HttpContext;

        if (httpContext != null)
        {
            var value = httpContext.User.Claims.FirstOrDefault(x => x.Type == key)?.Value;
            return value;
        }

        return string.Empty;
    }

    public static string? CurrentUserId =>
        _httpContextAccessor?.HttpContext?.User.FindFirstValue("user_id") ??
        _httpContextAccessor?.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);


    public static CultureInfo CurrentLanguage => Thread.CurrentThread.CurrentCulture;
}