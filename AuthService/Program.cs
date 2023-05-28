using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme);

var app = builder.Build();

app.UseAuthentication();

app.Use((ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/login"))
        return next();
    
    if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    {
        // Not Authorized, (not authenticated)
        ctx.Response.StatusCode = 401;
        return Task.CompletedTask;
    }
    
    if (!ctx.User.HasClaim("passport_type", "bra"))
    {
        // Not Allowed, (you are authenticated but you dont have permission to access this content)
        ctx.Response.StatusCode = 403;
        return Task.CompletedTask;
    }
    
    return next();
});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "Allowed";
});

app.MapGet("/login", (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "matheus"));
    claims.Add(new Claim("passport_type", "bra"));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var user = new ClaimsPrincipal(identity);
    ctx.SignInAsync("cookie", user);
    return "ok";
});

app.Run();