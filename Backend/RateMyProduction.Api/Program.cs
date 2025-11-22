using Microsoft.EntityFrameworkCore;
using RateMyProduction.Core.Entities;
using RateMyProduction.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RateMyProductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RateMyProductionDb")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
