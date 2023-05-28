using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
// Adicionando proteção de dados com DataProtection
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();
app.Use((ctx, next) =>
{
    // Accessing the cookie into Request Headers
    // Unprotect the data of cookie using DataProtection.Unprotect to discryptograph this cookie value
    // Add the data of user into HttpContext.User using Identity

    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(":");
    var key = parts.First();
    var value = parts.Last();
    
    var claims = new List<Claim>();
    claims.Add(new Claim(key, value));
    var identity = new ClaimsIdentity(claims);
    ctx.User = new ClaimsPrincipal(identity);
    
    return next();
});

app.MapGet("/username", (HttpContext ctx) =>
{
    // Accessing user data from HttpContext
    return ctx.User.FindFirst("usr").Value;
});

app.MapGet("/login", (AuthService authService) =>
{
    // Separate the logical of signIn into a service
    authService.SignIn();
    return "ok";
});

app.Run();

public class AuthService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IDataProtectionProvider _idp;

    public AuthService(IHttpContextAccessor accessor, IDataProtectionProvider idp)
    {
        _accessor = accessor;
        _idp = idp;
    }

    public void SignIn()
    {
        // Creating cookie into Response Headers
        // Protecting the cookie data with DataProtect.Protect, this method crypthograph the cookie value
        var protector = _idp.CreateProtector("auth-cookie");
        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:matheus")}";
    }
}