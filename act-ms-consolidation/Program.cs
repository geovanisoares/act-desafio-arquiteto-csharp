using act_ms_consolidation.Application.Interfaces;
using act_ms_consolidation.Application.Services;
using act_ms_consolidation.Domain.Interfaces;
using act_ms_consolidation.Infrastructure.Data;
using act_ms_consolidation.Infrastructure.Messaging;
using act_ms_consolidation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

// Configuração do RabbitMQ
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositórios e Serviços
builder.Services.AddScoped<IConsolidationRepository, ConsolidationRepository>();
builder.Services.AddScoped<IConsolidationCacheRepository, ConsolidationCacheRepository>();
builder.Services.AddScoped<IConsolidationService, ConsolidationService>();
builder.Services.AddScoped<IConsolidationCacheHandler, ConsolidationCacheHandler>();
builder.Services.AddScoped<RabbitMQConsumer>();

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "ConsolidationApp_";
});

// Background Service para o Consumer
builder.Services.AddHostedService<RabbitMQConsumerBGService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseAuthorization();
app.MapControllers();

app.Run();
