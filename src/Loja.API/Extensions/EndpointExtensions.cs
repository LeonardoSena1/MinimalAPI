using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalAPI.Abstractions;
using MinimalAPI.Attribute;
using MinimalAPI.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace MinimalAPI.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        app.Middleware();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    public static void Middleware(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    await next();
                    return;
                }

                var apiKeyValidator = context.RequestServices.GetRequiredService<ApiKeyAttribute>();

                if (!apiKeyValidator.IsValid(context))
                    throw new UnauthorizedAccessException();

                await next();
            }
            catch (TimeoutException ex)
            {
                await HandleErrorResponseAsync(context, "A operação excedeu o tempo limite.", ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (TaskCanceledException ex)
            {
                await HandleErrorResponseAsync(context, "Erro Task Canceled", ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException ex)
            {
                await HandleErrorResponseAsync(context, "Erro ao processar a requisição externa.", ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleErrorResponseAsync(context, "Acesso não autorizado.", ex.Message, StatusCodes.Status401Unauthorized);
            }

            catch (Exception ex)
            {
                await HandleErrorResponseAsync(context, "Erro Exception", ex.Message, StatusCodes.Status400BadRequest);
            }
        });
    }
    private static async Task HandleErrorResponseAsync(
               HttpContext context,
               string errorMessage,
               string ErrorMessageLong,
               int StatusCode)
    {
        var response = new BaseResponse
        {
            Success = false,
            Message = errorMessage + " " + ErrorMessageLong,
        };
        context.Response.StatusCode = StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}
