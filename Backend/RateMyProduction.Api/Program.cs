using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.Services;
using RateMyProduction.Infrastructure.Data;
using RateMyProduction.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RateMyProductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RateMyProductionDb")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

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

if (app.Environment.IsDevelopment())
{

}

app.Run();
