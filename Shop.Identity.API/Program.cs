using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Shop.Identity.API.Domain;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer("Data Source=LAPTOP-L8T1DF78\\SQL17;Initial Catalog=Shop.Identity;User ID=sa;Password=welcome;TrustServerCertificate=True");
    options.UseOpenIddict();
});

services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("connect/token");
        options.AllowClientCredentialsFlow();
        options.AllowPasswordFlow()
               .AllowRefreshTokenFlow();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate()
               .DisableAccessTokenEncryption();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .DisableTransportSecurityRequirement();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

services.AddAuthorization();

services.AddAuthorization(options =>
{
    options.AddPolicy("weatherapi", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "weatherapi");
    });
});

//services.AddControllers();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseForwardedHeaders();
app.UseRouting();
//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();
//app.MapDefaultControllerRoute();

async Task SeedDataAsync(IServiceProvider services)
{
    // Seed test client
    var appManager = services.GetRequiredService<IOpenIddictApplicationManager>();
    if (await appManager.FindByClientIdAsync("my_client") is null)
    {
        await appManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "my_client",
            ClientSecret = "secret",
           
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.GrantTypes.ClientCredentials
            }
        });
    }

    var scopeManager = services.GetRequiredService<IOpenIddictScopeManager>();
    if (await scopeManager.FindByNameAsync("weatherapi") is null)
    {
        await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
        {
            Name = "weatherapi",
            DisplayName = "Access to the Weather API",
            Resources =
            {
                // Optional: name of your resource server
                "resource_server"
            }
        });
    }

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userManager.FindByNameAsync("alice") is null)
    {
        var alice = new ApplicationUser { UserName = "alice", Email = "alice@example.com", EmailConfirmed = true };
        await userManager.CreateAsync(alice, "mypassword");
    }
}

using (var scope = app.Services.CreateScope())
{
    await SeedDataAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/weatherforecast", () =>
{
    return "Hello World!";
})
.RequireAuthorization("weatherapi")
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("connect/token", async (HttpContext httpContext, IOpenIddictApplicationManager _applicationManager) =>
{
    var request = httpContext.GetOpenIddictServerRequest();
    if (request.IsPasswordGrantType())
    {
        // Note: the client credentials are automatically validated by OpenIddict:
        // if client_id or client_secret are invalid, this action won't be invoked.

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            throw new InvalidOperationException("The application cannot be found.");

        // Create a new ClaimsIdentity containing the claims that
        // will be used to create an id_token, a token or a code.
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType/*, Claims.Name, Claims.Role*/);

        identity.SetScopes(new[]
        {
            Scopes.OpenId,    // if you ever want an id_token
            "weatherapi",                         // your custom API scope
            Scopes.OfflineAccess
        });

        // Use the client_id as the subject identifier.
        identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
        //identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

        var username = request.Username;
        var password = request.Password;

        identity.SetClaim(Claims.Name, username);
        identity.AddClaim("passwrod", password);

        identity.SetDestinations(claim => [Destinations.AccessToken/*, Destinations.IdentityToken*/]);

        return Results.SignIn(new ClaimsPrincipal(identity),
            new AuthenticationProperties(),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    return Results.BadRequest("The specified grant is not implemented.");
});

app.Run();

