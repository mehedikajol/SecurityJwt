using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Domain.Common;
using SecurityJwt.Infrastructure.Configuration;
using SecurityJwt.Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConneciton")));

// generate classes from appsettings 
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
