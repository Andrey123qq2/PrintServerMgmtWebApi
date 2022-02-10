using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class SwaggerAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public SwaggerAuthorizationMiddleware(RequestDelegate next, ILogger<SwaggerAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                if (context.User.Identity.IsAuthenticated && context.User.IsInRole("PrintServer_Admins"))
                {
                    await _next.Invoke(context).ConfigureAwait(false);
                    return;
                }
                _logger.LogWarning(12, $"API documentation endpoint unauthorized access attempt by [{context.Connection.RemoteIpAddress}]");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                await _next.Invoke(context).ConfigureAwait(false);
                
            }
        }
    }
}
