using System.Reflection;
using Application;
using Infrastructure;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();

	app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

await app.RunAsync();

namespace Web.Api
{
    public partial class Program;
}
