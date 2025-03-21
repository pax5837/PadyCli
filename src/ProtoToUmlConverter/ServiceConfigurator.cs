using Microsoft.Extensions.DependencyInjection;
using ProtoToUmlConverter.Services;
using ProtoToUmlConverter.Services.Converter;
using ProtoToUmlConverter.Services.Proto;
using ProtoToUmlConverter.Services.UmlGeneration;

namespace ProtoToUmlConverter;

public static class ServiceConfigurator
{
    public static IServiceCollection AddProtoToUmlServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IFilterService, FilterService>()
            .AddSingleton<IProtoParser, BasicProtoParser>()
            .AddSingleton<IUmlGeneratorFactory, UmlGeneratorFactory>()
            .AddSingleton<IProto2UmlConverter, Proto2UmlConverter>()
            .AddSingleton<IProtoToUmlConversionService, ProtoToUmlConversionService>();

        return services;
    }
}