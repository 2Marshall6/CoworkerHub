using CoworkerHub.API.Extensions;
using CoworkerHub.API.Middlewares;
using CoworkerHub.Infrastructure.Persistens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurationServices(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.Services.InitializeInfrastructureAsync();
app.Run();