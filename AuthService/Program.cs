using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme);

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("bra passport", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthScheme)
            .RequireClaim("passport_type", "bra");
    });
    
    builder.AddPolicy("role_permission", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthScheme)
            .RequireRole("admin");
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//[Authorize(Policy = "bra passport")]
app.MapGet("/sweden", () =>
{
    return "Allowed";
}).RequireAuthorization("bra passport");

app.MapGet("/admin", () =>
{
    return "Welcome admin";
}).RequireAuthorization("role_permission");

app.MapGet("/login", (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "matheus"));
    claims.Add(new Claim("passport_type", "bra"));
    //claims.Add(new Claim("role", "admin"));
    claims.Add(new Claim("role", "user"));
    var identity = new ClaimsIdentity(claims, AuthScheme, null, "role");
    var user = new ClaimsPrincipal(identity);
    ctx.SignInAsync("cookie", user);
    return "ok";
}).AllowAnonymous();

app.Run();