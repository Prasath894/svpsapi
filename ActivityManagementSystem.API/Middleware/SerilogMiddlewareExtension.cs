using Microsoft.AspNetCore.Builder;
using System;
namespace ActivityManagementSystem.Service.Middleware
{
    public static class SerilogMiddlewareExtension
    {
        public static IApplicationBuilder UserSerilogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SerilogMiddleware>();
        }
    }
}
