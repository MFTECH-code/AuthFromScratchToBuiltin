var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/username", (HttpContext ctx) =>
{
    // Acessando Cookies do client
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var payload = authCookie.Split("=");
    var key = payload.First();
    var value = payload.Last();
    return value;
});

app.MapGet("/login", (HttpContext ctx) =>
{
    // Criamos um cookie no client, onde posteriormente poderemos acessar para obter informações
    ctx.Response.Headers["set-cookie"] = "auth=usr:matheus";
    return "ok";
});

app.Run();
