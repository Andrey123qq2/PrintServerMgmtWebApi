using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            string requestMethodName = Regex.Match(
                context.Request.Path, 
                @"/([^/]+)/?$", 
                RegexOptions.IgnoreCase
            ).Groups[1].Value.ToString().ToLower();
            int eventId = RequestLoggingEventIDFactory.Create(requestMethodName);
            _logger.LogInformation(eventId, await FormatRequest(context));
            _logger.LogInformation(11, await FormatResponse(context));
        }

        private async Task<string> FormatRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            return $"Request Information:{Environment.NewLine}" +
                $"Method: {context.Request.Method} \n" +
                $"Uri: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString} \n" +
                $"Request Body: {requestBody} \n" +
                $"User: {context.User.Identity.Name}";
        }

        private async Task<string> FormatResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseStream;
            await _next(context);

            context.Response.Body.Position = 0;
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Position = 0;
            await responseStream.CopyToAsync(originalBodyStream);
            return $"Response Information:{Environment.NewLine} \n" +
                $"Uri: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString} => {context.Response.StatusCode} \n" +
                $"Response Body: {responseBody} \n";
            
        }
    }
}
