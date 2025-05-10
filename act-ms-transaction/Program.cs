using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Services;
using act_ms_transaction.Domain.Interfaces;
using act_ms_transaction.Infrastructure.Data;
using act_ms_transaction.Infrastructure.Messaging;
using act_ms_transaction.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection"); builder.Configuration.GetConnectionString("PostgresConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IMessageService, TransactionPublisher>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.Use(async (context, next) =>
{
    var authServiceUrl = builder.Configuration["AuthService:Url"];
    var authorizationHeader = context.Request.Headers["Authorization"].ToString();

    if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Token não fornecido.");
        return;
    }

    var token = authorizationHeader.Replace("Bearer ", "");

    using var httpClient = new HttpClient();
    var requestContent = new StringContent(JsonSerializer.Serialize(new { Token = token }));
    requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var response = await httpClient.PostAsync(authServiceUrl, requestContent);

    if (!response.IsSuccessStatusCode)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Token inválido ou expirado.");
        return;
    }

    await next.Invoke();
});

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseAuthorization();

app.MapControllers();

app.Run();
