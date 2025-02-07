using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public static class ActivitySourceExtenstion
{
    public static Action<Activity, HttpRequest> EnrichHttpRequest()
    {
        return (activity, request) =>
        {
            // 記錄使用者 IP
            var clientIp = request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(clientIp))
            {
                var remoteIp = request.HttpContext.Connection.RemoteIpAddress;

                if (remoteIp != null)
                {
                    clientIp = remoteIp.MapToIPv4().ToString();

                    if (clientIp.StartsWith("::ffff:", StringComparison.OrdinalIgnoreCase))
                    {
                        clientIp = clientIp.ToLower().Replace("::ffff:", "");
                    }
                }
            }

            activity.SetTag("http.client.ip", clientIp);
        };
    }

    public static Action<Activity, HttpResponse> EnrichHttpResponse()
    {
        return (activity, response) =>
        {
            var exceptionHandlerPathFeature = response.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature == null)
            {
                return;
            }

            var exception = exceptionHandlerPathFeature.Error;
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.AddException(exception);
        };
    }
}