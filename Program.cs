using APIGetway.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

var routes = "Routes";

builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});

builder.Services.AddOcelot(builder.Configuration).AddPolly();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddOcelot(routes, builder.Environment)
    .AddEnvironmentVariables();

builder.Services.AddAuthentication(options =>
{

    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://localhost:7275"; // This is the OAuth20.Server URI
        options.ClientId = "2";
        options.ClientSecret = "123456789";
        options.ResponseType = "code";
        options.CallbackPath = "/signin-oidc";
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("jwtapitestapp.read");
        options.Scope.Add("profile");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = "https://localhost:7275",
            ValidIssuer = "https://localhost:7275",
            ValidateLifetime = false,

            //IssuerSigningKey = rsaSecurityKey

        };
    });
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();

// Swagger for ocelot
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerForOcelotUI(options =>
{
   options.PathToSwaggerGenerator = "/swagger/docs";
   options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;

}).UseOcelot().Wait();

app.MapDefaultControllerRoute();

app.Run();
