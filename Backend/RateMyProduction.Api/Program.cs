using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.Services;
using RateMyProduction.Infrastructure.Data;
using RateMyProduction.Infrastructure.Repositories;
using NSwag;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RateMyProductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RateMyProductionDb")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "RateMyProduction API";
    config.Version = "v1";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
