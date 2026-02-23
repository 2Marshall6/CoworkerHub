using Microsoft.EntityFrameworkCore;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Infrastructure.Repositories;
using CoworkerHub.API.MappingProfiles;
using CoworkerHub.Application.Services;
using CoworkerHub.Infrastructure.Persistens;
using FluentValidation;
using CoworkerHub.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkspaceDTO>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
