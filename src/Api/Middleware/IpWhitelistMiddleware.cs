
using System.Net;

namespace Api.Middleware
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpWhitelistMiddleware> _logger;
        private readonly List<IPAddress> _whitelist;

        public IpWhitelistMiddleware(RequestDelegate next, ILogger<IpWhitelistMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;

            var ipWhitelist = configuration.GetSection("IpWhitelist").Get<string[]>();
            _whitelist = new List<IPAddress>();
            if (ipWhitelist != null)
            {
                foreach (var ip in ipWhitelist)
                {
                    if (IPAddress.TryParse(ip, out var parsedIp))
                    {
                        _whitelist.Add(parsedIp);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid IP address in whitelist: {IpAddress}", ip);
                    }
                }
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp == null)
            {
                _logger.LogWarning("Request from unknown IP address was blocked.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: IP address could not be determined.");
                return;
            }

            // Check if the IP is in the whitelist or if it's a loopback address
            if (!_whitelist.Any(ip => ip.Equals(remoteIp)) && !IPAddress.IsLoopback(remoteIp))
            {
                _logger.LogWarning("Forbidden request from IP address: {RemoteIp}", remoteIp);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Your IP address is not authorized.");
                return;
            }

            await _next(context);
        }
    }
}
