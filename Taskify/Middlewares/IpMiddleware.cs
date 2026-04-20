namespace Taskify.Middlewares
{
    public class IpMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly List<string> _blockedIps = new() { "192.168.56.1", "127.0.0.1" };
        public IpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? requestIp = context.Connection.RemoteIpAddress?.ToString();

            // בדיקה אם ה-IP נמצא בכותרת X-Forwarded-For
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                requestIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            if (_blockedIps.Contains(requestIp))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied: Your IP address is blocked.");
                return;
            }

            await _next(context);
        }

    }
}
