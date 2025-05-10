using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace act_ms_transaction.Application.Exeptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BusinessException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                };

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    status = context.Response.StatusCode
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
