using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw6.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            String str = " ";
            str += "---------------------\n" +
                httpContext.Request.Method + "\n" +
                httpContext.Request.Path + "\n";
            var bodyStream = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }
            str += bodyStream + "\n" +
                httpContext.Request.Query + "\n" +
                "---------------------\n";
            File.AppendAllText("log.txt", str);
            await _next(httpContext);
        }
    }
}
