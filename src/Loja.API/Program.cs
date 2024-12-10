using Asp.Versioning;
using Asp.Versioning.Builder;
using MinimalAPI.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwaggerGenService();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.ConfigureDependencyService();

builder.Services.AddEndpoints(typeof(Program).Assembly);

WebApplication app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.ConfigureUseSwagger();

app.UseHttpsRedirection();

app.Run();
