using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Saunter;

namespace StreetlightsAPI;

public static class AsyncApiConfigureExtensions
{
    public static IServiceCollection AddAsyncApiInternal(this IServiceCollection services)
    {
        services.AddAsyncApiSchemaGeneration(new Configurator(services).Configure);
        return services;
    }
     
     
    public static IEndpointRouteBuilder MapAsyncApiEndpoints(this IEndpointRouteBuilder e) 
    {
        e.MapAsyncApiDocuments();
        e.MapAsyncApiUi();

        return e;
    }
     
}