using Microsoft.AspNetCore.Http;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ActivityManagementSystem.CrossCutting.Helper;
using Serilog;

namespace ActivityManagementSystem.Service.Middleware
{
    public class SerilogMiddleware
    {
    private const string MessageTemplate =
               "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    private static readonly ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();

    private readonly RequestDelegate _next;

    public SerilogMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var start = StopwatchHelper.GetCurrentTimeStamp;
        try
        {
            await _next(httpContext);
            var elapsedMilliseconds = StopwatchHelper.GetElapsedMilliseconds(start);

            var statusCode = httpContext.Response?.StatusCode;
            var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

            var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;
            log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode,
                elapsedMilliseconds);
        }
        // Never caught, because `LogException()` returns false.
        catch (Exception ex) when (LogException(httpContext,
             StopwatchHelper.GetElapsedMilliseconds(start), ex))
        {
        }
    }

    private static bool LogException(HttpContext httpContext, string elapsedMs, Exception ex)
    {
        LogForErrorContext(httpContext)
            .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, elapsedMs);

        return false;
    }

    private static ILogger LogForErrorContext(HttpContext httpContext)
    {
        var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), true)
            .ForContext("RequestHost", request.Host)
            .ForContext("RequestProtocol", request.Protocol);

        if (request.HasFormContentType)
            result = result.ForContext("RequestForm",
                request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

        return result;
    }

}
}