using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Application.Services;
using act_ms_consolidation.Domain.Interfaces;
using act_ms_consolidation.Infrastructure.Data;
using act_ms_consolidation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IConsolidationRepository, ConsolidationRepository>();
builder.Services.AddScoped<IConsolidationCacheRepository, ConsolidationCacheRepository>();
builder.Services.AddScoped<IConsolidationService, ConsolidationService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "ConsolidationApp_";
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
