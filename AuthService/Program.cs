using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

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
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//[Authorize(Policy = "bra passport")]
app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "Allowed";
}).RequireAuthorization("bra passport");

app.MapGet("/login", (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "matheus"));
    claims.Add(new Claim("passport_type", "bra"));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var user = new ClaimsPrincipal(identity);
    ctx.SignInAsync("cookie", user);
    return "ok";
}).AllowAnonymous();

app.Run();