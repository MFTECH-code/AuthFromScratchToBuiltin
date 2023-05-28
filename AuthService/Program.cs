using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
// Adicionando proteção de dados com DataProtection
builder.Services.AddDataProtection();

var app = builder.Build();

app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    // Acessando Cookies do client
    var protector = idp.CreateProtector("auth-cookie");
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();
    // Unprotect discriptografamos o cookie no servidor e conseguimos fazer as devidas validações
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(":");
    var key = parts.First();
    var value = parts.Last();
    return value;
});

app.MapGet("/login", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    // Criamos um cookie no client, onde posteriormente poderemos acessar para obter informações
    // Protect criptografa os dados, dessa forma o que está sendo salvo neste cookie fica embaralhado no client
    ctx.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:matheus")}";
    return "ok";
});

app.Run();
