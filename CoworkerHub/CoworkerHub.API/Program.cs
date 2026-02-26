using CoworkerHub.API.MappingProfiles;
using CoworkerHub.API.Middlewares;
using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Services;
using CoworkerHub.Infrastructure.Persistens;
using CoworkerHub.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkspaceDTO>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
