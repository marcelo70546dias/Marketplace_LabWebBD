namespace Marketplace_LabWebBD.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClientIPAddress(this HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "Unknown";
        }
    }
}
