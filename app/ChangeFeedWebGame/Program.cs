using ChangeFeedWebGame;
using ChangeFeedWebGame.Components;
using ChangeFeedWebGame.Options;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.RegisterConfiguration();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static class ProgramExtensions
{
    public static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<CosmosDb>()
            .Bind(builder.Configuration.GetSection(nameof(CosmosDb)));
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<GameService, GameService>((provider) =>
        {
            var cosmosDbOptions = provider.GetRequiredService<IOptions<CosmosDb>>();
            if (cosmosDbOptions is null)
            {
                throw new ArgumentException($"{nameof(IOptions<CosmosDb>)} was not resolved through dependency injection.");
            }
            else
            {
                return new GameService(
                    endpoint: cosmosDbOptions.Value?.Endpoint ?? String.Empty,
                    database: cosmosDbOptions.Value?.Database ?? String.Empty,
                    container: cosmosDbOptions.Value?.Container ?? String.Empty
                );
            }
        });
    }
}