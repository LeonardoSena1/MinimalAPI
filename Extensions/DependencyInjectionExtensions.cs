using MinimalAPI.Attribute;

namespace MinimalAPI.Extensions;

public static class DependencyInjectionExtensions
{
    public static void ConfigureDependencyService(this WebApplicationBuilder app)
    {
        app.Services.AddSingleton<ApiKeyAttribute>();
    }
}

