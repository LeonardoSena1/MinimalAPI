using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MinimalAPI.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// swagger UseSwagger
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureUseSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        /// <summary>
        /// swagger configuration
        /// </summary>
        /// <param name="servicesCollection"></param>
        public static void ConfigureSwaggerGenService(this IServiceCollection servicesCollection)
        {
            servicesCollection.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Minimal API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Dev",
                        Email = "suporte@teste.com.br"
                    },
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.DocInclusionPredicate((docName, description) => true);

                options.EnableAnnotations();

                options.ResolveConflictingActions(e => e.First());

                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Name = "Api-Key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization by x-api-key inside request's header",
                    Scheme = "Api-Key"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                       { new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey",
                                },
                                In = ParameterLocation.Header
                            }, new List<string>()
                       }
                    });
            }).AddSwaggerGenNewtonsoftSupport();
        }
    }
}
