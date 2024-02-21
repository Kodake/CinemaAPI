using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace BackEnd.Helpers
{
    public static class SwaggerExtensions
    {
        public static TBuilder AgregarParametrosPeliculasFiltroOpenAPI<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(opt =>
            {
                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "pagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(1)
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPorPagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "titulo",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "enCines",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean"
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "proximosEstrenos",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean"
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "generoId",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "campoOrdenar",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Enum = new List<IOpenApiAny>
                        {
                            new OpenApiString("Titulo"),
                            new OpenApiString("FechaLanzamiento")
                        }
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "ordenAscendente",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean",
                        Default = new OpenApiBoolean(true)
                    }
                });
                return opt;
            });
        }
        public static TBuilder AgregarParametrosPaginacionOpenAPI<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(opt =>
            {
                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "pagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(1)
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPorPagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });
                return opt;
            });
        }
    }
}
